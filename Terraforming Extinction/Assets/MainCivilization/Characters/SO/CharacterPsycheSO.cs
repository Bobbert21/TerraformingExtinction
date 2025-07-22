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
    [Header("Decision Making Variables")]
    public RelationshipPersonalTree RelationshipPersonalTree;
    public SubIdentifierNode SelfIdentifier;
    public List<CharacterMainCPort> FriendsRanked;
    public List<CharacterMainCPort> EnemiesRanked;
    public Dictionary<CharacterMainCPort, SubIdentifierNode> FriendsCPortToSubNode = new Dictionary<CharacterMainCPort, SubIdentifierNode>();
    public Dictionary<CharacterMainCPort, SubIdentifierNode> EnemiesCPortToSubNode = new Dictionary<CharacterMainCPort, SubIdentifierNode>();
    public EnumPersonalityStats BIdentity;
    //How much to consider stats not the lowest for Action Selection
    public double OpportunismLevel;
    public double RiskAversion;
    public double RiskCutoff;
    public double RewardCutoff;
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
    public List<RelationshipDecisionNode> L_LearnedResponseDecisions;
    public List<RelationshipDecisionNode> NB_LearnedResponseDecisions;
    public List<RelationshipDecisionNode> DB_LearnedResponseDecisions;
    public List<RelationshipNode> L_LearnedEnvironment;
    public List<RelationshipNode> NB_LearnedEnvironment;
    public List<RelationshipNode> DB_LearnedEnvironment;

    [Header("Identifier Script Variables")]
    public double ProcessingSpeed;
    public float AwarenessLevel;
    public float DistinctiveAbility;
    public float ExistingNodesDistinctiveAbility;
    public float JudgementLevel;
    public float ExtrapolationLevel;
    public float GeneralizationLevel;
}
