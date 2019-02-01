﻿
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

/// <summary>
/// Controls the HelloAR example.
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
    /// A model to place when a raycast from a user touch hits a plane.
    /// </summary>
    public GameObject AndyPlanePrefab;

    /// <summary>
    /// A model to place when a raycast from a user touch hits a feature point.
    /// </summary>
    public GameObject AndyPointPrefab;

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
    private const float waterDepth = 0.25f;

    /// <summary>
    /// Returns false if the detected plane is filtered away and should be ignored.
    /// </summary>
    private bool PassesFilter(DetectedPlane plane)
    {
        const float horizontalDiffCosLowerLimit = 0.6f;
        const float currentDiffCosLowerLimit = 0.8f;
        const float currentDiffPosYUpperLimit = 0.05f;
        const float currentDiffPosXLUpperLimit = 0.2f;

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
            float sqrDiffPosY = diffPos.y * diffPos.y;
            if (sqrDiffPosY > currentDiffPosYUpperLimit)
            {
                return false; // Too different Y position from current
            }
            if (new Vector2(diffPos.x, diffPos.z).sqrMagnitude > currentDiffPosXLUpperLimit)
            {
                return false; // Too different XZ position from current
            }
        }

        // Filter passed
        return true;
    }

    /// <summary>
    /// Returns a new water surface.
    /// </summary>
    private GameObject newWaterSurfaceFrom(DetectedPlane plane)
    {
        const float planeScale = 0.1f;
        GameObject waterSurface = GameObject.CreatePrimitive(PrimitiveType.Plane);
        waterSurface.transform.position = plane.CenterPose.position;
        waterSurface.transform.position += waterDepth* plane.CenterPose.up;
        waterSurface.transform.localScale = new Vector3(planeScale* plane.ExtentX, planeScale, planeScale* plane.ExtentZ);
        waterSurface.transform.rotation = plane.CenterPose.rotation;
        return waterSurface;
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        _UpdateApplicationLifecycle();

        // Hide snackbar when currently tracking at least one plane.
        Session.GetTrackables<DetectedPlane>(m_AllPlanes);
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


        // Make out water surface
        foreach (var plane in m_AllPlanes)
        {
            GameObject surface = newWaterSurfaceFrom(plane);

            // Set current water surface
            if (waterSurface == null)
            {
                waterSurface = surface;
                waterSurface.GetComponent<Renderer>().sharedMaterial.color = Color.green;
            }
            else
            {
                surface.GetComponent<Renderer>().sharedMaterial.color = Color.red;
            }
        }

        SearchingForPlaneUI.SetActive(showSearchingUI);

        // If the player has not touched the screen, we are done with this update.
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Choose the Andy model for the Trackable that got hit.
                GameObject prefab;
                if (hit.Trackable is FeaturePoint)
                {
                    prefab = AndyPointPrefab;
                }
                else
                {
                    prefab = AndyPlanePrefab;
                }

                // Instantiate Andy model at the hit pose.
                var andyObject = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

                // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
                andyObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make Andy model a child of the anchor.
                andyObject.transform.parent = anchor.transform;
            }
        }
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
