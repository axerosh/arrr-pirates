using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaFloorVisible : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Hideable hideable = other.GetComponent<Hideable>();
        if (hideable != null)
        {
            hideable.SetVisible(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Hideable hideable = other.GetComponent<Hideable>();
        if (hideable != null)
        {
            hideable.SetVisible(false);
        }
    }
}
