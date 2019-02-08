using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAgentDebug : MonoBehaviour
{
    public List<Target> targets = new List<Target>();
    int targetIndex = -1;
    NavMeshAgent navAgent;
    bool hasFoundPath = false;
    
    
    void Start()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        ChangeTarget();
    }

    public void OnTargetReached(Target target)
    {
        if (targets.Count > targetIndex && target == targets[targetIndex])
        {
            ChangeTarget();
        }
    }

    void ChangeTarget()
    {
        if (targets.Count > 0)
        {
            ++targetIndex;
            if (targetIndex >= targets.Count)
            {
                targetIndex = 0;
            }

            Vector3 targetPosition = targets[targetIndex].transform.localPosition;
            SetDestination(targetPosition);
        }
    }

    public void SetDestination(Vector3 localPosition)
    {
        Vector3 worldPosition = transform.parent.TransformPoint(localPosition);
        navAgent.SetDestination(worldPosition);
    }
}
