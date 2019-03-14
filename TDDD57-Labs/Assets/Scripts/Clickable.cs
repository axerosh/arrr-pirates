using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    public GameObject mainObject;

    public float minScale = 1.0f;
    public float maxScale = 6.0f;

    public float minDistance = 10.0f;
    public float maxDistance = 100.0f;

    private float distanceToScale;
    private Vector3 baseScale;

    private void Start()
    {
        distanceToScale = (maxScale - minScale) / (maxDistance - minDistance);
        baseScale = transform.localScale;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        float scale;
        if (distance <= minDistance)
        {
            scale = minScale;
        }
        else if (distance >= maxDistance)
        {
            scale = maxScale;
        }
        else
        {
            scale = distanceToScale * (distance - minDistance) + minScale;
        }

        transform.localScale = baseScale * scale;
    }
}
