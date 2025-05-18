using UnityEditor;
using UnityEngine;

public class EditCharacteristicsWindow : EditorWindow
{
    private SubIdentifierNode subIdentifier;

    public static void Open(SubIdentifierNode subIdentifier)
    {
        EditCharacteristicsWindow window = GetWindow<EditCharacteristicsWindow>("Edit Characteristics");
        window.subIdentifier = subIdentifier;  // Ensure this is not null
        window.Show();
    }

    private void OnGUI()
    {
        if (subIdentifier == null)
        {
            EditorGUILayout.LabelField("No SubIdentifier selected.");
            return;
        }

        // Display Action Characteristics
        if (subIdentifier.ActionCharacteristicsWithValue != null && subIdentifier.ActionCharacteristicsWithValue.Count > 0)
        {
            EditorGUILayout.LabelField($"Editing Action Characteristics for: {subIdentifier.SubIdentifierName}");

            for (int i = 0; i < subIdentifier.ActionCharacteristicsWithValue.Count; i++)
            {
                var characteristic = subIdentifier.ActionCharacteristicsWithValue[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(characteristic.CharacteristicType.ToString(), GUILayout.Width(100));

                // Ensure Value is accessed correctly
                if (characteristic != null)
                {
                    characteristic.Value = EditorGUILayout.FloatField(characteristic.Value);
                }
                else
                {
                    EditorGUILayout.LabelField("Characteristic is null.");
                }

                // Delete button for removing the characteristic
                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    subIdentifier.ActionCharacteristicsWithValue.RemoveAt(i);
                    i--; // Adjust index since we've removed an item
                    Debug.Log($"Deleted action characteristic at index {i + 1}");
                }

                EditorGUILayout.EndHorizontal(); // Ensure this matches
            }
        }
        else
        {
            EditorGUILayout.LabelField("No Action characteristics available.");
        }

        // Display Appearance Characteristics
        if (subIdentifier.AppearanceCharacteristicsWithValue != null && subIdentifier.AppearanceCharacteristicsWithValue.Count > 0)
        {
            EditorGUILayout.LabelField($"Editing Appearance Characteristics for: {subIdentifier.SubIdentifierName}");

            for (int i = 0; i < subIdentifier.AppearanceCharacteristicsWithValue.Count; i++)
            {
                var characteristic = subIdentifier.AppearanceCharacteristicsWithValue[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(characteristic.CharacteristicType.ToString(), GUILayout.Width(100));

                // Ensure Value is accessed correctly
                if (characteristic != null)
                {
                    characteristic.Value = EditorGUILayout.FloatField(characteristic.Value);
                }
                else
                {
                    EditorGUILayout.LabelField("Characteristic is null.");
                }

                // Delete button for removing the characteristic
                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    subIdentifier.AppearanceCharacteristicsWithValue.RemoveAt(i);
                    i--; // Adjust index since we've removed an item
                    Debug.Log($"Deleted appearance characteristic at index {i + 1}");
                }

                EditorGUILayout.EndHorizontal(); // Ensure this matches
            }
        }
        else
        {
            EditorGUILayout.LabelField("No Appearance characteristics available.");
        }

        if (GUILayout.Button("Save Changes"))
        {
            //EditorUtility.SetDirty(FindParentUnityObject(subIdentifier));
            Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
    }

    private UnityEngine.Object FindParentUnityObject(SubIdentifierNode subIdentifier)
    {
        IdentifierNode currentParent = subIdentifier.Parent;

        while (currentParent != null)
        {
            if (currentParent.Tracker != null)
            {
                return currentParent.Tracker;
            }

            currentParent = currentParent.Parent;
        }

        return null;
    }
}
