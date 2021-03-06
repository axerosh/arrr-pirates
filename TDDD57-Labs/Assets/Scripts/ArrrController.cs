﻿using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

/// <summary>
/// Player controller script based on ARCore Controller scripts.
/// </summary>
public class ArrrController : MonoBehaviour
{
    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// ARCore Device of the scene, controlls scaling of the camera.
    /// </summary>
    public GameObject ARCoreDevice;

    public GameObject waterSurfacePrefab;
    public GameObject shipPrefab;

    /// <summary>
    /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    private GameObject waterSurface = null;
    private GameObject ship = null;
    private const float waterDepth = 0.25f;

    public GameObject boardSizeIndicator;
    public GameObject boardPlacementUI;
    public GameObject planeVisualizer;

    private int score;
    public int winCondition;

    public GamePlayUI gamePlayUI;
    private bool repositionBoard = false;

    public GameOverUI gameOverUI;

    private Selectable selected = null;

    /**
     * Returns progres towards a win.
     *
     * 0.0 -> No progress
     * 1.0 -> Win!
     */
    public float WinProgress()
    {
        return ((float)score) / (float)winCondition;
    }

    /// <summary>
    /// Returns false if the detected plane is filtered away and should be ignored.
    /// </summary>
    private bool PassesFilter(DetectedPlane plane)
    {
        const float horizontalDiffCosLowerLimit = 0.6f;
        const float currentDiffCosLowerLimit = 0.8f;
        const float currentDiffPosXZUpperLimit = 0.2f;
        const float currentDiffPosYUpperLimit = 0.1f;

        // Filter
        if (Vector3.Dot(plane.CenterPose.up, Vector3.up) <= horizontalDiffCosLowerLimit)
        {
            return false; // Non-horizontal surface
        }
        if (waterSurface != null)
        {
            if (Vector3.Dot(waterSurface.transform.up, plane.CenterPose.up) <= currentDiffCosLowerLimit)
            {
                return false; // Too different orientation from current
            }

            Vector3 diffPos = waterSurface.transform.position - waterSurface.transform.up * waterDepth - plane.CenterPose.position;
            float sqrDiffPosXZ = diffPos.x * diffPos.x  + diffPos.z + diffPos.z;
            if (sqrDiffPosXZ > currentDiffPosXZUpperLimit)
            {
                return false; // Too different XZ position from current
            }

            float sqrDiffPosY = diffPos.y * diffPos.y;
            if (sqrDiffPosY > currentDiffPosYUpperLimit)
            {
                return false; // Too different Y position from current
            }
        }
        // Filter passed
        return true;
    }

    void Start() {
        ARCoreDevice.GetComponent<ARCoreSession>().SessionConfig.CameraFocusMode = CameraFocusMode.Auto;
    }

    private void Awake() {
        gamePlayUI.SetWinCondition(winCondition);
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update() {
        _UpdateApplicationLifecycle();

        UpdateUI();

        // Filter detected surfaces
        for (int i = m_AllPlanes.Count - 1; i >= 0; i--)
        {
            if (!PassesFilter(m_AllPlanes[i]))
            {
                m_AllPlanes.RemoveAt(i);
            }
        }

        // If the player has not touched the screen, we are done with this update.
        Touch touch;
        Ray ray;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) {
                return;
        }
        else {
            ray = FirstPersonCamera.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y));
        }

        // Hit in virtual world?
        int layerMask = 1 << 10; // layer 10 (Clickable)
        Selectable newSelected = null;
        if (Physics.Raycast(ray, out RaycastHit virtualHit, Mathf.Infinity, layerMask)) {
            GameObject clicked = virtualHit.collider.GetComponent<Clickable>().mainObject;
            newSelected = clicked.GetComponent<Selectable>();
            Treasure treasure = clicked.GetComponent<Treasure>();
            Hideable treasureHideable = clicked.GetComponent<Hideable>();
            if (newSelected != null) {
                if (selected != null) {
                    selected.Deselect();
                }

                if (selected != null && clicked == selected.gameObject) {
                    // Deselect
                    selected.Deselect(); //for posterity.
                    selected = null;
                }
                else {
                    // Select new
                    selected = newSelected;
                    selected.Select();
                }
            }
            else if (treasure != null && treasureHideable != null && treasureHideable.IsVisible())
            {
                if (selected != null)
                {
                    if (!selected.SetTarget(clicked)) {
                        selected.Deselect();
                        selected = null;
                    }
                }
            }
        }
        else if (waterSurface == null || repositionBoard)
        {
            // Hit in physical world?
            // Raycast against the location the player touched to search for planes.
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out TrackableHit hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position * 100.0f,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    if (waterSurface == null)
                    {
                        CreateBoard(hit);
                    }
                    else if (repositionBoard)
                    {
                        PlaceBoard(hit);
                        ToggleRepositionBoard();
                    }
                }
            }
        }
        else
        {
            // Clicked elsewhere, deselect
            selected?.Deselect();
            selected = null;
        }
        gamePlayUI.SetSelected(newSelected);
    }

    private void UpdateUI() {
        //Deactivate the board placement hint UI if the board has already been placed.
        if (ship && UpdateTracking()) {
            boardPlacementUI.SetActive(false);
            gamePlayUI.gameObject.SetActive(true);
        } else if (!UpdateTracking()) {
            //Activate it immediately if have no tracked planes.
            gamePlayUI.gameObject.SetActive(false);
            boardPlacementUI.SetActive(true);
        }
    }

    /// <summary>
    /// Returns true if any planes are currently being tracked, else returns false.
    /// </summary>
    private bool UpdateTracking() {
        Session.GetTrackables(m_AllPlanes);
        for (int i = 0; i < m_AllPlanes.Count; i++) {
            if (m_AllPlanes[i].TrackingState == TrackingState.Tracking) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Creates the play area at the given hit and binds it to an anchor at the same locaion.
    /// </summary>
    /// <param name="hit"> A TrackableHit where the play area should be spawned. </param>
    private void CreateBoard(TrackableHit hit) {

        //Spawn
        waterSurface = Instantiate(waterSurfacePrefab);
        ship = Instantiate(shipPrefab);
        waterSurface.transform.parent = ARCoreDevice.transform;
        ship.transform.parent = ARCoreDevice.transform;

        PlaceBoard(hit);

        //Deactivate plane visualizer once we have placed the board.
        planeVisualizer.SetActive(false);
    }

    /// <summary>
    /// Places the play area at hit.
    /// </summary>
    private void PlaceBoard(TrackableHit hit)
    {
        //Set location
        waterSurface.transform.parent = ARCoreDevice.transform;
        ship.transform.parent = ARCoreDevice.transform;
        waterSurface.transform.localPosition = hit.Pose.position;
        ship.transform.localPosition = hit.Pose.position + Vector3.up * waterDepth;

        //Set rotation
        Vector3 fromCamera = ship.transform.position - FirstPersonCamera.transform.position;
        Vector3 fromCameraXZ = new Vector3(fromCamera.x, 0.0f, fromCamera.z);
        Quaternion rotation = new Quaternion();
        rotation.SetFromToRotation(Vector3.left, fromCameraXZ);
        waterSurface.transform.rotation = rotation;
        ship.transform.rotation = rotation;
    }

    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Adds amount score to the player. Amount may be negative to decrease score.
    /// </summary>
    public void AddScore(int amount) {
        score += amount;
        gamePlayUI.UpdateScoreText(score);
        if(score >= winCondition) {
            gameOverUI.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }

    /// <summary>
    /// Reactivates the planeVisualizer allowing the player to replace the board.
    /// </summary>
    public void ToggleRepositionBoard() {
        repositionBoard = !repositionBoard;
        gamePlayUI.ToggleRepositionButton();
        planeVisualizer.SetActive(repositionBoard);
    }
}
