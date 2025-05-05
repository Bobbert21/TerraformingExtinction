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
