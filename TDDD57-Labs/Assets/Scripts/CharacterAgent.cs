using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAgent : MonoBehaviour
{
    NavMeshAgent navAgent;
    public Vector3 initialDestination;

    void Start()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        SetDestination(initialDestination);
    }

    public void SetDestination(Vector3 localPosition)
    {
        Vector3 worldPosition = transform.parent.TransformPoint(localPosition);
        navAgent.SetDestination(worldPosition);
    }
}
