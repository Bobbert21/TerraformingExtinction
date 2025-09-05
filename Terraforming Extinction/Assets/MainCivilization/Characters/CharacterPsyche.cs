using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceanrioMemoryEntry
{
    public float LastConsideredTime;
    public RelationshipNode Scenario;

    public SceanrioMemoryEntry(RelationshipNode scenario)
    {
        Scenario = scenario;
        LastConsideredTime = Time.time;
    }
}

public class ScenarioMemory
{
    public List<SceanrioMemoryEntry> AllScenarios = new List<SceanrioMemoryEntry>();
    public List<SceanrioMemoryEntry> RecentScenarios = new List<SceanrioMemoryEntry>();

    public void AddScenario(RelationshipNode scenario)
    {
        SceanrioMemoryEntry entry = new SceanrioMemoryEntry(scenario);

        //Add in order of habit counter for all scenario
        int index = AllScenarios.FindIndex(e => e.Scenario.HabitCounter < entry.Scenario.HabitCounter);
        if (index >= 0)
            AllScenarios.Insert(index, entry);
        else
            AllScenarios.Add(entry);

        RecentScenarios.Add(entry);
    }

    // Return a list of scenarios that the character can consider
    // Prioritizing the most recent scenarios first then grab rest from all scenarios
    public List<RelationshipNode> GetConsideredScenarios(float workingMemoryScenarioCapacity = 3, float fixatationScenarioTime = 50f)
    {
        UpdateRecentScenarios(fixatationScenarioTime);

        var results = new List<RelationshipNode>();
        var seen = new HashSet<RelationshipNode>(); // avoid duplicates

        // Step 1: take from RecentScenarios
        foreach (var entry in RecentScenarios)
        {
            if (results.Count >= workingMemoryScenarioCapacity) break;

            //Only true if scenario isn't already in there
            if (seen.Add(entry.Scenario)) // only add if not already present
                results.Add(entry.Scenario);
        }

        // Step 2: Fill out remaining from all scenarios if needed
        foreach (var entry in AllScenarios)
        {
            if (results.Count >= workingMemoryScenarioCapacity) break;

            if (seen.Add(entry.Scenario))
                results.Add(entry.Scenario);
        }

        return results;

    }

    //Fixation scenario time is the amount of fixation a character has on a scenario before it isn't recent and removes it
    public void UpdateRecentScenarios(float fixatationScenarioTime = 10f)
    {
        float currentTime = Time.time;
        RecentScenarios.RemoveAll(entry => currentTime - entry.LastConsideredTime > fixatationScenarioTime); 
    }

}



public class CharacterPsyche
{
    //[Header("Decision Making Variables")]
    public RelationshipPersonalTree RelationshipPersonalTree;
    private SubIdentifierNode SelfIdentifier;
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
    //Consideration of abstract concepts vs. concrete concepts
    public double AbstractInclination;
    //how many actions they can decide
    public int CognitiveStamina;
    public List<RelationshipDecisionNode> L_LearnedResponseDecisions;
    public List<RelationshipDecisionNode> NB_LearnedResponseDecisions;
    public List<RelationshipDecisionNode> DB_LearnedResponseDecisions;
    public ScenarioMemory ScenarioMemoryBank = new ScenarioMemory();
    public List<RelationshipNode> L_LearnedScenarios;
    public List<RelationshipNode> NB_LearnedScenarios;
    public List<RelationshipNode> DB_LearnedScenarios;
    public Dictionary<DecisionSO, int> Decision_Step_Tracker = new Dictionary<DecisionSO, int>();

    //[Header("Identifier Script Variables")]
    public double ProcessingSpeed;
    public float AwarenessLevel;
    public float DistinctiveAbility;
    public float ExistingNodesDistinctiveAbility;
    public float JudgementLevel;
    public float ExtrapolationLevel;
    public float GeneralizationLevel;

    public CharacterPsyche(CharacterPsycheSO characterPsycheSO)
    {
        RelationshipPersonalTree = new RelationshipPersonalTree(characterPsycheSO.RelationshipPersonalTreeSO);
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
        AbstractInclination = characterPsycheSO.AbstractInclination;
        PerspectiveAbility = characterPsycheSO.PerspectiveAbility;
        CognitiveStamina = characterPsycheSO.CognitiveStamina;
        L_LearnedResponseDecisions = characterPsycheSO.L_LearnedResponseDecisions;
        DB_LearnedResponseDecisions = characterPsycheSO.DB_LearnedResponseDecisions;
        NB_LearnedResponseDecisions = characterPsycheSO.NB_LearnedResponseDecisions;
        ScenarioMemoryBank = characterPsycheSO.ScenarioMemoryBank;
        L_LearnedScenarios = characterPsycheSO.L_LearnedScenarios;
        DB_LearnedScenarios = characterPsycheSO.DB_LearnedScenarios;
        NB_LearnedScenarios = characterPsycheSO.NB_LearnedScenarios;

        //Identifier Script Variables
        ProcessingSpeed = characterPsycheSO.ProcessingSpeed;
        AwarenessLevel = characterPsycheSO.AwarenessLevel;
        DistinctiveAbility = characterPsycheSO.DistinctiveAbility;
        ExistingNodesDistinctiveAbility = characterPsycheSO.ExistingNodesDistinctiveAbility;
        JudgementLevel = characterPsycheSO.JudgementLevel;
        ExtrapolationLevel = characterPsycheSO.ExtrapolationLevel;
        GeneralizationLevel = characterPsycheSO.GeneralizationLevel;

        SelfIdentifier = RelationshipPersonalTree.SelfSubIdentifier;
    }

    public SubIdentifierNode GetSelfSubIdentifier() { 
        return SelfIdentifier;
    }
}
