using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using JetBrains.Annotations;


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
    public RelationshipPersonalTreeSO Tracker; // Reference to the parent ScriptableObject
    public List<RelationshipNode> RelationshipNodes = new();
    public List<IdentifierNode> Children = new();
    public List<SubIdentifierNode> SubIdentifiers = new();

    public IdentifierNode DeepCopy()
    {
        return new IdentifierNode
        {
            Identifier = this.Identifier,
            SubIdentifiers = SubIdentifiers != null
                ? new List<SubIdentifierNode>(SubIdentifiers.ConvertAll(s => s.DeepCopy()))
                : new List<SubIdentifierNode>()
        };
    }

    public void AddChild(IdentifierNode child)
    {
        Children.Add(child);
        child.Parent = this;

        // Mark Tracker (ScriptableObject) as dirty
        if (Tracker != null)
        {
            #if UNITY_EDITOR
                EditorUtility.SetDirty(Tracker);
            #endif
        }
    }

    public void AddSubIdentifier(SubIdentifierNode subIdentifier)
    {
        SubIdentifiers.Add(subIdentifier);
        subIdentifier.Parent = this;

        // Mark Tracker (ScriptableObject) as dirty
        if (Tracker != null)
        {
            #if UNITY_EDITOR
                EditorUtility.SetDirty(Tracker);
            #endif
        }
    }

    public void AddRelationshipNode(RelationshipNode relationshipNode)
    {
        RelationshipNode existingRelationshipNode = RelationshipNodes.
            FirstOrDefault(r => r.ActionContext == relationshipNode.ActionContext);

        if (existingRelationshipNode != null)
        {
            //Add to already exiting value if context action the same
            existingRelationshipNode.PRValues.AddValues(relationshipNode.PRValues);
            existingRelationshipNode.ModRValues.AddValues(relationshipNode.ModRValues);
        }
        else
        {
            //Add deep copy
            RelationshipNodes.Add(new RelationshipNode(relationshipNode));
        }

    }
}

[System.Serializable]
public class SubIdentifierNode
{
    public string SubIdentifierName;
    public List<ActionCharacteristicWithValue> ActionCharacteristicsWithValue = new();
    public List<AppearanceCharacteristicWithValue> AppearanceCharacteristicsWithValue = new();
    //Adopted values before zero point
    public List<ActionCharacteristicWithValue> LearningPeriodActionCharacteristicsWithValue = new();
    public List<AppearanceCharacteristicWithValue> LearningPeriodAppearanceCharacteristicsWithValue = new();
    public List<RelationshipNode> RelationshipNodes = new();
    //adopted relationships values before zero value
    public List<RelationshipNode> LearningPeriodRelationshipValues = new();
    public bool isZeroPoint = false;
    public bool isAnchor = false;
    public SubIdentifierNode Heuristic = null; 
    public List<SubIdentifierNode> Specifics = new();
    public IdentifierNode Parent;

    public SubIdentifierNode() { }


    public SubIdentifierNode(IdentifierNode parent)
    {
        Parent = parent;
    }

    public SubIdentifierNode DeepCopy()
    {
        return new SubIdentifierNode
        {
            Parent = Parent,
            SubIdentifierName = this.SubIdentifierName,
            Heuristic = this.Heuristic,
            isAnchor = this.isAnchor,
            AppearanceCharacteristicsWithValue = AppearanceCharacteristicsWithValue != null
                ? AppearanceCharacteristicsWithValue.ConvertAll(a => a.DeepCopy())
                : new List<AppearanceCharacteristicWithValue>(),
            ActionCharacteristicsWithValue = ActionCharacteristicsWithValue != null
                ? ActionCharacteristicsWithValue.ConvertAll(a => a.DeepCopy())
                : new List<ActionCharacteristicWithValue>(),
            Specifics = Specifics != null
                ? Specifics.ConvertAll(s => s.DeepCopy())
                : new List<SubIdentifierNode>(),
        };
    }

    public RelationshipNode GetMainRelationshipNode()
    {
        RelationshipNode mainNode = RelationshipNodes
            .FirstOrDefault(r => r.ActionContext == EnumActionCharacteristics.Main);

        return mainNode;
    }

    public void AddActionCharacteristic(EnumActionCharacteristics characteristic, float value)
    {
        var existing = ActionCharacteristicsWithValue
        .FirstOrDefault(c => c.CharacteristicType == characteristic);

        if (existing != null)
        {
            existing.Value += value; // Or use: existing.Value = value; to overwrite
        }
        else
        {
            ActionCharacteristicsWithValue.Add(
                new ActionCharacteristicWithValue(characteristic, value)
            );
        }

        // Mark Tracker (ScriptableObject) as dirty
        if (Parent != null && Parent.Tracker != null)
        {
            #if UNITY_EDITOR
                EditorUtility.SetDirty(Parent.Tracker);
            #endif
        }
    }

    public void AddAppearanceCharacteristic(EnumAppearanceCharacteristics characteristic, float value) // Add this method
    {
        var existing = AppearanceCharacteristicsWithValue
        .FirstOrDefault(c => c.CharacteristicType == characteristic);

        if (existing != null)
        {
            existing.Value += value; // Or overwrite: existing.Value = value;
        }
        else
        {
            AppearanceCharacteristicsWithValue.Add(
                new AppearanceCharacteristicWithValue
                {
                    CharacteristicType = characteristic,
                    Value = value
                }
            );
        }


        // Mark Tracker (ScriptableObject) as dirty
        if (Parent != null && Parent.Tracker != null)
        {
            #if UNITY_EDITOR
                EditorUtility.SetDirty(Parent.Tracker);
            #endif
        }
    }

    //This is for mixing together relationship nodes when creating new collective node
    public void AddRelationshipNode(RelationshipNode relationshipNode)
    {
        RelationshipNode existingRelationshipNode = RelationshipNodes.
            FirstOrDefault(r => r.ActionContext == relationshipNode.ActionContext);

        if (existingRelationshipNode != null)
        {
            //Add to already exiting value if context action the same
            existingRelationshipNode.PRValues.AddValues(relationshipNode.PRValues);
            existingRelationshipNode.ModRValues.AddValues(relationshipNode.ModRValues);
        }
        else
        {
            //Add deep copy
            RelationshipNodes.Add(new RelationshipNode(relationshipNode));
        }
    
    }


}

[System.Serializable]
public class ActionCharacteristicWithValue
{
    public EnumActionCharacteristics CharacteristicType;
    public float Value;

    public ActionCharacteristicWithValue DeepCopy()
    {
        return new ActionCharacteristicWithValue
        {
            CharacteristicType = this.CharacteristicType,
            Value = this.Value
        };
    }

    public ActionCharacteristicWithValue(EnumActionCharacteristics characteristic, float value)
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
    public float Value;

    public AppearanceCharacteristicWithValue DeepCopy()
    {
        return new AppearanceCharacteristicWithValue
        {
            CharacteristicType = this.CharacteristicType,
            Value = this.Value
        };
    }

    public AppearanceCharacteristicWithValue(EnumAppearanceCharacteristics characteristic, float value)
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