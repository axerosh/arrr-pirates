using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Character chara = other.GetComponent<Character>();
        if (chara != null)
        {
            chara.OnTargetReached(this);
        }
        CharacterAgentDebug charaDebug = other.GetComponent<CharacterAgentDebug>();
        if (charaDebug != null)
        {
            charaDebug.OnTargetReached(this);
        }
    }
}
