using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "IdentifierTrackerSO", menuName = "ScriptableObject/Civilization/IdentifierTrackerSO")]
public class IdentifierTrackerSO : ScriptableObject
{
    [Header("Do Not Edit")]
    public List<IdentifierNode> RootIdentifiers;
    public IdentifierTreeCreator TreeCreator;

    [ContextMenu("Initialize Identifier Tree")]
    public void InitializeIdentifierTree()
    {
        if (TreeCreator != null)
        {
            Debug.Log("TreeCreator is assigned. Initializing tree...");
            RootIdentifiers.Clear();
            RootIdentifiers.AddRange(TreeCreator.RootIdentifiers);
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
