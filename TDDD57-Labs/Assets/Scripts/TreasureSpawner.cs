using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureSpawner : MonoBehaviour
{
    public GameObject treasurePrefab;

    private int treasureCountInner = 3;
    private int treasureCountOuter = 7;

    private float innerRadius = 30.0f;
    private float outerRadius = 100.0f;

    void Start()
    {
        for (int i = treasureCountInner; i > 0; --i)
        {
            Vector2 localPosition = Random.insideUnitCircle * innerRadius;
            SpawnTreasure(localPosition);
        }
        for (int i = treasureCountOuter; i > 0; --i)
        {
            Vector2 localPosition = Random.insideUnitCircle * outerRadius;
            SpawnTreasure(localPosition);
        }
    }

    private void SpawnTreasure(Vector2 localPosition)
    {
        GameObject treasure = Instantiate(treasurePrefab, transform);
        treasure.transform.localPosition = new Vector3(localPosition.x, 0.0f, localPosition.y);

        // Enter/Exit visibility hitbox
        treasure.GetComponent<Hideable>().SetVisible(false);
        treasure.GetComponent<Collider>().enabled = false;
        treasure.GetComponent<Collider>().enabled = true;
    }
}
