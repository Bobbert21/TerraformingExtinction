using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IdentifierTreeCreator))]
public class IdentifierTreeCreatorEditor : Editor
{
    private IdentifierTreeCreator treeCreator;

    //when first loaded
    private void OnEnable()
    {
        //target = actual instance of the object that this is editing
        treeCreator = (IdentifierTreeCreator)target;
        if (treeCreator.RootIdentifiers == null)
        {
            treeCreator.RootIdentifiers = new List<IdentifierNode>();
        }
    }

    //custom inspector UI
    public override void OnInspectorGUI()
    {
        // Display each root node in the list
        for (int i = 0; i < treeCreator.RootIdentifiers.Count; i++)
        {
            EditorGUILayout.LabelField($"Root Node {i + 1}");
            DrawNode(treeCreator.RootIdentifiers[i], 0, i, true);
        }

        // Button to add a new root node
        if (GUILayout.Button("Add Root Node"))
        {
            treeCreator.RootIdentifiers.Add(new IdentifierNode { Identifier = EnumIdentifiers.None });
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(treeCreator);
        }
    }

    //display each node and its children
    private void DrawNode(IdentifierNode node, int indentLevel, int rootIndex, bool isRootNode)
    {
        // Draw the identifier selection dropdown
        EditorGUI.indentLevel = indentLevel;
        node.Identifier = (EnumIdentifiers)EditorGUILayout.EnumPopup("Identifier", node.Identifier);

        // Button to add a child node
        if (GUILayout.Button("Add Child Node"))
        {
            IdentifierNode childrenNode = new IdentifierNode { Identifier = EnumIdentifiers.None };
            childrenNode.Parent = node;
            node.Children.Add(childrenNode);
        }

        // Button to remove the current node (root or child)
        if (GUILayout.Button(isRootNode ? "Remove Root Node" : "Remove Child Node"))
        {
            if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to remove this node?", "Yes", "No"))
            {
                if (isRootNode)
                {
                    if (rootIndex >= 0 && rootIndex < treeCreator.RootIdentifiers.Count)
                    {
                        treeCreator.RootIdentifiers.RemoveAt(rootIndex);
                    }
                }
                else
                {
                    Debug.Log("Check node index: " + rootIndex);
                    RemoveChildNode(node);
                }
            }
        }

        // Recursively draw each child node
        for (int i = 0; i < node.Children.Count; i++)
        {
            DrawNode(node.Children[i], indentLevel + 1, rootIndex, false);
        }

        EditorGUI.indentLevel = 0; // Reset indentation level
    }

    private void RemoveChildNode(IdentifierNode removingNode)
    {
        IdentifierNode parentNode = removingNode.Parent;
        parentNode?.Children.Remove(removingNode);
    }
}
