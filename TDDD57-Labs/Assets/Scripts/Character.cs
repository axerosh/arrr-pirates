using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public List<Target> targets = new List<Target>();
    public GameObject characterAgentPrefab;
    CharacterAgent charAgent;
    int targetIndex = 0;

    public Material selectedMaterial;
    public Material deselectedMaterial;

    void Start()
    {
        GameObject agent = Instantiate(characterAgentPrefab, transform.localPosition, transform.rotation);
        agent.transform.parent = GameObject.FindWithTag("ShipHitbox").transform;
        charAgent = agent.GetComponent<CharacterAgent>();
    }

    private void Update()
    {
        transform.localPosition = charAgent.transform.localPosition;
    }

    public void OnTargetReached(Target target)
    {
        // TODO
    }

    private Color highlightOffset = new Color(0.0f, 0.5f, 0.5f);

    public void Select()
    {
        GetComponent<Renderer>().sharedMaterial = selectedMaterial;
    }

    public void Deselect()
    {
        GetComponent<Renderer>().sharedMaterial = deselectedMaterial;
    }

    public void SetTarget(Target target)
    {
        Vector3 targetPosition = target.transform.localPosition;
        charAgent.SetDestination(targetPosition);
    }
}
