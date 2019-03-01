using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaFloorVisible : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Renderer renderer = other.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Renderer renderer = other.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.enabled = false;
        }
    }
}
