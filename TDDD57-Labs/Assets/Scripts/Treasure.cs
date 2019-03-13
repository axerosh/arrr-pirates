using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Treasure : MonoBehaviour {
    [SerializeField]
    private TextMeshPro selectionIndicator = null;

    private bool selected = false;

    void Update() {
        if (selected) {
            UpdateSelectionMarker();
        }
    }

    void OnTriggerEnter(Collider other) {
        Crewman crewman = other.GetComponent<Crewman>();
        if (crewman != null)
        {
            crewman.OnTreasureReached(this);
        }
    }

    public void ToggleSelected(bool state) {
        selected = state;
        selectionIndicator.gameObject.SetActive(state);
    }

    void UpdateSelectionMarker() {
        selectionIndicator.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
