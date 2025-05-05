using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DELETE
//How the relatinship tree develops, using the AdaptiveIdentifierFunctions
//Relationship Tree uses these functions to develop
public static class AdaptiveRelationshipTreeDevelopment
{
    //From identifier script. It does everything when perfect match (or above likeness score for same)
    public static void SubIdentifierVersionSameToHeuristic(SubIdentifierNode subIdentifierNode, int value = 1)
    {
        AdaptiveIdentifierFunctions.AddValueToAllCharacteristicsInSubidentity(subIdentifierNode, value);
    }
}
