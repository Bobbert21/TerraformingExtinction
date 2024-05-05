using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using System.Collections.Generic;

// Tagging a class with the EditorTool attribute and no target type registers a global tool. Global tools are valid for any selection, and are accessible through the top left toolbar in the editor.
[EditorTool("Position Tool")]
class PositionTool : EditorTool
{
    List<Vector2> positions = new List<Vector2>();

    // Serialize this value to set a default value in the Inspector.
    [SerializeField] Texture2D m_ToolIcon;

    GUIContent m_IconContent;

    void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = "Position Tool",
            tooltip = "Position Tool"
        };
    }

    public override GUIContent toolbarIcon
    {
        get { return m_IconContent; }
    }

    // This is called for each window that your tool is active in. Put the functionality of your tool here.
    public override void OnToolGUI(EditorWindow window)
    {
        //If we're not in the scene view, exit.
        if (!(window is SceneView))
            return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        GUIStyle gUIStyle = new GUIStyle() { fontSize = 12, alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold };
        gUIStyle.normal.textColor = Color.magenta;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            var position = GetCurrentMousePositionInScene();
            positions.Add(position);
        }

        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Space)
                positions.Clear();
            else if (Event.current.control && Event.current.keyCode == KeyCode.Z) // Undo
            {
                if (positions.Count > 0)
                    positions.RemoveAt(positions.Count - 1);
                Event.current.Use();
            }
        }

        Handles.DrawWireDisc(GetCurrentMousePositionInScene(), Vector3.forward, 0.2f);

        foreach (var position in positions)
        {
            Handles.DrawSolidDisc(position, Vector3.forward, 0.15f);
            Handles.Label(position + (Vector2.down * 0.5f) + (Vector2.left * 4), position.ToString("0.0000"), gUIStyle);
        }

        //Force the window to repaint.
        window.Repaint();
    }

    Vector3 GetCurrentMousePositionInScene()
    {
        Vector3 mousePosition = Event.current.mousePosition;
        return HandleUtility.GUIPointToWorldRay(mousePosition).GetPoint(10);
    }
}