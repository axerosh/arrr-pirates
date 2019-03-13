using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaObjectSpawner : MonoBehaviour
{
    public GameObject treasurePrefab;
    public GameObject plankPrefab;
    public GameObject rockPrefab;

    private int treasureCountInner = 3;
    private int treasureCountOuter = 7;
    private int plankCount = 150;
    private int rockCount = 150;

    private float treasureRadiusInner = 30.0f;
    private float treasureRadiusOuter = 100.0f;
    private float plankRadius = 150.0f;
    private float rockRadius = 150.0f;
    void Start()
    {
        // Spawn treasures
        for (int i = treasureCountInner; i > 0; --i)
        {
            Vector2 localPosition = Random.insideUnitCircle * treasureRadiusInner;
            SpawnObject(treasurePrefab, localPosition, 0.0f);
        }
        for (int i = treasureCountOuter; i > 0; --i)
        {
            Vector2 localPosition = Random.insideUnitCircle * treasureRadiusOuter;
            SpawnObject(treasurePrefab, localPosition, 0.0f);
        }

        // Spawn planks
        for (int i = plankCount; i > 0; --i)
        {
            Vector2 localPosition = Random.insideUnitCircle * plankRadius;
            SpawnObject(plankPrefab, localPosition, 25.0f);
        }

        // Spawn rocks
        for (int i = rockCount; i > 0; --i)
        {
            Vector2 localPosition = Random.insideUnitCircle * rockRadius;
            SpawnObject(rockPrefab, localPosition, 0.0f);
        }
    }

    private void SpawnObject(GameObject prefab, Vector2 localPosition, float height)
    {
        GameObject o = Instantiate(prefab, transform);
        o.transform.localPosition = new Vector3(localPosition.x, height, localPosition.y);
        o.transform.Rotate(0.0f, Random.Range(0.0f, 360.0f), 0.0f);

        // Enter/Exit visibility hitbox
        o.GetComponent<Hideable>().SetVisible(false);
        o.GetComponent<Collider>().enabled = false;
        o.GetComponent<Collider>().enabled = true;
    }
}
