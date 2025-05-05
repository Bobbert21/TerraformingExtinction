using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RelationshipNodeGraphWindow : EditorWindow
{
    private RelationshipPersonalTreeSO trackerSO;
    private int selectedRootIndex = 0;
    private string[] rootIdentifiersNames;

    private Vector2 windowSize;

    [MenuItem("Window/Relationship Node Graph")]
    public static void OpenWindow()
    {
        RelationshipNodeGraphWindow window = GetWindow<RelationshipNodeGraphWindow>("Relationship Node Graph");
        window.Show();
    }

    private void OnGUI()
    {
        windowSize = position.size;

        trackerSO = (RelationshipPersonalTreeSO)EditorGUILayout.ObjectField("Identifier Tracker", trackerSO, typeof(RelationshipPersonalTreeSO), false);

        if (trackerSO == null)
        {
            EditorGUILayout.HelpBox("Please assign an IdentifierTrackerSO to visualize the tree.", MessageType.Warning);
            return;
        }

        if (trackerSO.RootIdentifiers != null && trackerSO.RootIdentifiers.Count > 0)
        {
            UpdateDropdownOptions();
            selectedRootIndex = EditorGUILayout.Popup("Select Root Identifier", selectedRootIndex, rootIdentifiersNames);

            GUILayout.Space(20);
            DrawNodeGraph(trackerSO.RootIdentifiers[selectedRootIndex]);
        }
        else
        {
            EditorGUILayout.HelpBox("Tree is not initialized or is empty.", MessageType.Info);
        }
    }

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

    private void DrawNodeGraph(IdentifierNode rootNode)
    {
        Vector2 startPos = new Vector2(windowSize.x / 2 - 75, 100);

        float xSpacing = 150f;
        float ySpacing = 170f;

        CreateNodeView(rootNode, null, startPos, xSpacing, ySpacing);
    }

    private void CreateNodeView(IdentifierNode node, IdentifierNode parent, Vector2 position, float xSpacing, float ySpacing)
    {
        // Draw the current node (IdentifierNode) at the given position
        GUI.Box(new Rect(position.x, position.y, 150, 60), node.Identifier.ToString());

        // Add "Edit Identifier" button
        float editIdentifierButtonYPosition = position.y;
        if (GUI.Button(new Rect(position.x + 160, editIdentifierButtonYPosition, 140, 20), "Edit Identifier"))
        {
            IdentifierEditorWindow.ShowWindow(node);
        }

        // Add "Add Relationship" button
        float addRelationshipButtonYPosition = editIdentifierButtonYPosition + 30;
        if (GUI.Button(new Rect(position.x + 160, addRelationshipButtonYPosition, 140, 20), "Add Relationship"))
        {
            // Add a new RelationshipNode to this IdentifierNode
            
            node.RelationshipNodes.Add(
                new RelationshipNode
                    {
                        Name = "New Relationship",
                        PRValues = new RelationshipValues(),
                        ModRValues = new RelationshipValues(),
                        ActionContext = new EnumActionCharacteristics(),
                    }
            );


            EditorUtility.SetDirty(trackerSO); // Mark ScriptableObject as dirty to save changes
        }

        // Draw RelationshipNodes below IdentifierNode and above SubIdentifier
        float relationshipOffsetY = ySpacing * 0.5f;
        if (node.RelationshipNodes != null)
        {
            Vector2 relationshipPos = new Vector2(position.x, position.y + relationshipOffsetY + 20);
            Vector2 identifierNodePos = new Vector2(position.x + 75, position.y + 60);
            foreach (var relationshipNode in node.RelationshipNodes)
            {
                DrawRelationshipNode(relationshipNode, relationshipPos, identifierNodePos);
                relationshipPos.x += 120; // optional: offset each one visually
            }
        }


        // Draw SubIdentifiers below RelationshipNodes
        float subIdentifierOffsetY = ySpacing;
        if (node.SubIdentifiers != null && node.SubIdentifiers.Count > 0)
        {
            float subIdentifierStartX = position.x - (node.SubIdentifiers.Count - 1) * xSpacing / 2;

            for (int i = 0; i < node.SubIdentifiers.Count; i++)
            {
                Vector2 subIdentifierPos = new Vector2(subIdentifierStartX + i * xSpacing, position.y + subIdentifierOffsetY);
                CreateSubIdentifierView(node.SubIdentifiers[i], node, subIdentifierPos, xSpacing, ySpacing);

                Handles.BeginGUI();
                Handles.DrawLine(new Vector3(position.x + 75, position.y + 60 + relationshipOffsetY, 0), new Vector3(subIdentifierPos.x + 75, subIdentifierPos.y, 0));
                Handles.EndGUI();
            }
        }

        if (node.Children != null && node.Children.Count > 0)
        {
            float childStartX = position.x - (node.Children.Count - 1) * xSpacing / 2;
            float childOffsetY = ySpacing + 100f;

            for (int i = 0; i < node.Children.Count; i++)
            {
                Vector2 childPos = new Vector2(childStartX + i * xSpacing, position.y + subIdentifierOffsetY + ySpacing);
                CreateNodeView(node.Children[i], node, childPos, xSpacing, ySpacing);

                Handles.BeginGUI();
                Handles.DrawLine(new Vector3(position.x + 75, position.y + 60, 0), new Vector3(childPos.x + 75, childPos.y, 0));
                Handles.EndGUI();
            }
        }
    }

    private void CreateSubIdentifierView(SubIdentifierNode subIdentifier, IdentifierNode parent, Vector2 position, float xSpacing, float ySpacing)
    {
        // Set background color for sub-identifier nodes
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.cyan;

        // Draw the sub-identifier box
        GUI.Box(new Rect(position.x, position.y, 150, 100), subIdentifier.SubIdentifierName);

        GUI.backgroundColor = originalColor;

        int lineHeight = 15;
        int lineOffset = 0;

        if (subIdentifier.ActionCharacteristicsWithValue != null && subIdentifier.ActionCharacteristicsWithValue.Count > 0)
        {
            foreach (var actionCharacteristic in subIdentifier.ActionCharacteristicsWithValue)
            {
                EditorGUI.LabelField(new Rect(position.x + 5, position.y + 20 + lineOffset * lineHeight, 140, 20),
                    $"{actionCharacteristic.CharacteristicType}: {actionCharacteristic.Value}");
                lineOffset++;
            }
        }

        if (subIdentifier.AppearanceCharacteristicsWithValue != null && subIdentifier.AppearanceCharacteristicsWithValue.Count > 0)
        {
            foreach (var appearanceCharacteristic in subIdentifier.AppearanceCharacteristicsWithValue)
            {
                EditorGUI.LabelField(new Rect(position.x + 5, position.y + 20 + lineOffset * lineHeight, 140, 20),
                    $"{appearanceCharacteristic.CharacteristicType}: {appearanceCharacteristic.Value}");
                lineOffset++;
            }
        }

        // Add "Edit SubIdentifier" button
        float editSubIdentifierButtonYPosition = position.y + 20 + lineOffset * lineHeight + 5;
        if (GUI.Button(new Rect(position.x + 5, editSubIdentifierButtonYPosition, 140, 20), "Edit SubIdentifier"))
        {
            ShowEditSubIdentifierWindow(subIdentifier);
        }

        // Add "Delete SubIdentifier" button
        float deleteButtonYPosition = editSubIdentifierButtonYPosition + 25;
        if (GUI.Button(new Rect(position.x + 5, deleteButtonYPosition, 140, 20), "Delete SubIdentifier"))
        {
            if (parent.SubIdentifiers != null)
            {
                parent.SubIdentifiers.Remove(subIdentifier);
                EditorUtility.SetDirty(parent.Tracker);
            }
        }


        // Draw RelationshipNodes between SubIdentifiers and IdentifierNode
        if (subIdentifier.RelationshipNodes != null && subIdentifier.RelationshipNodes.Count > 0)
        {
            float offsetX = -((subIdentifier.RelationshipNodes.Count - 1) * 110f) / 2f;
            for (int i = 0; i < subIdentifier.RelationshipNodes.Count; i++)
            {
                Vector2 relationshipStart = new Vector2(position.x + 75 + offsetX + i * 110f, position.y - 10);
                Vector2 parentPos = new Vector2(position.x + 75, position.y); // Adjusted to center above the subidentifier
                DrawRelationshipNode(subIdentifier.RelationshipNodes[i], relationshipStart, parentPos);
            }
        }


    }

    private void DrawRelationshipNode(RelationshipNode relationshipNode, Vector2 pos, Vector2 parentNodePos)
    {
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;

        // Set the width and height for the relationship node box
        float boxWidth = 100f;
        float boxHeight = 70f;

        // Center the relationship node box on the position `pos`
        Rect relationshipRect = new Rect(pos.x - boxWidth / 2, pos.y - boxHeight / 2, boxWidth, boxHeight);

        // Draw the relationship box centered at `pos`
        GUI.Box(relationshipRect, relationshipNode.Name);

        // Draw the red line connecting the bottom center of the IdentifierNode to the top center of the RelationshipNode
        Handles.BeginGUI();
        Color originalHandlesColor = Handles.color;
        Handles.color = Color.red;

        // Calculate the bottom-center point of the parent node and the top-center of the relationship node
        //Vector3 parentBottomCenter = new Vector3(parentNodePos.x, parentNodePos.y + 60, 0); // Assuming IdentifierNode has a height of 60
        Vector3 relationshipTopCenter = new Vector3(pos.x, pos.y - boxHeight / 2, 0);

        // Draw line between IdentifierNode and RelationshipNode
        Handles.DrawLine(parentNodePos, relationshipTopCenter);

        Handles.EndGUI();

        // Draw the "Value" label inside the box, centered horizontally and vertically
        GUI.Label(new Rect(relationshipRect.x + (boxWidth / 2) - 40, relationshipRect.y + (boxHeight / 2) - 10, 80, 20), $"{relationshipNode.PRValues}", EditorStyles.label);

        // Draw the "Edit Name" button at the bottom of the relationship box
        if (GUI.Button(new Rect(relationshipRect.x + (boxWidth / 2) - 50, relationshipRect.yMax + 25, 100, 20), "Edit Name"))
        {
            RelationshipNameEditorWindow.ShowWindow(relationshipNode);
        }

        // Draw the "Edit Values" button below the "Value" label, centered horizontally
        if (GUI.Button(new Rect(relationshipRect.x + (boxWidth / 2) - 50, relationshipRect.yMax, 100, 20), "Edit Values"))
        {
            RelationshipValuesWindow.ShowWindow(relationshipNode);
        }

        relationshipNode.ActionContext = (EnumActionCharacteristics)EditorGUI.EnumPopup(
        new Rect(relationshipRect.x + (boxWidth / 2) - 50, relationshipRect.yMax + 50, boxWidth - 10, 18),
        relationshipNode.ActionContext);

        Handles.color = originalHandlesColor;
        GUI.backgroundColor = originalColor;
    }





    private void ShowEditSubIdentifierWindow(SubIdentifierNode subIdentifier)
    {
        EditSubIdentifierWindow.Open(subIdentifier);
    }
}
