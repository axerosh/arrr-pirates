using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public Material selectedMaterial;
    public Material deselectedMaterial;

    public System.Action onSelected = null;
    public System.Action onDeselected = null;
    public System.Action<GameObject> onTargetSet = null;

    public void Select()
    {
        GetComponent<Renderer>().sharedMaterial = selectedMaterial;
        onSelected?.Invoke();
    }

    public void Deselect()
    {
        GetComponent<Renderer>().sharedMaterial = deselectedMaterial;
        onDeselected?.Invoke();
    }

    /**
     * Return true if setting a target was supported, else returns false.
     */
    public bool SetTarget(GameObject target)
    {
        if (onTargetSet != null)
        {
            onTargetSet(target);
            return true;
        }
        else
        {
            return false;
        }
    }
}
