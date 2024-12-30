using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class EditSubIdentifierWindow : EditorWindow
{
    private SubIdentifier subIdentifier;

    public static void Open(SubIdentifier subIdentifier)
    {
        EditSubIdentifierWindow window = GetWindow<EditSubIdentifierWindow>("Edit SubIdentifier");
        window.subIdentifier = subIdentifier;
        window.Show();
    }

    private void OnGUI()
    {
        if (subIdentifier == null)
        {
            EditorGUILayout.LabelField("SubIdentifier is not selected.", EditorStyles.boldLabel);
            return;
        }

        EditorGUILayout.LabelField("Edit SubIdentifier", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();

        // Text field for editing the sub-identifier name
        subIdentifier.SubIdentifierName = EditorGUILayout.TextField("SubIdentifier Name", subIdentifier.SubIdentifierName);

        // Save button to confirm changes
        if (GUILayout.Button("Save"))
        {
            // Mark the subIdentifier as dirty so changes are saved
            EditorUtility.SetDirty(subIdentifier.Parent.Tracker);
            Close();
        }

    }
}