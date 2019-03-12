using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAgent : MonoBehaviour {
    
    private Vector3 toFeet = new Vector3(0.0f, -1.0f, 0.0f);

    private bool isRoaming = false;

    public void SetDestination(Vector3 localPosition) {
        Vector3 worldPosition = transform.parent.TransformPoint(localPosition);
        var navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(worldPosition);
        navAgent.speed = 3.5f;
        isRoaming = false;
    }

    public void SetRoaming()
    {
        SetRandomDestination();
        GetComponent<NavMeshAgent>().speed = 2.0f;
        isRoaming = true;
    }

    private void Update()
    {
        if (isRoaming)
        {
            var navAgent = GetComponent<NavMeshAgent>();
            if (Vector3.Distance(navAgent.destination, transform.position + toFeet) < 0.1f)
            {
                SetRandomDestination();
            }
        }
    }

    private void SetRandomDestination()
    {
        const float maxRadius = 10.0f;

        // Random position
        Vector2 randPos2d = Random.insideUnitCircle * Random.value * maxRadius;
        Vector3 randPos = new Vector3(randPos2d.x, 0.0f, randPos2d.y);

        // Snap to navmesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randPos, out hit, maxRadius, NavMesh.AllAreas))
        {
            gameObject.GetComponent<NavMeshAgent>().SetDestination(hit.position);
            Debug.Log("Set random destination at " + hit.position);
        }

    }
}

