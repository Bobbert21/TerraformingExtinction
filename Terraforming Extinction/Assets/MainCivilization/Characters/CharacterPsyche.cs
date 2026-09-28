using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class DecisionMemoryEntry
{
    public float LastConsideredTime;
    public NiDecisionNode Decision;

    public DecisionMemoryEntry(NiDecisionNode decision)
    {
        Decision = decision;
        LastConsideredTime = Time.time;
    }
}

[System.Serializable]
public class DecisionMemory
{
    public List<DecisionMemoryEntry> AllDecisions = new List<DecisionMemoryEntry>();
    public List<DecisionMemoryEntry> RecentDecisions = new List<DecisionMemoryEntry>();
    public List<DecisionMemoryEntry> LRecentDecisions = new List<DecisionMemoryEntry>();
    public List<DecisionMemoryEntry> NBRecentDecisions = new List<DecisionMemoryEntry>();
    public List<DecisionMemoryEntry> DBRecentDecisions = new List<DecisionMemoryEntry>();
    public List<DecisionMemoryEntry> LDecisions = new List<DecisionMemoryEntry>();
    public List<DecisionMemoryEntry> NBDecisions = new List<DecisionMemoryEntry>();
    public List<DecisionMemoryEntry> DBDecisions = new List<DecisionMemoryEntry>();

    public void AddScenario(NiDecisionNode scenario, EnumPersonalityStats scenarioOfInterest = EnumPersonalityStats.None)
    {
        DecisionMemoryEntry entry = new DecisionMemoryEntry(scenario);

        // Add in order of habit counter for all scenarios
        int index = AllDecisions.FindIndex(e => e.Decision.HabitCounter < entry.Decision.HabitCounter);
        if (index >= 0)
            AllDecisions.Insert(index, entry);
        else
            AllDecisions.Add(entry);

        if (scenarioOfInterest == EnumPersonalityStats.L)
        {
            index = LDecisions.FindIndex(e => e.Decision.HabitCounter < entry.Decision.HabitCounter);
            if (index >= 0)
                LDecisions.Insert(index, entry);
            else
                LDecisions.Add(entry);

            LRecentDecisions.Add(entry);
        }
        else if (scenarioOfInterest == EnumPersonalityStats.NB)
        {
            index = NBDecisions.FindIndex(e => e.Decision.HabitCounter < entry.Decision.HabitCounter);
            if (index >= 0)
                NBDecisions.Insert(index, entry);
            else
                NBDecisions.Add(entry);

            NBRecentDecisions.Add(entry);
        }
        else if (scenarioOfInterest == EnumPersonalityStats.DB)
        {
            index = DBDecisions.FindIndex(e => e.Decision.HabitCounter < entry.Decision.HabitCounter);
            if (index >= 0)
                DBDecisions.Insert(index, entry);
            else
                DBDecisions.Add(entry);

            DBRecentDecisions.Add(entry);
        }

        RecentDecisions.Add(entry);
    }

    // Return a list of scenarios that the character can consider
    // Prioritizing the most recent scenarios first then grab rest from all scenarios
    public List<NiDecisionNode> GetConsideredScenarios(EnumPersonalityStats scenarioOfInterest = EnumPersonalityStats.None, float workingMemoryScenarioCapacity = 3, float fixatationScenarioTime = 50f)
    {
        UpdateRecentScenarios(fixatationScenarioTime);

        var results = new List<NiDecisionNode>();
        var seen = new HashSet<NiDecisionNode>(); // avoid duplicates

        List<DecisionMemoryEntry> AllScenariosOfInterest = AllDecisions;
        List<DecisionMemoryEntry> RecentScenariosOfInterest = RecentDecisions;

        if (scenarioOfInterest == EnumPersonalityStats.L)
        {
            AllScenariosOfInterest = LDecisions;
            RecentScenariosOfInterest = LRecentDecisions;
        }
        else if (scenarioOfInterest == EnumPersonalityStats.NB)
        {
            AllScenariosOfInterest = NBDecisions;
            RecentScenariosOfInterest = NBRecentDecisions;
        }
        else if (scenarioOfInterest == EnumPersonalityStats.DB)
        {
            AllScenariosOfInterest = DBDecisions;
            RecentScenariosOfInterest = DBRecentDecisions;
        }

        // Step 1: take from RecentScenarios
        foreach (var entry in RecentScenariosOfInterest)
        {
            if (results.Count >= workingMemoryScenarioCapacity) break;

            // Only true if scenario isn't already in there
            if (seen.Add(entry.Decision)) // only add if not already present
                results.Add(entry.Decision);
        }

        // Step 2: Fill out remaining from all scenarios if needed
        foreach (var entry in AllScenariosOfInterest)
        {
            if (results.Count >= workingMemoryScenarioCapacity) break;

            if (seen.Add(entry.Decision))
                results.Add(entry.Decision);
        }

        return results;
    }

    // Fixation scenario time is the amount of fixation a character has on a scenario before it isn't recent and removes it
    public void UpdateRecentScenarios(float fixatationScenarioTime = 10f)
    {
        float currentTime = Time.time;
        RecentDecisions.RemoveAll(entry => currentTime - entry.LastConsideredTime > fixatationScenarioTime);
        LRecentDecisions.RemoveAll(entry => currentTime - entry.LastConsideredTime > fixatationScenarioTime);
        NBRecentDecisions.RemoveAll(entry => currentTime - entry.LastConsideredTime > fixatationScenarioTime);
        DBRecentDecisions.RemoveAll(entry => currentTime - entry.LastConsideredTime > fixatationScenarioTime);
    }
}


[System.Serializable]
public class ScenarioMemoryEntry
{
    public float LastConsideredTime;
    public NeScenarioNode Scenario;

    public ScenarioMemoryEntry(NeScenarioNode scenario)
    {
        Scenario = scenario;
        LastConsideredTime = Time.time;
    }
}


[System.Serializable]
//These are for craves
public class ScenarioMemory
{
    public List<ScenarioMemoryEntry> AllScenarios = new List<ScenarioMemoryEntry>();
    public List<ScenarioMemoryEntry> RecentScenarios = new List<ScenarioMemoryEntry>();
    public List<ScenarioMemoryEntry> LRecentScenarios = new List<ScenarioMemoryEntry>();
    public List<ScenarioMemoryEntry> NBRecentScenarios = new List<ScenarioMemoryEntry>();
    public List<ScenarioMemoryEntry> DBRecentScenarios = new List<ScenarioMemoryEntry>();
    public List<ScenarioMemoryEntry> LScenarios = new List<ScenarioMemoryEntry>();
    public List<ScenarioMemoryEntry> NBScenarios = new List<ScenarioMemoryEntry>();
    public List<ScenarioMemoryEntry> DBScenarios = new List<ScenarioMemoryEntry>();

    public void AddScenario(NeScenarioNode scenario, EnumPersonalityStats scenarioOfInterest = EnumPersonalityStats.None)
    {
        ScenarioMemoryEntry entry = new ScenarioMemoryEntry(scenario);

        //Add in order of habit counter for all scenario
        int index = AllScenarios.FindIndex(e => e.Scenario.HabitCounter < entry.Scenario.HabitCounter);
        if (index >= 0)
            AllScenarios.Insert(index, entry);
        else
            AllScenarios.Add(entry);

        if(scenarioOfInterest == EnumPersonalityStats.L)
        {
            index = LScenarios.FindIndex(e => e.Scenario.HabitCounter < entry.Scenario.HabitCounter);
            if (index >= 0)
                LScenarios.Insert(index, entry);
            else
                LScenarios.Add(entry);

            LRecentScenarios.Add(entry);
        }
        else if(scenarioOfInterest == EnumPersonalityStats.NB)
        {
            index = NBScenarios.FindIndex(e => e.Scenario.HabitCounter < entry.Scenario.HabitCounter);
            if (index >= 0)
                NBScenarios.Insert(index, entry);
            else
                NBScenarios.Add(entry);

            NBRecentScenarios.Add(entry);
        }
        else if(scenarioOfInterest == EnumPersonalityStats.DB)
        {
            index = DBScenarios.FindIndex(e => e.Scenario.HabitCounter < entry.Scenario.HabitCounter);
            if (index >= 0)
                DBScenarios.Insert(index, entry);
            else
                DBScenarios.Add(entry);

            DBRecentScenarios.Add(entry);
        }

        RecentScenarios.Add(entry);
    }

    // Return a list of scenarios that the character can consider
    // Prioritizing the most recent scenarios first then grab rest from all scenarios
    public List<NeScenarioNode> GetConsideredScenarios(EnumPersonalityStats scenarioOfInterest = EnumPersonalityStats.None, float workingMemoryScenarioCapacity = 3, float fixatationScenarioTime = 50f)
    {
        UpdateRecentScenarios(fixatationScenarioTime);

        var results = new List<NeScenarioNode>();
        var seen = new HashSet<NeScenarioNode>(); // avoid duplicates

        List<ScenarioMemoryEntry> AllScenariosOfInterest = AllScenarios;
        List<ScenarioMemoryEntry> RecentScenariosOfInterest = RecentScenarios;

        if (scenarioOfInterest == EnumPersonalityStats.L)
        {
            AllScenariosOfInterest = LScenarios;
            RecentScenariosOfInterest = LRecentScenarios;
        }
        else if(scenarioOfInterest == EnumPersonalityStats.NB)
        {
            AllScenariosOfInterest = NBScenarios;
            RecentScenariosOfInterest = NBRecentScenarios;
        }
        else if(scenarioOfInterest == EnumPersonalityStats.DB)
        {
            AllScenariosOfInterest = DBScenarios;
            RecentScenariosOfInterest = DBRecentScenarios;
        }

        // Step 1: take from RecentScenarios
        foreach (var entry in RecentScenariosOfInterest)
        {
            if (results.Count >= workingMemoryScenarioCapacity) break;

            //Only true if scenario isn't already in there
            if (seen.Add(entry.Scenario)) // only add if not already present
                results.Add(entry.Scenario);
        }

        // Step 2: Fill out remaining from all scenarios if needed
        foreach (var entry in AllScenariosOfInterest)
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
        LRecentScenarios.RemoveAll(entry => currentTime - entry.LastConsideredTime > fixatationScenarioTime);
        NBRecentScenarios.RemoveAll(entry => currentTime - entry.LastConsideredTime > fixatationScenarioTime);
        DBRecentScenarios.RemoveAll(entry => currentTime - entry.LastConsideredTime > fixatationScenarioTime);
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
    public double PlanningFlexibility;
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
    public List<InstinctSO> Instincts;

    //Adjust the instinct cutoff
    public double ImpulsiveControlLevel;
    //Adjust the instinct personality trigger value
    public double ImpulsiveInclinationLevel;

    //Learning Variables
    public double ExpectationLearningThreshold;
    public double GroundedLearningThreshold;

    public DecisionMemory DecisionMemoryBank = new DecisionMemory();
    public ScenarioMemory ScenarioMemoryBank = new ScenarioMemory();
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
        PlanningFlexibility = characterPsycheSO.PlanningFlexibility;
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
        DecisionMemoryBank = characterPsycheSO.DecisionMemoryBank;
        ScenarioMemoryBank = characterPsycheSO.ScenarioMemoryBank;

        //Clone the instincts instead of monobehavior inheritance
        Instincts = new List<InstinctSO>();
        foreach (var instinct in characterPsycheSO.Instincts)
        {
            if (instinct == null) continue;
            var clone = Object.Instantiate(instinct);   // <— Use UnityEngine.Object.Instantiate
            Instincts.Add(clone);
        }

        //Adjust the instinct cutoff
        ImpulsiveControlLevel = characterPsycheSO.ImpulsiveControlLevel;
        //Adjust the instinct personality trigger value
        ImpulsiveInclinationLevel = characterPsycheSO.ImpulsiveInclinationLevel;

        //Learning Variables
        ExpectationLearningThreshold = characterPsycheSO.ExpectationLearningThreshold;
        GroundedLearningThreshold = characterPsycheSO.GroundedLearningThreshold;

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
