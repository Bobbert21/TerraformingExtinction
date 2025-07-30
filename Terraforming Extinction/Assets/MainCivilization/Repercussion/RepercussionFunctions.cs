using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public static class RepercussionFunctions
{
    //Add Relationship Values from Repercussion
    //Get subidentity, then get the change, and add the relationship change based on LR


    public static void ImplementRepercussion(
         SubIdentifierNode sourceNode,
         EnumActionCharacteristics actionContext,
         float lValue = 0,
         float dbValue = 0,
         float nbValue = 0,
         //0 - 1
         float learningRate = 0.01f)
    {
        float lDelta = lValue * learningRate;
        float dbDelta = dbValue * learningRate;
        float nbDelta = nbValue * learningRate;
        Debug.Log("Changes in relationship: L value " + lDelta + " DB Value " + dbDelta + " NB Value " + nbDelta);
        List<RelationshipNode> relationships = sourceNode.RelationshipNodes;

        RelationshipNode mainNode = null;
        RelationshipNode actionNode = null;

        // Single pass: identify nodes that are main nodes (no actions) or the action node of interest
        foreach (RelationshipNode node in relationships)
        {
            if (node.ActionContext == EnumActionCharacteristics.Main)
                mainNode = node;
            //The else here prevents the actionNode to also be the main node (which would duplicate the values)
            else if (node.ActionContext == actionContext)
                actionNode = node;
        }

        //Update or create node with the actioncontext
        if (actionNode != null)
        {
            //action node found
            Debug.Log("Action node found " + actionNode.Name);
            actionNode.ModRValues.LivelihoodValue += lDelta;
            actionNode.ModRValues.DefensiveBelongingValue += dbDelta;
            actionNode.ModRValues.NurtureBelongingValue += nbDelta;
        }
        //Make sure that if no nodes are found, doesn't create new node same as main node
        else if(actionContext != EnumActionCharacteristics.Main) 
        {
            Debug.Log("No action node found and creating new one");
            relationships.Add(new RelationshipNode(
                actionContext.ToString(), new RelationshipValues(0,0,0),
                new RelationshipValues(lDelta, dbDelta, nbDelta),
                actionContext, null));
        }

        //Update or create Main (None) node
        if (mainNode != null)
        {
            Debug.Log("Main node found with name " + mainNode.Name);
            mainNode.ModRValues.LivelihoodValue += lDelta;
            mainNode.ModRValues.DefensiveBelongingValue += dbDelta;
            mainNode.ModRValues.NurtureBelongingValue += nbDelta;
        }
        else
        {
            Debug.Log("No main node found and creating new one");
            relationships.Add(
                new RelationshipNode(
                    "Main Node",
                    //PRValues
                    new RelationshipValues(0,0,0),
                    //ModRValues
                    new RelationshipValues(lDelta, dbDelta, nbDelta),
                    EnumActionCharacteristics.Main, 
                    null
                    )
                );
        }
        
    }

}
