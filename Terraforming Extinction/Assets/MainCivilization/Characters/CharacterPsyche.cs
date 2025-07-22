using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPsyche : MonoBehaviour
{
    [Header("Decision Making Variables")]
    public RelationshipPersonalTree RelationshipPersonalTree;
    public SubIdentifierNode SelfIdentifier;
    /*
    public List<CharacterMainCPort> Friends;
    public List<CharacterMainCPort> Enemy;
    public List<SubIdentifierNode> FriendsNodes;
    public List<SubIdentifierNode> EnemyNodes;
    */

    public List<CharacterMainCPort> FriendsRanked;
    public List<CharacterMainCPort> EnemiesRanked;
    public Dictionary<CharacterMainCPort, SubIdentifierNode> FriendsCPortToSubNode = new Dictionary<CharacterMainCPort, SubIdentifierNode>();
    public Dictionary<CharacterMainCPort, SubIdentifierNode> EnemiesCPortToSubNode = new Dictionary<CharacterMainCPort, SubIdentifierNode>();
    public EnumPersonalityStats BIdentity;
    public double OpportunismLevel;
    public double RiskAversion;
    public double RewardCutoff;
    public double RiskCutoff;
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
    public List<RelationshipDecisionNode> L_InternalResponseDecisions;
    public List<RelationshipDecisionNode> NB_InternalResponseDecisions;
    public List<RelationshipDecisionNode> DB_InternalResponseDecisions;
    public List<RelationshipNode> L_LearnedEnvironment;
    public List<RelationshipNode> NB_LearnedEnvironment;
    public List<RelationshipNode> DB_LearnedEnvironment;
    public Dictionary<DecisionSO, int> Decision_Step_Tracker = new Dictionary<DecisionSO, int>();

    [Header("Identifier Script Variables")]
    public double ProcessingSpeed;
    public float AwarenessLevel;
    public float DistinctiveAbility;
    public float ExistingNodesDistinctiveAbility;
    public float JudgementLevel;
    public float ExtrapolationLevel;
    public float GeneralizationLevel;

    public CharacterPsyche(CharacterPsycheSO characterPsycheSO)
    {
        RelationshipPersonalTree = characterPsycheSO.RelationshipPersonalTree;
        SelfIdentifier = characterPsycheSO.SelfIdentifier;
        FriendsRanked = characterPsycheSO.FriendsRanked;
        EnemiesRanked = characterPsycheSO.EnemiesRanked;
        FriendsCPortToSubNode = characterPsycheSO.FriendsCPortToSubNode;
        EnemiesCPortToSubNode = characterPsycheSO.EnemiesCPortToSubNode;
        BIdentity = characterPsycheSO.BIdentity;
        OpportunismLevel = characterPsycheSO.OpportunismLevel;
        RiskAversion = characterPsycheSO.RiskAversion;
        RewardCutoff = characterPsycheSO.RewardCutoff;
        RiskCutoff = characterPsycheSO.RiskCutoff;
        EmpathyLevel = characterPsycheSO.EmpathyLevel;
        SelfEfficacy = characterPsycheSO.SelfEfficacy;
        ProgressInclination = characterPsycheSO.ProgressInclination;
        InternalMotivationLevel = characterPsycheSO.InternalMotivationLevel;
        MaxHabitCounter = characterPsycheSO.MaxHabitCounter;
        HabitualTendencies = characterPsycheSO.HabitualTendencies;
        PerspectiveAbility = characterPsycheSO.PerspectiveAbility;
        CognitiveStamina = characterPsycheSO.CognitiveStamina;
        L_InternalResponseDecisions = characterPsycheSO.L_LearnedResponseDecisions;
        DB_InternalResponseDecisions = characterPsycheSO.DB_LearnedResponseDecisions;
        NB_InternalResponseDecisions = characterPsycheSO.NB_LearnedResponseDecisions;
        L_LearnedEnvironment = characterPsycheSO.L_LearnedEnvironment;
        DB_LearnedEnvironment = characterPsycheSO.DB_LearnedEnvironment;
        NB_LearnedEnvironment = characterPsycheSO.NB_LearnedEnvironment;

        //Identifier Script Variables
        ProcessingSpeed = characterPsycheSO.ProcessingSpeed;
        AwarenessLevel = characterPsycheSO.AwarenessLevel;
        DistinctiveAbility = characterPsycheSO.DistinctiveAbility;
        ExistingNodesDistinctiveAbility = characterPsycheSO.ExistingNodesDistinctiveAbility;
        JudgementLevel = characterPsycheSO.JudgementLevel;
        ExtrapolationLevel = characterPsycheSO.ExtrapolationLevel;
        GeneralizationLevel = characterPsycheSO.GeneralizationLevel;
    }
}
