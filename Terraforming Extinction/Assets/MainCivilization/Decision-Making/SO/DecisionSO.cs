using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DMSO", menuName = "ScriptableObject/Civilization/DecisionSO")]
public class DecisionSO : ScriptableObject
{
    public string Name;
    public EnumActionCharacteristics Action;
    //public EnumActionCharacteristics Context;
    public DMTypes DMType;
    public List<Perspective> Perspectives;
    //The steps of actions in a complex goal 
    //Simple decisions does not use this list
    //To-DO Make a simple decision SO class without this list
    public List<DecisionSO> ComplexGoalListOfDecisions;
    public int HabitCounter;
}

