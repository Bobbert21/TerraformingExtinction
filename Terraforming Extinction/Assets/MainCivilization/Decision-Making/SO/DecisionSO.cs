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
    public List<DecisionSO> ComplexGoalActions;
    public int HabitCounter;
}

