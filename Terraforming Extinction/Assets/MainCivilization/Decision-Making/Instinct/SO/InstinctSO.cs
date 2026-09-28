using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InstinctPersonalityTrigger
{
    public EnumPersonalityStats Stat;
    public double Value;
    public bool TriggersWhenAbove;
}

[Serializable]
public class ImpulsiveControl
{
    public EnumPersonalityStats Stat;
    public double Limit;
    public bool UpperLimit;
}

[CreateAssetMenu(fileName = "InstinctSO", menuName = "ScriptableObject/CPort/InstinctSO")]
public class InstinctSO : ScriptableObject
{
    public string Name;
    public List<InstinctPersonalityTrigger> Triggers = new();
    public RelationshipValues LearnedModRValues;
    //The cutoff for the learned Mod R values to not consider the action
    public List<ImpulsiveControl> ImpulsiveControlCutoff = new();
    public DecisionSO Decision;
}
