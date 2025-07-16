using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DMTestSO", menuName = "ScriptableObject/Civilization/DecisionTestSO")]
public class DecisionTestSO : ScriptableObject
{
    public string Name;
    public EnumActionCharacteristics Action;
    public EnumActionCharacteristics Context;
    public DMTypes DMType;
    public List<Perspective> Perspectives;
    public List<DecisionTestSO> ComplexGoalActions;
    public int HabitCounter;
}
