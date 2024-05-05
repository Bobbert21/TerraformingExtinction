//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomPropertyDrawer(typeof(Sprite))]
//public class SpriteDrawer : PropertyDrawer
//{

//    private static GUIStyle s_TempStyle = new GUIStyle();

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        var ident = EditorGUI.indentLevel;
//        //EditorGUI.indentLevel = 0;

//        Rect spriteRect;

//        //create object field for the sprite
//        spriteRect = new Rect(position.x, position.y, position.width, position.height);
//        property.objectReferenceValue = EditorGUI.ObjectField(spriteRect, label, property.objectReferenceValue, typeof(Sprite), false);

//        //if this is not a repain or the property is null exit now
//        if (Event.current.type != EventType.Repaint || property.objectReferenceValue == null)
//            return;

//        //draw a sprite
//        //Sprite sp = property.objectReferenceValue as Sprite;

//        //spriteRect.x += 140;
//        //spriteRect.y += EditorGUIUtility.singleLineHeight + 4;
//        //spriteRect.width = 64;
//        //spriteRect.height = 64;
//        //s_TempStyle.normal.background = sp.texture;
//        //s_TempStyle.Draw(spriteRect, GUIContent.none, false, false, false, false);

//        EditorGUI.indentLevel = ident;
//    }

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return base.GetPropertyHeight(property, label) + 70f;
//    }
//}

//using UnityEngine;
//using UnityEditor;

//namespace IronCore.Editor
//{
//    [CustomPropertyDrawer(typeof(Sprite))]
//    public class SpritePropertyDrawer : PropertyDrawer
//    {
//        //public override float GetPropertyHeight(SerializedProperty property, GUIContent labelN)
//        //{
//        //    if (property.objectReferenceValue != null)
//        //    {
//        //        return _texSize;
//        //    }

//        //    return base.GetPropertyHeight(property, labelN);
//        //}

//        private const float _texSize = 70;


//        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
//        {
//            //EditorGUI.BeginProperty(position, label, prop);

//            if (prop.objectReferenceValue != null)
//            {
//                //position.width = EditorGUIUtility.labelWidth;
//                //GUI.Label(position, prop.displayName);

//                //position.x += position.width;
//                //position.width = _texSize;
//                //position.height = _texSize;

//                prop.objectReferenceValue =
//                    EditorGUI.ObjectField(position, prop.objectReferenceValue, typeof(Sprite), false);
//            }
//            else
//            {
//                EditorGUI.PropertyField(position, prop, true);
//            }

//            //EditorGUI.EndProperty();
//        }
//    }
//}

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

[CustomPropertyDrawer(typeof(SpritePropertyAttribute))]
public class SpritePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        //if (prop.objectReferenceValue != null)
        {
            return _textureSize;
        }
        //else
        {
            //return base.GetPropertyHeight(prop, label);
        }
    }

    private const float _textureSize = 65;


    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        //EditorGUI.BeginProperty(position, label, prop);

        //if (prop.objectReferenceValue != null)
        {
            position.width = EditorGUIUtility.currentViewWidth / 4;
            EditorGUI.LabelField(position, label);

            if (label != GUIContent.none)
                position.x += EditorGUIUtility.currentViewWidth / 4;

            //position.width = position.width - 40;
            //position.height = position.height;

            prop.objectReferenceValue = EditorGUI.ObjectField(position, prop.objectReferenceValue, typeof(Sprite), false);
        }
        //else
        {
            //EditorGUI.PropertyField(position, prop, true);
        }

        //EditorGUI.EndProperty();
    }
}


[AttributeUsage(AttributeTargets.Field)]
public class SpritePropertyAttribute : PropertyAttribute
{
    public GUIContent BuildLabel(GUIContent label)
    {
        return GUIContent.none;
    }

    public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
    }

    internal virtual float? GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return null;
    }
}

