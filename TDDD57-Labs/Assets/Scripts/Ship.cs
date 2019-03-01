using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    float moveSpeed = 5.0f;

    void Update()
    {
        GameObject seaFloor = GameObject.FindWithTag("SeaFloor");
        if (seaFloor != null)
        {
            seaFloor.transform.position -= transform.forward * moveSpeed * Time.deltaTime;
        }
    }
}
