using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





[CreateAssetMenu(fileName = "CharacterTestSO", menuName = "ScriptableObject/Civilization/CharacterDebug")]

[System.Serializable]
public class StatsTest
{
    public double L;
    public double DB;
    public double NB;
}

[System.Serializable]
public class ModRTest
{
    public EnumActionCharacteristics ActionCharacteristics;
    public CharactersTestSO Identifiers;
    public StatsTest Stats;
}

[System.Serializable]
public class RelationshipSheetTest
{
    public List<RelationshipValueTest> PR;
    public List<RelationshipValueTest> SR;
    public List<RelationshipValueTest> ModR;
}

[System.Serializable]
public class RelationshipValueTest
{
    public EnumActionCharacteristics ActionCharacteristics;
    public CharactersTestSO Identifiers;
    public StatsTest Stats;
}

public class CharactersTestSO : ScriptableObject
{
    public string Name;
    public StatsTest Stats;
    public RelationshipSheetTest RelationshipSheet;
    public List<CharactersTestSO> FriendsTest;
    public List<CharactersTestSO> EnemyTest;
    public EnumIdentifiers Identifier;
    public EnumPersonalityStats BIdentity;
    public EnumMentalOpportunities MentalOpportunities;
    public double RiskAversion;
    public double RewardInclination;
    public double EmpathyLevel;
    public double SelfEfficacy;
    public double ProgressInclination;
    public int MaxHabitCounter = 10;
    public double HabitualTendencies;
    //How many perspectives they can consider
    public int PerspectiveAbility;
    //how many actions they can decide
    public int CognitiveStamina;
}
