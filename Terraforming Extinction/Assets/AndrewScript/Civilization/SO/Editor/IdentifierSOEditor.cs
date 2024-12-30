using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IdentifierSO))]
public class IdentifierSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        IdentifierSO identifierSO = (IdentifierSO)target;

        // Draw default inspector elements first
        DrawDefaultInspector();

        GUILayout.Label("Can Edit Below", EditorStyles.boldLabel);

        // Draw a button to initialize the Identifier Tree
        if (GUILayout.Button("Initialize Identifier Tree"))
        {
            identifierSO.InitializeIdentifierTree();
        }

        GUILayout.Space(10);

        // Display the Identifier Tree with clickable nodes
        GUILayout.Label("Identifier Tree", EditorStyles.boldLabel);
        if (identifierSO.RootIdentifiers != null && identifierSO.RootIdentifiers.Count > 0)
        {
            foreach (var rootNode in identifierSO.RootIdentifiers)
            {
                DrawNode(rootNode, identifierSO);
            }
        }
    }

    private void DrawNode(IdentifierNode node, IdentifierSO identifierSO, int indentLevel = 0)
    {
        Color defaultColor = GUI.backgroundColor;

        //make green if selected
        if (identifierSO.identifiers.Contains(node.Identifier))
        {
            GUI.backgroundColor = Color.green;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Space(indentLevel * 20);  // Indent based on hierarchy level

        if (GUILayout.Button(indentLevel.ToString() + ": " + node.Identifier.ToString(), GUILayout.Width(150)))
        {
            //if not selected identifier
            if (!identifierSO.identifiers.Contains(node.Identifier))
            {
                identifierSO.identifiers.Add(node.Identifier);
                // Mark the ScriptableObject as dirty (flag) to make Unity update
                EditorUtility.SetDirty(identifierSO);
            }
            //if already selected, then unselect
            else
            {
                identifierSO.identifiers.Remove(node.Identifier);
                EditorUtility.SetDirty(identifierSO);
            }
        }
        GUILayout.EndHorizontal();

        //reset color for next button
        GUI.backgroundColor = defaultColor;

        if (node.Children != null && node.Children.Count > 0)
        {
            foreach (var childNode in node.Children)
            {
                DrawNode(childNode, identifierSO, indentLevel + 1); // Recursively draw child nodes
            }
        }
    }
}
