using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropoff : MonoBehaviour
{
    public GameObject goldPile;

    void OnTriggerEnter(Collider other)
    {
        Crewman crewman = other.GetComponent<Crewman>();
        if (crewman != null)
        {
            crewman.OnDropoffReached();
        }

        float winProgress = GameObject.FindObjectOfType<ArrrController>().WinProgress();
        if (winProgress > 0.0f)
        {
            goldPile.transform.localScale = new Vector3(1.0f, 2.0f * winProgress, 1.0f);
        }
    }
}
