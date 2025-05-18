using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "IdentifierPersonalTreeSO", menuName = "ScriptableObject/Civilization/IdentifierPersonalTreeSO")]
public class RelationshipPersonalTreeSO : ScriptableObject
{
    [Header("Do Not Edit")]
    public List<IdentifierNode> RootIdentifiers;
    public IdentifierMasterTreeSO MasterTree;
    public List<EnumIdentifiers> identifiers;
    public List<SubIdentifierNode> subIdentifiers;

    public RelationshipPersonalTreeSO DeepCopy()
    {
        RelationshipPersonalTreeSO newSO = ScriptableObject.CreateInstance<RelationshipPersonalTreeSO>();

        newSO.RootIdentifiers = RootIdentifiers != null
            ? RootIdentifiers.ConvertAll(node => node.DeepCopy()) 
            : new List<IdentifierNode>();

        newSO.subIdentifiers = subIdentifiers != null
            ? subIdentifiers.ConvertAll(s => s.DeepCopy())
            : new List<SubIdentifierNode>();

        newSO.identifiers = identifiers != null
            ? new List<EnumIdentifiers>(identifiers)
            : new List<EnumIdentifiers>();

        newSO.MasterTree = this.MasterTree;

        return newSO;
    }

    [ContextMenu("Initialize Identifier Tree")]
    public void InitializeIdentifierTree()
    {
        if (MasterTree != null)
        {
            Debug.Log("TreeCreator is assigned. Initializing tree...");
            RootIdentifiers.Clear();
            RootIdentifiers.AddRange(MasterTree.RootIdentifiers);
            Debug.Log("Identifier Tree Initialized");

        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
        }
        else
        {
            Debug.LogWarning("TreeCreator is not assigned!");
        }
    }
}
