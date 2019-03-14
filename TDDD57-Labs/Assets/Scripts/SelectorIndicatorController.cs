using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectorIndicatorController : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro selectionIndicator = null;

    private bool selected = false;

    void Update()
    {
        if (selected)
        {
            UpdateSelectionMarker();
        }
    }

    public void SetSelected(bool state)
    {
        selected = state;
        selectionIndicator.gameObject.SetActive(state);
    }

    void UpdateSelectionMarker()
    {
        selectionIndicator.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
