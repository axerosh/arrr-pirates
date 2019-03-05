using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeEmitter : MonoBehaviour
{
    private float emitInterval = 0.2f;
    public GameObject wakePrefab;

    private float timeUntilEmit = 0.0f;
    
    private void Update()
    {
        timeUntilEmit -= Time.deltaTime;
        if (timeUntilEmit < 0.0f)
        {
            timeUntilEmit = emitInterval;

            // Emit wake
            GameObject seaFloor = GameObject.FindWithTag("SeaFloor");
            if (seaFloor != null)
            {
                GameObject.Instantiate(wakePrefab, transform.position, transform.rotation, seaFloor.transform);
            }
        }
    }
}
