using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipTreeDebugSetter : MonoBehaviour
{
    public bool isAgentTree = true;
    private RelationshipPersonalTree relationshipPersonalTree = null;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(relationshipPersonalTree != GetComponent<CharacterMainCPort>().characterPsyche.RelationshipPersonalTree)
        {
            relationshipPersonalTree = GetComponent<CharacterMainCPort>().characterPsyche.RelationshipPersonalTree;
            if (isAgentTree)
            {
                RelationshipTreeDebugUI.Instance.agentTree = relationshipPersonalTree;
            }
            else
            {
                RelationshipTreeDebugUI.Instance.environmentTree = relationshipPersonalTree;
            }
        }
    }
}
