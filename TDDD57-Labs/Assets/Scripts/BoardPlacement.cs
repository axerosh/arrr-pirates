using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is responsible for controlling the UI showing hints for detecting surfaces and placing the board.
/// </summary>
public class BoardPlacement : MonoBehaviour {
    private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();
    private RectTransform thisTransform;

    private readonly string SURFACE_DETECTION_HINT = "Move your phone in circles to detect surfaces";
    private readonly string BOARD_PLACEMENT_HINT = "Tap anywhere on a detected surface to place the board";

    public Image sweepImage;
    private RectTransform sweepTransform;
    public float sweepAngle;

    public Text boardPlacementHintText;

    public Button hintButton;
    public Image hintOverlay;
    private RectTransform hintOverlayTransform;
    private bool hintsActive = false;

    void Start() {
        //Get Rekt.
        thisTransform = GetComponent<RectTransform>();
        sweepTransform = sweepImage.GetComponent<RectTransform>();
        hintOverlayTransform = hintOverlay.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update() {
        bool tracking = UpdateTracking();

        if (!tracking) {
            //Show tracking hint if no surfaces are found.
            sweepTransform.gameObject.SetActive(true);
            boardPlacementHintText.text = SURFACE_DETECTION_HINT;
            //Rotate the sweep indicator image around the center of the UI.
            //sweepTransform.RotateAround(thisTransform.position, thisTransform.forward, sweepAngle);
            //sweepTransform.rotation = Quaternion.LookRotation(thisTransform.forward, thisTransform.up);
        } else {
            sweepImage.gameObject.SetActive(false);
            //Show ship-placement hint when we have surfaces available.
            boardPlacementHintText.text = BOARD_PLACEMENT_HINT;
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

    public void ToggleHints() {
        hintsActive = !hintsActive;
        hintOverlay.gameObject.SetActive(hintsActive);
        if (hintsActive) {
            hintButton.gameObject.GetComponentInChildren<Text>().text = "X";
        } else {
            hintButton.gameObject.GetComponentInChildren<Text>().text = "?";
        }
    }
}
