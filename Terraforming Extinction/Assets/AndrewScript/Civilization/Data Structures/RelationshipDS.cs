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
public class PersonalRelationshipValues
{
    public int DefensiveBelongingValue;
    public int NurtureBelongingValue;
    public int LivelihoodValue;
}

[System.Serializable]
public class ModifierRelationshipValues
{
    public int DefensiveBelongingValue;
    public int NurtureBelongingValue;
    public int LivelihoodValue;
}

[System.Serializable]
public class RelationshipNode
{
    public string Name;
    public PersonalRelationshipValues PRValues;
    public ModifierRelationshipValues ModRValues;
    public List<ActionNode> ActionNodes;
}

[System.Serializable]
public class ActionNode
{
    public List<EnumActionCharacteristics> Action;
    public RewardNode Win;
    public RewardNode Loss;
}

[System.Serializable]
public class GoalNode
{
    public List<EnumGoals> Goals;
    public List<ActionNode> ActionNodes;
    public RewardNode Win;
    public RewardNode Loss;
}

[System.Serializable]
public class RewardNode
{
    public SystemTheory SystemType;
    public int PLValue;
    public int SLValue;
    public int PDBValue;
    public int PNBValue;
    public int SDBValue;
    public int SNBValue;
}
