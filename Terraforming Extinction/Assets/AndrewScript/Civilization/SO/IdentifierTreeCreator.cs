using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "IdentifierTreeCreator", menuName = "ScriptableObject/Civilization/IdentifierTreeCreator")]
public class IdentifierTreeCreator : ScriptableObject
{
    public List<IdentifierNode> RootIdentifiers = new List<IdentifierNode>();
}
