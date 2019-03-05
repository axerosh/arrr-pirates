using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropoff : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Crewman crewman = other.GetComponent<Crewman>();
        if (crewman != null)
        {
            crewman.OnDropoffReached();
        }
    }
}
