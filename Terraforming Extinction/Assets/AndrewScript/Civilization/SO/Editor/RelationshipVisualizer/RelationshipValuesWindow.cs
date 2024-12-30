using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RelationshipValuesWindow : EditorWindow
{
    private static RelationshipNode currentNode;

    public static void ShowWindow(RelationshipNode relationshipNode)
    {
        currentNode = relationshipNode;
        RelationshipValuesWindow window = GetWindow<RelationshipValuesWindow>("Edit Relationship Values");
        window.Show();
    }

    private void OnGUI()
    {
        if (currentNode == null)
        {
            EditorGUILayout.HelpBox("No RelationshipNode selected.", MessageType.Warning);
            return;
        }

        EditorGUILayout.LabelField("Edit PRValues", EditorStyles.boldLabel);

        currentNode.PRValues.DefensiveBelongingValue = EditorGUILayout.IntField("Defensive Belonging Value", currentNode.PRValues.DefensiveBelongingValue);
        currentNode.PRValues.NurtureBelongingValue = EditorGUILayout.IntField("Nurture Belonging Value", currentNode.PRValues.NurtureBelongingValue);
        currentNode.PRValues.LivelihoodValue = EditorGUILayout.IntField("Livelihood Value", currentNode.PRValues.LivelihoodValue);

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Edit ModRValues", EditorStyles.boldLabel);

        currentNode.ModRValues.DefensiveBelongingValue = EditorGUILayout.IntField("Defensive Belonging Value", currentNode.ModRValues.DefensiveBelongingValue);
        currentNode.ModRValues.NurtureBelongingValue = EditorGUILayout.IntField("Nurture Belonging Value", currentNode.ModRValues.NurtureBelongingValue);
        currentNode.ModRValues.LivelihoodValue = EditorGUILayout.IntField("Livelihood Value", currentNode.ModRValues.LivelihoodValue);

        GUILayout.Space(20);

        /*
        // Save the changes
        if (GUILayout.Button("Save Changes"))
        {
            EditorUtility.SetDirty(currentNode); // Mark the current node as dirty so that Unity saves the changes
            Close(); // Close the window after saving
        }
        */
    }
}
