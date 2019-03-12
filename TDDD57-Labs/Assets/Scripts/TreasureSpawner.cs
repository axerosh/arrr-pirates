using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureSpawner : MonoBehaviour
{
    public GameObject treasurePrefab;

    private int treasureCount = 10;
    
    void Start()
    {
        for (int i = treasureCount; i > 0; --i) {
            GameObject treasure = Instantiate(treasurePrefab, transform);
            Vector2 p = Random.insideUnitCircle * 100.0f;
            treasure.transform.localPosition = new Vector3(p.x, 0.0f, p.y);

            // Enter/Exit visibility hitbox
            treasure.GetComponent<Hideable>().SetVisible(false);
            treasure.GetComponent<Collider>().enabled = false;
            treasure.GetComponent<Collider>().enabled = true;
        }
    }
}
