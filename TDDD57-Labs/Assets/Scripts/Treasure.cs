using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Treasure : MonoBehaviour {
    [SerializeField]
    private TextMeshPro selectionIndicator = null;

    private bool selected = false;

    void Update() {
        if (selected) {
            UpdateSelectionMarker();
        }
    }

    void OnTriggerEnter(Collider other) {
        Crewman crewman = other.GetComponent<Crewman>();
        if (crewman != null)
        {
            crewman.OnTreasureReached(this);
        }
    }

    public void ToggleSelected(bool state) {
        selected = state;
        selectionIndicator.gameObject.SetActive(state);
    }

    void UpdateSelectionMarker() {
        selectionIndicator.transform.right = Camera.main.transform.right;
    }

    private void ShowAndroidToastMessage(string message) {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null) {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
