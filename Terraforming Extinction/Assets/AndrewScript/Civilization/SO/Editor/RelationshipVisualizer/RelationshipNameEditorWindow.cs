using UnityEditor;
using UnityEngine;

public class RelationshipNameEditorWindow : EditorWindow
{
    private RelationshipNode relationshipNode;
    private string newName;

    public static void ShowWindow(RelationshipNode relationshipNode)
    {
        RelationshipNameEditorWindow window = GetWindow<RelationshipNameEditorWindow>("Edit Relationship Name");
        window.relationshipNode = relationshipNode;
        window.newName = relationshipNode.Name; // Set the current name as the default
    }

    private void OnGUI()
    {
        GUILayout.Label("Edit Relationship Name", EditorStyles.boldLabel);
        newName = EditorGUILayout.TextField("New Name:", newName);

        if (GUILayout.Button("Save"))
        {
            if (relationshipNode != null)
            {
                relationshipNode.Name = newName; // Update the name
                //EditorUtility.SetDirty(relationshipNode); // Mark the node as dirty to save changes
                Close(); // Close the window
            }
        }

        if (GUILayout.Button("Cancel"))
        {
            Close(); // Close the window without saving
        }
    }
}
