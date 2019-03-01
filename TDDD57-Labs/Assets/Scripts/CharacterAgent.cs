using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAgent : MonoBehaviour
{

    public void SetDestination(Vector3 localPosition)
    {
        Vector3 worldPosition = transform.parent.TransformPoint(localPosition);
        gameObject.GetComponent<NavMeshAgent>().SetDestination(worldPosition);
    }
}
