using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Crewman crewman = other.GetComponent<Crewman>();
        if (crewman != null)
        {
            crewman.OnTargetReached(this);
        }
        CharacterAgentDebug charaDebug = other.GetComponent<CharacterAgentDebug>();
        if (charaDebug != null)
        {
            charaDebug.OnTargetReached(this);
        }
    }
}
