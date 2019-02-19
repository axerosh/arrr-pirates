using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAgent : MonoBehaviour
{
    NavMeshAgent navAgent;

    void Start()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 localPosition)
    {
        Vector3 worldPosition = transform.parent.TransformPoint(localPosition);
        navAgent.SetDestination(worldPosition);
    }
}
