using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public enum EnumIdentifiers
{
    None,
    Uprooter,
    Root,
    Alien,
    FistRooter,
    DarkAlien,
    CatholicRoot,
    Count
}

[System.Serializable]
public class IdentifierNode
{
    public EnumIdentifiers Identifier;
    public IdentifierNode Parent;
    public IdentifierTrackerSO Tracker; // Reference to the parent ScriptableObject
    public List<RelationshipNode> RelationshipValues = new();
    public List<IdentifierNode> Children = new();
    public List<SubIdentifier> SubIdentifiers = new();

    public void AddChild(IdentifierNode child)
    {
        Children.Add(child);
        child.Parent = this;

        // Mark Tracker (ScriptableObject) as dirty
        if (Tracker != null)
        {
            EditorUtility.SetDirty(Tracker);
        }
    }

    public void AddSubIdentifier(SubIdentifier subIdentifier)
    {
        SubIdentifiers.Add(subIdentifier);
        subIdentifier.Parent = this;

        // Mark Tracker (ScriptableObject) as dirty
        if (Tracker != null)
        {
            EditorUtility.SetDirty(Tracker);
        }
    }
}

[System.Serializable]
public class SubIdentifier
{
    public string SubIdentifierName;
    public List<ActionCharacteristicWithValue> ActionCharacteristicsWithValue = new();
    public List<AppearanceCharacteristicWithValue> AppearanceCharacteristicsWithValue = new();
    public List<RelationshipNode> RelationshipValues = new();
    public bool isZeroPoint = false;
    public bool isAnchor = false;
    public SubIdentifier Anchor = null;
    public List<SubIdentifier> Versions = new();
    public IdentifierNode Parent;

    public SubIdentifier(IdentifierNode parent)
    {
        Parent = parent;
    }

    public void AddActionCharacteristic(EnumActionCharacteristics characteristic, int value)
    {
        ActionCharacteristicsWithValue.Add(new ActionCharacteristicWithValue(characteristic, value));

        // Mark Tracker (ScriptableObject) as dirty
        if (Parent != null && Parent.Tracker != null)
        {
            EditorUtility.SetDirty(Parent.Tracker);
        }
    }

    public void AddAppearanceCharacteristic(EnumAppearanceCharacteristics characteristic, int value) // Add this method
    {
        AppearanceCharacteristicsWithValue.Add(new AppearanceCharacteristicWithValue(characteristic, value));

        // Mark Tracker (ScriptableObject) as dirty
        if (Parent != null && Parent.Tracker != null)
        {
            EditorUtility.SetDirty(Parent.Tracker);
        }
    }


}

[System.Serializable]
public class ActionCharacteristicWithValue
{
    public EnumActionCharacteristics CharacteristicType;
    public int Value;

    public ActionCharacteristicWithValue(EnumActionCharacteristics characteristic, int value)
    {
        CharacteristicType = characteristic;
        Value = value;
    }


    public ActionCharacteristicWithValue()
    {
        //Characteristic = Characteristics.None;
        Value = 0;
    }
}

[System.Serializable]
public class AppearanceCharacteristicWithValue
{
    public EnumAppearanceCharacteristics CharacteristicType;
    public int Value;

    public AppearanceCharacteristicWithValue(EnumAppearanceCharacteristics characteristic, int value)
    {
        CharacteristicType = characteristic;
        Value = value;
    }


    public AppearanceCharacteristicWithValue()
    {
        //Characteristic = Characteristics.None;
        Value = 0;
    }
}