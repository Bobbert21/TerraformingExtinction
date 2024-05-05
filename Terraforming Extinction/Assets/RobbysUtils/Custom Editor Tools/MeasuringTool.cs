using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using System.Collections.Generic;

// Tagging a class with the EditorTool attribute and no target type registers a global tool. Global tools are valid for any selection, and are accessible through the top left toolbar in the editor.
[EditorTool("Measuring Tool")]
class MeasuringTool : EditorTool
{
    Vector2 position1;
    Vector2 position2;

    bool mouseDown = false;

    // Serialize this value to set a default value in the Inspector.
    [SerializeField] Texture2D m_ToolIcon;

    List<Line> lines = new List<Line>();

    GUIContent m_IconContent;

    void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = "Measuring Tool",
            tooltip = "Measuring Tool"
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

        GUIStyle gUIStyle = new GUIStyle() { fontSize = 12, alignment = TextAnchor.MiddleCenter };
        gUIStyle.normal.textColor = Color.green;

        Vector2 mousePosition = GetCurrentMousePositionInScene();

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            position1 = Vector2.zero;
            position2 = Vector2.zero;

            position1 = mousePosition;

            mouseDown = true;

            Event.current.Use();
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            mouseDown = false;

            position2 = mousePosition;
            var line = new Line() { point1 = position1, point2 = position2, distance = RobbysUtils.TransformUtils.GetDistance(position1, position2) };
            lines.Add(line);
        }

        if (mouseDown)
        {
            using (new Handles.DrawingScope(Color.green))
            {
                Handles.DrawWireDisc(mousePosition, Vector3.forward, 0.2f);
                Handles.DrawSolidDisc(position1, Vector3.forward, 0.2f);
                Handles.DrawDottedLine(position1, mousePosition, 5);
                Handles.Label(mousePosition + (Vector2.up * 1.5f) + (Vector2.right * -1), RobbysUtils.TransformUtils.GetDistance(position1, mousePosition).ToString("0.00000"), gUIStyle);
            }
        }
        else
            Handles.DrawWireDisc(mousePosition, Vector3.forward, 0.2f);

        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Space)
                lines.Clear();
            else if (Event.current.control && Event.current.keyCode == KeyCode.Z) // Undo
            {
                if (lines.Count > 0)
                    lines.RemoveAt(lines.Count - 1);
                Event.current.Use();
            }
        }

        foreach (var line in lines)
        {
            Handles.DrawSolidDisc(line.point1, Vector3.forward, 0.15f);
            Handles.DrawSolidDisc(line.point2, Vector3.forward, 0.15f);
            Handles.DrawLine(line.point1, line.point2);
            Handles.Label(RobbysUtils.TransformUtils.GetMidPoint(line.point1, line.point2) + (Vector2.up * 1.25f) + (Vector2.right * -1), line.distance.ToString("0.00000"), gUIStyle);
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

[Serializable]
public class Line
{
    public Vector2 point1;
    public Vector2 point2;
    public float distance;
}