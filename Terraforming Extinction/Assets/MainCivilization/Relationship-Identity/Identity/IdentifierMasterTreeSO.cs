using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "IdentifierMasterTree", menuName = "ScriptableObject/Civilization/IdentifierMasterTree")]
public class IdentifierMasterTreeSO : ScriptableObject
{
    public List<IdentifierNode> RootIdentifiers = new List<IdentifierNode>();
}
