using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public enum EnumMentalOpportunities
{
    Internal,
    External
}

//Can eliminate ICharacterPsyche. Will only use interface if there will be many subcategories of characters (i.e. enemies, players, allies, etc.)
[CreateAssetMenu(fileName = "CharacterPsycheSO", menuName = "ScriptableObject/CPort/CharacterPsycheSO")]
//TO-DO: Make CharacterPsyche, not SO. So I can adjust the values 
public class CharacterPsycheSO : ScriptableObject, ICharacterPsyche
{
    public RelationshipPersonalTree RelationshipPersonalTree;
    public SubIdentifierNode SelfIdentifier;
    public List<CharacterMainCPort> Friends;
    public List<CharacterMainCPort> Enemy;
    public List<SubIdentifierNode> FriendsNodes;
    public List<SubIdentifierNode> EnemyNodes;
    public EnumIdentifiers Identifier;
    public EnumPersonalityStats BIdentity;
    public EnumMentalOpportunities MentalOpportunities;
    public double RiskAversion;
    public double RewardInclination;
    public double EmpathyLevel;
    public double SelfEfficacy;
    public double ProgressInclination;
    public double InternalMotivationLevel;
    public int MaxHabitCounter = 10;
    public double HabitualTendencies;
    //How many perspectives they can consider
    public int PerspectiveAbility;
    //how many actions they can decide
    public int CognitiveStamina;
}
