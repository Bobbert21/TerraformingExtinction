using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AddSubIdentifierWindow : EditorWindow
{
    private IdentifierNode targetNode;
    private string subIdentifierName;

    public static void Open(IdentifierNode node)
    {
        AddSubIdentifierWindow window = GetWindow<AddSubIdentifierWindow>("Add SubIdentifier");
        window.targetNode = node;
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Add SubIdentifier", EditorStyles.boldLabel);

        // Field to enter the SubIdentifier name
        subIdentifierName = EditorGUILayout.TextField("SubIdentifier Name", subIdentifierName);

        if (GUILayout.Button("Add"))
        {
            if (!string.IsNullOrEmpty(subIdentifierName))
            {
                // Create and add the new SubIdentifier
                SubIdentifierNode newSubIdentifier = new SubIdentifierNode(targetNode) // Pass the parent node
                {
                    SubIdentifierName = subIdentifierName,
                    // Parent is already set in the constructor
                };

                // Ensure targetNode has SubIdentifiers initialized
                if (targetNode.SubIdentifiers == null)
                {
                    targetNode.SubIdentifiers = new List<SubIdentifierNode>();
                }

                targetNode.SubIdentifiers.Add(newSubIdentifier);

                // Mark the containing ScriptableObject as dirty
                if (targetNode.Tracker != null) // Use the Tracker reference
                {
                    EditorUtility.SetDirty(targetNode.Tracker);
                }

                // Close the window after adding
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "SubIdentifier name cannot be empty.", "OK");
            }
        }


        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
    }
}
