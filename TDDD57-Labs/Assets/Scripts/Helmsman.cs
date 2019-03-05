using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmsman : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Selectable selectable = GetComponent<Selectable>();
        selectable.onSelected = Select;
        selectable.onDeselected = Deselect;
    }

    private void Select()
    {
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
