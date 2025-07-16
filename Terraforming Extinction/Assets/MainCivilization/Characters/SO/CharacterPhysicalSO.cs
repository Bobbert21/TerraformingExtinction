using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public double L;
    public double DB;
    public double NB;
}

[CreateAssetMenu(fileName = "CharacterPhysicalSO", menuName = "ScriptableObject/CPort/CharacterPhysicialSO")]
public class CharacterPhysicalSO : ScriptableObject
{
    public Stats Stats;
    public List<EnumActionCharacteristics> actionCharacteristics;
    public List<EnumAppearanceCharacteristics> appearanceCharacteristics;
    public EnumIdentifiers Identifier;
    public EnumActionCharacteristics ActionCommitting;
}
