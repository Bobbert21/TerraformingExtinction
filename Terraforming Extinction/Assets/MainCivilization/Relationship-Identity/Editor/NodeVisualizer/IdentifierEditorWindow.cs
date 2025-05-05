using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// Create an EditorWindow to edit the Identifier
public class IdentifierEditorWindow : EditorWindow
{
    private IdentifierNode node;

    // Function to show the window
    public static void ShowWindow(IdentifierNode node)
    {
        IdentifierEditorWindow window = GetWindow<IdentifierEditorWindow>("Edit Identifier");
        window.node = node;
        window.Show();
    }

    // Function to draw the GUI in the window
    private void OnGUI()
    {
        if (node == null) return;

        // Display an Enum popup to select the identifier
        node.Identifier = (EnumIdentifiers)EditorGUILayout.EnumPopup("Select Identifier", node.Identifier);

        // If the object is modified, mark it as dirty to allow saving changes
        if (GUI.changed && node.Tracker != null)
        {
            EditorUtility.SetDirty(node.Tracker); // Mark ScriptableObject as dirty to save the changes
        }

        // Add a button to close the window
        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
    }
}
