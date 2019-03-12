using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Helmsman : MonoBehaviour
{
    public TextMeshPro hintText;

    private bool helmsmanHintRead = false;

    // Start is called before the first frame update
    void Start()
    {
        Selectable selectable = GetComponent<Selectable>();
        selectable.onSelected = Select;
        selectable.onDeselected = Deselect;
    }

    private void Update() {
        UpdateText();
    }

    void UpdateText() {
        if (!helmsmanHintRead) {
            hintText.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }

    private void Select()
    {
        if (!helmsmanHintRead) {
            helmsmanHintRead = true;
            hintText.gameObject.SetActive(false);
            GameObject.FindWithTag("UI").GetComponentInChildren<GamePlayUI>().DisplayHelmsmanHint();
        }
        SetControlEnabled(true);
    }

    private void Deselect()
    {
        SetControlEnabled(false);
    }

    private void SetControlEnabled(bool flag)
    {
        GameObject ship = GameObject.FindWithTag("Ship");
        if (ship != null)
        {
            SeaMover movement = ship.GetComponent<SeaMover>();
            if (movement != null)
            {
                movement.enabled = flag;
            }
            MotionController steering = ship.GetComponent<MotionController>();
            if (steering != null)
            {
                steering.enabled = flag;
            }
            WakeEmitter wakes = ship.GetComponentInChildren<WakeEmitter>();
            if (wakes != null)
            {
                wakes.enabled = flag;
            }
        }
    }
}
