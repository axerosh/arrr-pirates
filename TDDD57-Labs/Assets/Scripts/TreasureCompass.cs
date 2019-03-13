using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreasureCompass : MonoBehaviour
{
    public GameObject needle;
    public TextMeshPro hintText;

    private float spinSpeed = 80.0f;

    void UpdateText()
    {
        hintText.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();

        // Find closest treasure
        var treasures = GameObject.FindObjectsOfType<Treasure>();
        Transform closestTreasure = null;
        float closestDistance = float.MaxValue;
        foreach (Treasure treasure in treasures)
        {
            float distance = Vector3.Distance(transform.position, treasure.transform.position);
            if (distance < closestDistance)
            {
                closestTreasure = treasure.transform;
                closestDistance = distance;
            }
        }

        // Turn needle
        if (closestTreasure != null)
        {
            var toTreasure = closestTreasure.position - needle.transform.position;
            if (toTreasure.x != 0.0f || toTreasure.z != 0.0f)
            {
                needle.transform.right = Vector3.Scale(toTreasure, new Vector3(1.0f, 0.0f, 1.0f));
            }
        }
        else
        {
            // Spin
            needle.transform.right = Quaternion.AngleAxis(Time.deltaTime * spinSpeed, Vector3.up) * needle.transform.right;
        }
    }
}
