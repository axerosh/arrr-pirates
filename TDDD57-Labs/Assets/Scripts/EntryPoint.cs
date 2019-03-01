using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    public System.Action<Collider> onEntryFunc;

    void OnTriggerEnter(Collider other)
    {
        onEntryFunc(other);
    }
}
