using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class NodeView
{
    private IdentifierNode node;
    private Vector2 position;
    private NodeView parentView;
    private IdentifierTrackerSO trackerSO;  // Reference to the tracker scriptable object
    private bool showSubIdentifiers = false;

    private const float Width = 150;
    private const float Height = 50;

    public NodeView(IdentifierNode node, Vector2 position, NodeView parent, IdentifierTrackerSO trackerSO)
    {
        this.node = node;
        this.position = position;
        this.parentView = parent;
        this.trackerSO = trackerSO;  // Store the reference
    }

    // Public property to access position
    public Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }

    public void Draw()
    {
        // Draw node box
        Rect rect = new Rect(position.x, position.y, Width, Height);
        GUI.Box(rect, node.Identifier.ToString());

        // Draw lines from parent to child if available
        if (parentView != null)
        {
            DrawConnection(parentView.Position, this.Position);  // Use Position property here
        }

        // Draw node properties and options to add subidentifiers
        GUILayout.BeginArea(new Rect(position.x, position.y + Height + 10, Width, 200));
        GUILayout.Label("Identifier: " + node.Identifier);

        if (GUILayout.Button("Add SubIdentifier"))
        {
            AddSubIdentifier();
        }

        showSubIdentifiers = EditorGUILayout.Foldout(showSubIdentifiers, "SubIdentifiers");

        if (showSubIdentifiers)
        {
            foreach (var subIdentifier in node.SubIdentifiers)
            {
                GUILayout.Label("Sub: " + subIdentifier.SubIdentifierName);
            }
        }

        GUILayout.EndArea();
    }

    public void Drag(Vector2 delta)
    {
        Position += delta;  // Use Position property here
    }

    private void DrawConnection(Vector2 startPos, Vector2 endPos)
    {
        Vector3 start = new Vector3(startPos.x + Width / 2, startPos.y + Height / 2, 0);
        Vector3 end = new Vector3(endPos.x + Width / 2, endPos.y + Height / 2, 0);
        Handles.DrawLine(start, end);
    }

    private void AddSubIdentifier()
    {
        // Create a new subidentifier and add it to the current node
        var newSubIdentifier = new SubIdentifier(node)
        {
            SubIdentifierName = "New SubIdentifier"
        };
        node.SubIdentifiers.Add(newSubIdentifier);

        // Mark the ScriptableObject as dirty so changes are saved
        EditorUtility.SetDirty(trackerSO);  // Mark the trackerSO as dirty
    }
}
