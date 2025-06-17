using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPsyche : MonoBehaviour
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

    public CharacterPsyche(CharacterPsycheSO characterPsycheSO)
    {
        RelationshipPersonalTree = characterPsycheSO.RelationshipPersonalTree;
        SelfIdentifier = characterPsycheSO.SelfIdentifier;
        Friends = characterPsycheSO.Friends;
        Enemy = characterPsycheSO.Enemy;    
        FriendsNodes = characterPsycheSO.FriendsNodes;
        EnemyNodes = characterPsycheSO.EnemyNodes;
        Identifier = characterPsycheSO.Identifier;
        BIdentity = characterPsycheSO.BIdentity;
        MentalOpportunities = characterPsycheSO.MentalOpportunities;
        RiskAversion = characterPsycheSO.RiskAversion;
        RewardInclination = characterPsycheSO.RewardInclination;
        EmpathyLevel = characterPsycheSO.EmpathyLevel;
        SelfEfficacy = characterPsycheSO.SelfEfficacy;
        ProgressInclination = characterPsycheSO.ProgressInclination;
        InternalMotivationLevel = characterPsycheSO.InternalMotivationLevel;
        MaxHabitCounter = characterPsycheSO.MaxHabitCounter;
        HabitualTendencies = characterPsycheSO.HabitualTendencies;
        PerspectiveAbility = characterPsycheSO.PerspectiveAbility;
        CognitiveStamina = characterPsycheSO.CognitiveStamina;
    }
}
