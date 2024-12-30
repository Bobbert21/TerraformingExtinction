using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SubIdentifierNodeGraphWindow : EditorWindow
{
    private IdentifierTrackerSO trackerSO;
    private int selectedRootIndex = 0; // Index of the selected root identifier
    private string[] rootIdentifiersNames; // Array to hold root identifier names for the dropdown

    private Vector2 windowSize; // To store the current window size

    [MenuItem("Window/SubIdentifier Node Graph")]
    public static void OpenWindow()
    {
        SubIdentifierNodeGraphWindow window = GetWindow<SubIdentifierNodeGraphWindow>("Identifier Node Graph");
        window.Show();
    }

    private void OnGUI()
    {
        // Get the current size of the window
        windowSize = position.size;

        // Display a field to assign the trackerSO ScriptableObject
        trackerSO = (IdentifierTrackerSO)EditorGUILayout.ObjectField("Identifier Tracker", trackerSO, typeof(IdentifierTrackerSO), false);

        if (trackerSO == null)
        {
            EditorGUILayout.HelpBox("Please assign an IdentifierTrackerSO to visualize the tree.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Initialize Tree"))
        {
            trackerSO.InitializeIdentifierTree();
        }

        // Update the dropdown options if there are any root identifiers
        if (trackerSO.RootIdentifiers != null && trackerSO.RootIdentifiers.Count > 0)
        {
            UpdateDropdownOptions();

            // Display dropdown for selecting root identifier
            selectedRootIndex = EditorGUILayout.Popup("Select Root Identifier", selectedRootIndex, rootIdentifiersNames);

            // Add some vertical space to lower the tree below the dropdown
            GUILayout.Space(20);

            // Draw the selected root node and its children
            DrawNodeGraph(trackerSO.RootIdentifiers[selectedRootIndex]);
        }
        else
        {
            EditorGUILayout.HelpBox("Tree is not initialized or is empty.", MessageType.Info);
        }
    }

    // Updates the names for the dropdown based on the root identifiers
    private void UpdateDropdownOptions()
    {
        if (trackerSO.RootIdentifiers == null || trackerSO.RootIdentifiers.Count == 0)
            return;

        rootIdentifiersNames = new string[trackerSO.RootIdentifiers.Count];
        for (int i = 0; i < trackerSO.RootIdentifiers.Count; i++)
        {
            rootIdentifiersNames[i] = trackerSO.RootIdentifiers[i].Identifier.ToString();
        }
    }

    // Draws the node graph for the selected root node
    private void DrawNodeGraph(IdentifierNode rootNode)
    {
        // Calculate the starting position based on the window size
        Vector2 startPos = new Vector2(windowSize.x / 2 - 75, 100); // Center horizontally and move the graph down

        float xSpacing = 200f; // Horizontal spacing between nodes
        float ySpacing = 150f; // Vertical spacing between parent and child nodes

        // Draw the root node and recursively draw its children and sub-identifiers
        CreateNodeView(rootNode, null, startPos, xSpacing, ySpacing);
    }

    private void CreateNodeView(IdentifierNode node, IdentifierNode parent, Vector2 position, float xSpacing, float ySpacing)
    {
        // Draw the current node (IdentifierNode) at the given position
        GUI.Box(new Rect(position.x, position.y, 150, 60), node.Identifier.ToString());

        // Button to add a new SubIdentifier
        if (GUI.Button(new Rect(position.x + 160, position.y + 10, 100, 20), "Add SubIdentifier"))
        {
            ShowAddSubIdentifierWindow(node);
        }

        // Position for the "Edit Identifier" button below the "Add SubIdentifier" button
        float editIdentifierButtonYPosition = position.y + 30; // Adjust this to position it below the "Add SubIdentifier" button
        if (GUI.Button(new Rect(position.x + 160, editIdentifierButtonYPosition, 140, 20), "Edit Identifier"))
        {
            // Show a new window to edit the identifier (passing the current node instead of parent)
            IdentifierEditorWindow.ShowWindow(node);
        }

        // Draw SubIdentifiers directly below the IdentifierNode
        float subIdentifierOffsetY = ySpacing * 0.5f; // Offset for sub-identifiers
        if (node.SubIdentifiers != null && node.SubIdentifiers.Count > 0)
        {
            float subIdentifierStartX = position.x - (node.SubIdentifiers.Count - 1) * xSpacing / 2; // Center sub-identifiers

            for (int i = 0; i < node.SubIdentifiers.Count; i++)
            {
                Vector2 subIdentifierPos = new Vector2(subIdentifierStartX + i * xSpacing, position.y + subIdentifierOffsetY);
                CreateSubIdentifierView(node.SubIdentifiers[i], node, subIdentifierPos, xSpacing, ySpacing);

                // Draw line connecting the parent node to the sub-identifier
                Handles.BeginGUI();
                Handles.DrawLine(new Vector3(position.x + 75, position.y + 60, 0), new Vector3(subIdentifierPos.x + 75, subIdentifierPos.y, 0));
                Handles.EndGUI();
            }
        }

        // Draw Children nodes below the SubIdentifiers
        if (node.Children != null && node.Children.Count > 0)
        {
            float childStartX = position.x - (node.Children.Count - 1) * xSpacing / 2; // Center children under parent
            float childOffsetY = ySpacing; // Space for children

            for (int i = 0; i < node.Children.Count; i++)
            {
                Vector2 childPos = new Vector2(childStartX + i * xSpacing, position.y + subIdentifierOffsetY + ySpacing);
                CreateNodeView(node.Children[i], node, childPos, xSpacing, ySpacing);

                // Draw line connecting the parent to the child node
                Handles.BeginGUI();
                Handles.DrawLine(new Vector3(position.x + 75, position.y + 60, 0), new Vector3(childPos.x + 75, childPos.y, 0));
                Handles.EndGUI();
            }
        }
    }



    private void ShowAddCharacteristicWindow(SubIdentifier subIdentifier)
    {
        AddCharacteristicWindow.Open(subIdentifier);
    }




    private void CreateSubIdentifierView(SubIdentifier subIdentifier, IdentifierNode parent, Vector2 position, float xSpacing, float ySpacing)
    {
        // Set background color for sub-identifier nodes
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.cyan; // Use a distinct color for sub-identifiers

        // Draw the sub-identifier box
        GUI.Box(new Rect(position.x, position.y, 150, 100), subIdentifier.SubIdentifierName); // Increased height for appearance characteristics

        // Reset the color after drawing the sub-identifier
        GUI.backgroundColor = originalColor;

        // Position for characteristics
        int lineHeight = 15;
        int lineOffset = 0;

        // If the sub-identifier has action characteristics, draw them inside the box
        if (subIdentifier.ActionCharacteristicsWithValue != null && subIdentifier.ActionCharacteristicsWithValue.Count > 0)
        {
            for (int i = 0; i < subIdentifier.ActionCharacteristicsWithValue.Count; i++)
            {
                EditorGUI.LabelField(new Rect(position.x + 5, position.y + 20 + lineOffset * lineHeight, 140, 20),
                    $"{subIdentifier.ActionCharacteristicsWithValue[i].CharacteristicType}: {subIdentifier.ActionCharacteristicsWithValue[i].Value}");
                lineOffset++;
            }
        }

        // If the sub-identifier has appearance characteristics, draw them inside the box
        if (subIdentifier.AppearanceCharacteristicsWithValue != null && subIdentifier.AppearanceCharacteristicsWithValue.Count > 0)
        {
            for (int i = 0; i < subIdentifier.AppearanceCharacteristicsWithValue.Count; i++)
            {
                EditorGUI.LabelField(new Rect(position.x + 5, position.y + 20 + lineOffset * lineHeight, 140, 20),
                    $"{subIdentifier.AppearanceCharacteristicsWithValue[i].CharacteristicType}: {subIdentifier.AppearanceCharacteristicsWithValue[i].Value}");
                lineOffset++;
            }
        }

        // Position for the "Add Characteristic" button
        float addButtonYPosition = position.y + 20 + lineOffset * lineHeight + 5; // Adjust this based on the height of the box
        if (GUI.Button(new Rect(position.x + 5, addButtonYPosition, 140, 20), "Add Characteristic"))
        {
            ShowAddCharacteristicWindow(subIdentifier);
        }

        // Position for the "Edit Characteristics" button
        float editButtonYPosition = addButtonYPosition + 25; // Position below the "Add Characteristic" button
        if (GUI.Button(new Rect(position.x + 5, editButtonYPosition, 140, 20), "Edit Characteristics"))
        {
            ShowEditCharacteristicsWindow(subIdentifier);
        }

        // Position for the "Edit Identifier" button
        float editSubIdentifierButtonYPosition = editButtonYPosition + 25; // Position below the "Edit Characteristics" button

        if (GUI.Button(new Rect(position.x + 5, editSubIdentifierButtonYPosition, 140, 20), "Edit SubIdentifier"))
        {
            ShowEditSubIdentifierWindow(subIdentifier);  // Call method to show edit window for SubIdentifier
        }

        // Position for the "Delete SubIdentifier" button
        float deleteButtonYPosition = editSubIdentifierButtonYPosition + 25; // Position below the "Edit SubIdentifier" button
        if (GUI.Button(new Rect(position.x + 5, deleteButtonYPosition, 140, 20), "Delete SubIdentifier"))
        {
            if (parent.SubIdentifiers != null)
            {
                parent.SubIdentifiers.Remove(subIdentifier); // Remove the SubIdentifier from the parent's list
                EditorUtility.SetDirty(parent.Tracker); // Mark the parent ScriptableObject as dirty to save the changes
            }
        }
    }


    private void ShowEditSubIdentifierWindow(SubIdentifier subIdentifier)
    {
        // Open the EditSubIdentifierWindow with the selected subIdentifier
        EditSubIdentifierWindow.Open(subIdentifier);
    }


    private void ShowEditCharacteristicsWindow(SubIdentifier subIdentifier)
    {
        // Make sure to check if subIdentifier is not null
        if (subIdentifier == null)
        {
            Debug.LogError("SubIdentifier is null, cannot open edit window.");
            return;
        }

        EditCharacteristicsWindow.Open(subIdentifier);
    }



    // Method to add a new SubIdentifier to a node
    private void ShowAddSubIdentifierWindow(IdentifierNode node)
    {
        AddSubIdentifierWindow.Open(node);
    }
}
