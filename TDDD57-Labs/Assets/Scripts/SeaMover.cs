using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMover : MonoBehaviour
{
    float moveSpeed = 4.0f;

    void Update()
    {
        GameObject seaFloor = GameObject.FindWithTag("SeaFloor");
        if (seaFloor != null)
        {
            seaFloor.transform.position -= transform.forward * moveSpeed * Time.deltaTime;
        }
    }
}
