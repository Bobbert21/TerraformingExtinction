  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EnumGoals
{
    None,
    Rob_Home,
    Count
}

[System.Serializable]
public enum SystemTheory
{
    Consume,
    Resist,
    Feed,
    Protect,
    Eliminate,
    Gain
}

[System.Serializable]
public class RelationshipValues
{
    public float DefensiveBelongingValue;
    public float NurtureBelongingValue;
    public float LivelihoodValue;
    public RelationshipValues() { }

    public RelationshipValues(RelationshipValues other)
    {
        DefensiveBelongingValue = other.DefensiveBelongingValue;
        NurtureBelongingValue = other.NurtureBelongingValue;
        LivelihoodValue = other.LivelihoodValue;
    }
    public RelationshipValues(float livelihoodValue, float defensiveBelongingValue, float nurtureBelongingValue)
    {
        DefensiveBelongingValue = defensiveBelongingValue;
        NurtureBelongingValue = nurtureBelongingValue;
        LivelihoodValue = livelihoodValue;
    }

    public void AddValues(RelationshipValues relationshipValues)
    {
        DefensiveBelongingValue += relationshipValues.DefensiveBelongingValue;
        NurtureBelongingValue += relationshipValues.NurtureBelongingValue;
        LivelihoodValue += relationshipValues.LivelihoodValue;
    }

    public override string ToString()
    {
        return "L: " + LivelihoodValue.ToString() + " NB: " + NurtureBelongingValue.ToString() + " DB: " + DefensiveBelongingValue.ToString();
    }
}

[System.Serializable]
public class RelationshipNode
{
    public string Name;
    public RelationshipValues PRValues;
    public RelationshipValues ModRValues;
    public EnumActionCharacteristics ActionContext;

    public RelationshipNode() { }

    //Copy constructor
    public RelationshipNode(RelationshipNode other)
    {
        Name = other.Name;
        PRValues = new RelationshipValues(other.PRValues);  
        ModRValues = new RelationshipValues(other.ModRValues);
        ActionContext = other.ActionContext; // enums are value types, so direct copy is fine
    }

    public RelationshipNode(string name, RelationshipValues pRValues, RelationshipValues modRValues, EnumActionCharacteristics actionContext)
    {
        Name = name;
        PRValues = pRValues;
        ModRValues = modRValues;
        ActionContext = actionContext;
    }
}

[System.Serializable]
public class ActionNode
{
    public string Name;
    public EnumActionCharacteristics Action;
    public RelationshipValues PRValues;
    public RelationshipValues ModRValues;
}
