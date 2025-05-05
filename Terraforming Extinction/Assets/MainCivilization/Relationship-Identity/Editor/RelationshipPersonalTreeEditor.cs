using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(RelationshipPersonalTreeSO))]
public class RelationshipPersonalTreeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RelationshipPersonalTreeSO identifierSO = (RelationshipPersonalTreeSO)target;

        // Draw default inspector elements first
        DrawDefaultInspector();

        GUILayout.Label("Can Edit Below", EditorStyles.boldLabel);

        // Draw a button to initialize the Identifier Tree
        if (GUILayout.Button("Initialize Identifier Tree"))
        {
            identifierSO.InitializeIdentifierTree();
        }

        GUILayout.Space(10);

        // Display the Identifier Tree with clickable nodes
        GUILayout.Label("Identifier Tree", EditorStyles.boldLabel);
        if (identifierSO.RootIdentifiers != null && identifierSO.RootIdentifiers.Count > 0)
        {
            foreach (var rootNode in identifierSO.RootIdentifiers)
            {
                DrawNode(rootNode, identifierSO);
            }
        }
    }

    private void DrawNode(IdentifierNode node, RelationshipPersonalTreeSO identifierSO, int indentLevel = 0)
    {
        Color defaultColor = GUI.backgroundColor;

        // Highlight identifier in green if selected
        if (identifierSO.identifiers.Contains(node.Identifier))
        {
            GUI.backgroundColor = Color.green;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(indentLevel * 20);  // Indent based on hierarchy level

        if (GUILayout.Button(indentLevel.ToString() + ": " + node.Identifier.ToString(), GUILayout.Width(150)))
        {
            // Toggle selection for identifier
            if (!identifierSO.identifiers.Contains(node.Identifier))
            {
                identifierSO.identifiers.Add(node.Identifier);
            }
            else
            {
                identifierSO.identifiers.Remove(node.Identifier);
            }

            // Mark as dirty to update Unity
            EditorUtility.SetDirty(identifierSO);
        }
        GUILayout.EndHorizontal();

        // Reset color
        GUI.backgroundColor = defaultColor;

        // Draw SubIdentifiers as buttons
        if (node.SubIdentifiers != null && node.SubIdentifiers.Count > 0)
        {
            foreach (var subIdentifier in node.SubIdentifiers)
            {
                // Highlight subidentifier in green if selected
                if (identifierSO.subIdentifiers.Contains(subIdentifier))
                {
                    GUI.backgroundColor = Color.green;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space((indentLevel + 1) * 20); // Indent slightly more than the parent node

                if (GUILayout.Button("Sub: " + subIdentifier.SubIdentifierName, GUILayout.Width(150)))
                {
                    // Toggle selection for subIdentifier
                    if (!identifierSO.subIdentifiers.Contains(subIdentifier))
                    {
                        identifierSO.subIdentifiers.Add(subIdentifier);
                    }
                    else
                    {
                        identifierSO.subIdentifiers.Remove(subIdentifier);
                    }

                    // Mark as dirty to update Unity
                    EditorUtility.SetDirty(identifierSO);
                }
                GUILayout.EndHorizontal();

                // Reset color
                GUI.backgroundColor = defaultColor;
            }
        }

        // Draw children nodes recursively
        if (node.Children != null && node.Children.Count > 0)
        {
            foreach (var childNode in node.Children)
            {
                DrawNode(childNode, identifierSO, indentLevel + 1);
            }
        }
    }

}
