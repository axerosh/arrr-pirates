using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BoardPlacement : MonoBehaviour {
    private RectTransform thisTransform;

    public Image sweepImage;
    private RectTransform sweepTransform;
    public float sweepAngle;

    void Start() {
        thisTransform = GetComponent<RectTransform>();
        sweepTransform = sweepImage.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update() {
        sweepTransform.RotateAround(thisTransform.position, thisTransform.forward, sweepAngle);
        sweepTransform.rotation = Quaternion.LookRotation(thisTransform.forward, thisTransform.up);
    }
}
