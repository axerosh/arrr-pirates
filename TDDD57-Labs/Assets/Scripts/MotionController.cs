using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionController : MonoBehaviour
{
    float rotationSpeed = 25.0f;
    float moveSpeed = 5.0f;

    private void Start()
    {
        Input.gyro.enabled = true;
    }
    
    void Update()
    {
        Vector2 gravityXY = new Vector2(Input.gyro.gravity.x, Input.gyro.gravity.y).normalized;
        Vector2 down = new Vector2(1.0f, 0.0f);
        float steering = Vector2.Dot(gravityXY, down);
        print("Gyro gravity: " + Input.gyro.gravity);

        transform.Rotate(transform.up, steering * rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}
