using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public List<Target> targets = new List<Target>();
    public GameObject characterAgentPrefab;
    CharacterAgent charAgent;
    int targetIndex = 0;

    void Start()
    {
        GameObject agent = Instantiate(characterAgentPrefab, transform.localPosition, transform.rotation);
        agent.transform.parent = GameObject.FindWithTag("ShipHitbox").transform;
        charAgent = agent.GetComponent<CharacterAgent>();

        if (targets.Count > targetIndex)
        {
            Vector3 targetPosition = targets[targetIndex].transform.localPosition;
            charAgent.initialDestination = targetPosition;
        }
    }

    private void Update()
    {
        transform.localPosition = charAgent.transform.localPosition;
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
            charAgent.SetDestination(targetPosition);
        }
    }
}
