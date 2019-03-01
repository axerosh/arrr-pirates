using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public Color waterFogColor;

    public float waterFogDensity;
    public float normalFogDensity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ShowAndroidToastMessage("Camera at: " + transform.position);
    }

    /// <summary>
    /// Fires when exiting an trigger collider.
    /// Specifically used to set the camera to underwater mode.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other) {
        if(other.name == "WaterHitBox") {
            RenderSettings.fogColor = waterFogColor;
            RenderSettings.fogDensity = waterFogDensity;
            ShowAndroidToastMessage("Splash");
        }
    }

    /// <summary>
    /// Fires when exiting an trigger collider.
    /// Specifically used to reset the camera when exiting the water.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other) {
        if(other.name == "WaterHitBox") {
            RenderSettings.fogDensity = normalFogDensity;
            ShowAndroidToastMessage("Plosh!");
        }
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
