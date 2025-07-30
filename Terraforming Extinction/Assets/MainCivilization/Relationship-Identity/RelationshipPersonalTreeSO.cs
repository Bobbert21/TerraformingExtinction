using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "IdentifierPersonalTreeSO", menuName = "ScriptableObject/Civilization/IdentifierPersonalTreeSO")]
public class RelationshipPersonalTreeSO : ScriptableObject
{
    [Header("Do Not Edit")]
    public List<IdentifierNode> RootIdentifiers;
    public IdentifierMasterTreeSO MasterTree;
    public EnumIdentifiers identifier;
    public SubIdentifierNode subIdentifier;

    public RelationshipPersonalTreeSO DeepCopy()
    {
        RelationshipPersonalTreeSO newRPT = ScriptableObject.CreateInstance<RelationshipPersonalTreeSO>();

        newRPT.RootIdentifiers = RootIdentifiers != null
            ? RootIdentifiers.ConvertAll(node => node.DeepCopy(newRPT)) 
            : new List<IdentifierNode>();

        newRPT.subIdentifier = subIdentifier != null
            ? subIdentifier.DeepCopy()
            : new SubIdentifierNode();

        newRPT.identifier = identifier;

        newRPT.MasterTree = this.MasterTree;

        return newRPT;
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
