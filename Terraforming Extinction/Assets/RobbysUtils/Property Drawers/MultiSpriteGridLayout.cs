using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class MultiSpriteGridLayout
{
    [SpriteProperty][SerializeField] Sprite topLeft;
    [SpriteProperty][SerializeField] Sprite topMidd;
    [SpriteProperty][SerializeField] Sprite topRigh;

    [SpriteProperty][SerializeField] Sprite midLeft;
    [SpriteProperty][SerializeField] Sprite midMidd;
    [SpriteProperty][SerializeField] Sprite midRigh;

    [SpriteProperty][SerializeField] Sprite botLeft;
    [SpriteProperty][SerializeField] Sprite botMidd;
    [SpriteProperty][SerializeField] Sprite botRigh;

    public Sprite TopLeft { get => topLeft; set => topLeft = value; }
    public Sprite TopMidd { get => topMidd; set => topMidd = value; }
    public Sprite TopRigh { get => topRigh; set => topRigh = value; }
    public Sprite MidLeft { get => midLeft; set => midLeft = value; }
    public Sprite MidMidd { get => midMidd; set => midMidd = value; }
    public Sprite MidRigh { get => midRigh; set => midRigh = value; }
    public Sprite BotLeft { get => botLeft; set => botLeft = value; }
    public Sprite BotMidd { get => botMidd; set => botMidd = value; }
    public Sprite BotRigh { get => botRigh; set => botRigh = value; }
}

[CustomPropertyDrawer(typeof(MultiSpriteGridLayout))]
public class MultiSpriteGridLayoutDrawer : PropertyDrawer
{
    float basePositionXModifier = 0;
    float basePositionYModifier = 20;
    float inBetweenSpace = 5;
    float sizeX = 40;
    float sizeY = 20;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (sizeY + inBetweenSpace) * 3 + basePositionYModifier;
        //float height = 0;
        //height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("topLeft")); // Drawing an object. That object can tell me what it's height should be (e.g. through its own PropertyDrawer's GetPropertyHeight())
        //height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("topMidd")); // Drawing an object. That object can tell me what it's height should be (e.g. through its own PropertyDrawer's GetPropertyHeight())
        //height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("topRigh")); // Drawing an object. That object can tell me what it's height should be (e.g. through its own PropertyDrawer's GetPropertyHeight())
        //height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("midLeft")); // Drawing an object. That object can tell me what it's height should be (e.g. through its own PropertyDrawer's GetPropertyHeight())
        //height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("midMidd")); // Drawing an object. That object can tell me what it's height should be (e.g. through its own PropertyDrawer's GetPropertyHeight())
        //height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("midRigh")); // Drawing an object. That object can tell me what it's height should be (e.g. through its own PropertyDrawer's GetPropertyHeight())
        //height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("botLeft")); // Drawing an object. That object can tell me what it's height should be (e.g. through its own PropertyDrawer's GetPropertyHeight())
        //height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("botMidd")); // Drawing an object. That object can tell me what it's height should be (e.g. through its own PropertyDrawer's GetPropertyHeight())
        //height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("botRigh")); // Drawing an object. That object can tell me what it's height should be (e.g. through its own PropertyDrawer's GetPropertyHeight())
        //return height;
    }


    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        sizeX = (EditorGUIUtility.currentViewWidth / 3) - 15;
        sizeY = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("topLeft"));

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.LabelField(position, property.displayName);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float basePositionX = position.x + basePositionXModifier;
        float basePositionY = position.y + basePositionYModifier;

        // Calculate rects
        var topLeftRect = new Rect(basePositionX, basePositionY, sizeX, sizeY);
        var topMiddRect = new Rect(basePositionX + (sizeX + inBetweenSpace), basePositionY, sizeX, sizeY);
        var topRighRect = new Rect(basePositionX + (sizeX + inBetweenSpace) * 2, basePositionY, sizeX, sizeY);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(topLeftRect, property.FindPropertyRelative("topLeft"), GUIContent.none);
        EditorGUI.PropertyField(topMiddRect, property.FindPropertyRelative("topMidd"), GUIContent.none);
        EditorGUI.PropertyField(topRighRect, property.FindPropertyRelative("topRigh"), GUIContent.none);

        // Calculate rects
        var midLeftRect = new Rect(basePositionX, basePositionY + (sizeY + inBetweenSpace), sizeX, sizeY);
        var midMiddRect = new Rect(basePositionX + (sizeX + inBetweenSpace), basePositionY + (sizeY + inBetweenSpace), sizeX, sizeY);
        var midRighRect = new Rect(basePositionX + (sizeX + inBetweenSpace) * 2, basePositionY + (sizeY + inBetweenSpace), sizeX, sizeY);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(midLeftRect, property.FindPropertyRelative("midLeft"), GUIContent.none);
        EditorGUI.PropertyField(midMiddRect, property.FindPropertyRelative("midMidd"), GUIContent.none);
        EditorGUI.PropertyField(midRighRect, property.FindPropertyRelative("midRigh"), GUIContent.none);

        // Calculate rects
        var botLeftRect = new Rect(basePositionX, basePositionY + (sizeY + inBetweenSpace) * 2, sizeX, sizeY);
        var botMiddRect = new Rect(basePositionX + (sizeX + inBetweenSpace), basePositionY + (sizeY + inBetweenSpace) * 2, sizeX, sizeY);
        var botRighRect = new Rect(basePositionX + (sizeX + inBetweenSpace) * 2, basePositionY + (sizeY + inBetweenSpace) * 2, sizeX, sizeY);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(botLeftRect, property.FindPropertyRelative("botLeft"), GUIContent.none);
        EditorGUI.PropertyField(botMiddRect, property.FindPropertyRelative("botMidd"), GUIContent.none);
        EditorGUI.PropertyField(botRighRect, property.FindPropertyRelative("botRigh"), GUIContent.none);

        // Set indent back to what it was
        //EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}