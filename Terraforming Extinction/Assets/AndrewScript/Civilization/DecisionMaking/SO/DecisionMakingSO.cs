using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DMSO", menuName = "ScriptableObject/Civilization/DecisionMakingSO")]
public class DecisionMakingSO : ScriptableObject
{
    public string Name;
    public EnumActionCharacteristics Action;
    public EnumActionCharacteristics Context;
    public DMTypes DMType;
    public List<Perspective> Perspectives;
    public List<DecisionMakingSO> ComplexGoalActions;
}
