using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "IdentifierSO", menuName = "ScriptableObject/Civilization/IdentifierSO")]
public class IdentifierSO : ScriptableObject
{
    [Header("Do Not Edit")]
    public List<IdentifierNode> RootIdentifiers;
    public IdentifierTreeCreator TreeCreator;
    public List<EnumIdentifiers> identifiers;
    //initialize the tree
    [ContextMenu("Initialize Identifier Tree")]
    public void InitializeIdentifierTree()
    {
        if(TreeCreator != null)
        {
            RootIdentifiers.Clear();
            RootIdentifiers.AddRange(TreeCreator.RootIdentifiers);
            Debug.Log("Identifier Tree Initialized");

            // Mark the ScriptableObject as dirty (flag) to make unity update
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
            Debug.Log("Identifier Tree Initialized");
        }
        else
        {
            Debug.LogWarning("TreeCreator is not assigned!");
        }

        
    }
}
