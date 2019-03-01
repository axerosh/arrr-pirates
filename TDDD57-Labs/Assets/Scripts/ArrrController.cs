﻿using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

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
    /// A prefab for tracking and visualizing detected planes.
    /// </summary>
    public GameObject DetectedPlanePrefab;

    /// <summary>
    /// ARCore Device of the scene, controlls scaling of the camera.
    /// </summary>
    public GameObject ARCoreDevice;

    public GameObject waterSurfacePrefab;
    public GameObject shipPrefab;

    /// <summary>
    /// A game object parenting UI for displaying the "searching for planes" snackbar.
    /// </summary>
    public GameObject SearchingForPlaneUI;

    /// <summary>
    /// The rotation in degrees need to apply to model when the Andy model is placed.
    /// </summary>
    private const float k_ModelRotation = 180.0f;

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

    public void Start()
    {
        ARCoreDevice.GetComponent<ARCoreSession>().SessionConfig.CameraFocusMode = CameraFocusMode.Auto;
    }

    private GameObject selectedCharacter = null;

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        _UpdateApplicationLifecycle();

        // Hide snackbar when currently tracking at least one plane.
        Session.GetTrackables(m_AllPlanes);
        bool showSearchingUI = true;
        for (int i = 0; i < m_AllPlanes.Count; i++)
        {
            if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
            {
                showSearchingUI = false;
                break;
            }
        }

        // Filter detected surfaces
        for (int i = m_AllPlanes.Count - 1; i >= 0; i--)
        {
            if (!PassesFilter(m_AllPlanes[i]))
            {
                m_AllPlanes.RemoveAt(i);
            }
        }

        SearchingForPlaneUI.SetActive(showSearchingUI);

        // If the player has not touched the screen, we are done with this update.
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Hit in virtual world?
        RaycastHit virtualHit;
        Ray ray = FirstPersonCamera.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y));
        int layerMask = 1 << 10; // layer 10 (Clickable)
        if (Physics.Raycast(ray, out virtualHit, Mathf.Infinity, layerMask))
        {
            GameObject clicked = virtualHit.collider.gameObject;
            Character character = clicked.GetComponent<Character>();
            Target target = clicked.GetComponent<Target>();
            if (character != null)
            {
                if (selectedCharacter != null)
                {
                    selectedCharacter.GetComponent<Character>().Deselect();
                }

                if (clicked == selectedCharacter)
                {
                    // Deselect
                    selectedCharacter = null;
                }
                else
                {
                    // Select new
                    character.Select();
                    selectedCharacter = clicked;
                }
            }
            else if (target != null)
            {
                if (selectedCharacter != null)
                {
                    selectedCharacter.GetComponent<Character>().SetTarget(target);
                }
            }
        }
        else
        {
            // Hit in physical world?
            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
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
                    _ShowAndroidToastMessage("Coordinates: " + hit.Pose.position.ToString());
                    if (waterSurface == null)
                    {
                        CreateBoard(hit);
                    }
                    else
                    {
                        ReplaceBoard(hit);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Fires when the player collides with a trigger, such as the body of water.
    /// </summary>
    /// <param name="other">The Trigger object that was entered</param>
    public void OnWaterEnter() {
        _ShowAndroidToastMessage("Entered the water!");
    }

    /// <summary>
    /// Fires when the player leaves a trigger, such as the body of water.
    /// </summary>
    /// <param name="other">The Trigger object that was left.</param>
    public void OnWaterExit() {
        _ShowAndroidToastMessage("Left the water!");
    }

    /// <summary>
    /// Creates the play area at the given hit and binds it to an anchor at the same locaion.
    /// </summary>
    /// <param name="hit"> A TrackableHit where the play area should be spawned. </param>
    private void CreateBoard(TrackableHit hit) {

        //Spawn water
        waterSurface = Instantiate(waterSurfacePrefab);
        //Spawn ship
        ship = Instantiate(shipPrefab);

        //Set locatiion
        waterSurface.transform.parent = ARCoreDevice.transform;
        ship.transform.parent = ARCoreDevice.transform;
        waterSurface.transform.localPosition = hit.Pose.position;
        waterSurface.transform.localRotation = new Quaternion();
        ship.transform.localPosition = hit.Pose.position + Vector3.up * waterDepth;
        ship.transform.localRotation = new Quaternion();
    }

    /// <summary>
    /// Replaces the play area.
    /// </summary>
    private void ReplaceBoard(TrackableHit hit)
    {
        waterSurface.transform.localPosition = hit.Pose.position;
        waterSurface.transform.localRotation = new Quaternion();
        ship.transform.localPosition = hit.Pose.position + Vector3.up * waterDepth;
        ship.transform.localRotation = new Quaternion();
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
}
