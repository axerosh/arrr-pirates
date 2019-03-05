using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    public EntryPoint exit;
    public System.Action<Collider> onEntryFunc;

    void OnTriggerEnter(Collider other)
    {
        onEntryFunc(other);
    }
}
