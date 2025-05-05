using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IdentifierNodeGraphWindow : EditorWindow
{
    private RelationshipPersonalTreeSO identifierTrackerSO;
    private EnumIdentifiers selectedIdentifier; // Selected identifier from the dropdown
    private EnumIdentifiers selectedChildIdentifier; // Selected child identifier from dropdown
    private Vector2 scrollPos;
    private int selectedRootIdentifierIndex = -1; // Index for the selected root identifier

    [MenuItem("Window/Identifier Node Graph")]
    public static void ShowWindow()
    {
        IdentifierNodeGraphWindow window = GetWindow<IdentifierNodeGraphWindow>("Identifier Node Graph");
        window.Show();
    }

    private void OnGUI()
    {
        // Select the IdentifierTrackerSO from the editor
        identifierTrackerSO = (RelationshipPersonalTreeSO)EditorGUILayout.ObjectField("IdentifierTrackerSO", identifierTrackerSO, typeof(RelationshipPersonalTreeSO), false);

        // If IdentifierTrackerSO is selected, render the dropdown for identifiers
        if (identifierTrackerSO != null)
        {
            // Dropdown to select from Identifiers enum
            selectedIdentifier = (EnumIdentifiers)EditorGUILayout.EnumPopup("Select Identifier to Add", selectedIdentifier);

            // Button to add a new root identifier
            if (GUILayout.Button("Add Root Identifier"))
            {
                AddRootIdentifier(selectedIdentifier);
            }

            // Dropdown to select which root identifier to visualize
            if (identifierTrackerSO.RootIdentifiers.Count > 0)
            {
                // Create a string array for the root identifiers
                string[] rootIdentifierNames = new string[identifierTrackerSO.RootIdentifiers.Count];
                for (int i = 0; i < identifierTrackerSO.RootIdentifiers.Count; i++)
                {
                    rootIdentifierNames[i] = identifierTrackerSO.RootIdentifiers[i].Identifier.ToString();
                }

                // Dropdown to select a root identifier to display
                selectedRootIdentifierIndex = EditorGUILayout.Popup("Select Root Identifier to Show", selectedRootIdentifierIndex, rootIdentifierNames);

                // Button to delete the selected root identifier
                if (GUILayout.Button("Delete Selected Root Identifier"))
                {
                    DeleteRootIdentifier(selectedRootIdentifierIndex);
                }

                // Dropdown to select from Identifiers enum for child node
                selectedChildIdentifier = (EnumIdentifiers)EditorGUILayout.EnumPopup("Select Child Identifier to Add", selectedChildIdentifier);

            }

            // Begin scrollable view
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Draw the selected root identifier node
            if (selectedRootIdentifierIndex >= 0 && selectedRootIdentifierIndex < identifierTrackerSO.RootIdentifiers.Count)
            {
                var rootNode = identifierTrackerSO.RootIdentifiers[selectedRootIdentifierIndex];
                DrawNode(rootNode, (position.width - 150) / 2, 10); // Center the root node
            }

            // End scrollable view
            EditorGUILayout.EndScrollView();
        }
    }

    // Function to add a new root identifier
    private void AddRootIdentifier(EnumIdentifiers identifier)
    {
        // Create a new IdentifierNode
        IdentifierNode newNode = new IdentifierNode
        {
            Identifier = identifier // Set the selected identifier
        };

        // Add the new node to the root identifiers list
        identifierTrackerSO.RootIdentifiers.Add(newNode);

        // Mark the ScriptableObject as dirty
        EditorUtility.SetDirty(identifierTrackerSO);
    }

    // Function to delete the selected root identifier
    private void DeleteRootIdentifier(int index)
    {
        if (index >= 0 && index < identifierTrackerSO.RootIdentifiers.Count)
        {
            // Remove the selected node
            identifierTrackerSO.RootIdentifiers.RemoveAt(index);

            // Reset selected index to ensure it's valid after removal
            selectedRootIdentifierIndex = -1;

            // Mark the ScriptableObject as dirty
            EditorUtility.SetDirty(identifierTrackerSO);
        }
    }


    // Recursive function to draw a node and its children
    private void DrawNode(IdentifierNode node, float x, float y)
    {
        // Draw node GUI box
        Rect nodeRect = new Rect(x, y, 150, 50);
        GUI.Box(nodeRect, node.Identifier.ToString());

        // Position for child identifier selection and buttons
        float dropdownY = y + 55; // Position below the node
        float buttonY = dropdownY + 25; // Position below the dropdown
        float deleteButtonY = buttonY + 25; // Position for delete button

        // Dropdown for child identifier selection
        selectedChildIdentifier = (EnumIdentifiers)EditorGUI.EnumPopup(new Rect(x + 10, dropdownY, 130, 20), selectedChildIdentifier);

        // Button to add a new child identifier node
        if (GUI.Button(new Rect(x + 10, buttonY, 130, 20), "Add Identifier Node"))
        {
            AddChildIdentifierNode(node, selectedChildIdentifier);
        }

        // Button to delete this node
        if (GUI.Button(new Rect(x + 10, deleteButtonY, 130, 20), "Delete Node"))
        {
            DeleteNode(node);
        }

        // Draw connections to children
        for (int i = 0; i < node.Children.Count; i++)
        {
            var childNode = node.Children[i];

            // Center children horizontally under the parent node
            float childX = x - 75 + (150 + 30) * i;
            float childY = y + 140; // Increased spacing between levels

            // Draw a line connecting the parent to the child
            Handles.DrawLine(new Vector3(nodeRect.x + 75, nodeRect.yMax, 0), new Vector3(childX + 75, childY, 0));

            // Recursive call to draw the child node
            DrawNode(childNode, childX, childY);
        }
    }

    // Function to delete the selected node from its parent's children
    private void DeleteNode(IdentifierNode node)
    {
        if (node.Parent != null)
        {
            // Remove the node from its parent's children
            node.Parent.Children.Remove(node);

            // Mark the ScriptableObject as dirty
            EditorUtility.SetDirty(identifierTrackerSO);
        }
    }

    // Function to add a child identifier node to the selected root identifier
    private void AddChildIdentifierNode(IdentifierNode parentNode, EnumIdentifiers identifier)
    {
        // Create a new IdentifierNode
        IdentifierNode childNode = new IdentifierNode
        {
            Identifier = identifier // Set the selected child identifier
        };

        childNode.Parent = parentNode;
        // Add the new child node to the parent node's children
        parentNode.AddChild(childNode);

        // Mark the ScriptableObject as dirty
        EditorUtility.SetDirty(identifierTrackerSO);
    }
}
