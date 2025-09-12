using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AllStats
{
    public double L;
    public double DB;
    public double NB;
    public double NL;
    public double NDB;
    public double NNB;

    public AllStats(double l, double db, double nb, double nl, double ndb, double nnb)
    {
        L = l;
        DB = db;
        NB = nb;
        NL = nl;
        NDB = ndb;
        NNB = nnb;
    }

    public double StatOfInterest(string stat)
    {
        if(stat == "L(A)")
        {
            return L;
        }else if(stat == "DB(A)")
        {
            return DB;
        }else if(stat == "NB(A)")
        {
            return NB;
        }else if(stat == "L(N)")
        {
            return NL;
        }else if(stat == "DB(N)")
        {
            return NDB;
        }else if(stat == "NB(N)")
        {
            return NNB;
        }

        throw new ArgumentException($"Invalid stat input: {stat}", nameof(stat));
    }

    public double StatOfInterest(EnumPersonalityStats stat)
    {
        if (stat == EnumPersonalityStats.L)
        {
            return L;
        }
        else if (stat == EnumPersonalityStats.DB)
        {
            return DB;
        }
        else if (stat == EnumPersonalityStats.NB)
        {
            return NB;
        }
        else if (stat == EnumPersonalityStats.NL)
        {
            return NL;
        }
        else if (stat == EnumPersonalityStats.NDB)
        {
            return NDB;
        }
        else if (stat == EnumPersonalityStats.NNB)
        {
            return NNB;
        }

        throw new ArgumentException($"Invalid stat input: {stat}", nameof(stat));
    }
}

public enum CraveType
{
    Si,
    Se,
    Ne,
    Ni
}

public class DecisionMaking : MonoBehaviour
{
    private CharacterMainCPort selfMainCPort;

    private double externalMotivationCutoff = 30;
    private double timePassed = 0;


    private void Start()
    {
        selfMainCPort = GetComponent<CharacterMainCPort>();
    }

    

    //Will normally character pass parameters to this with the psyche. Implement later. Have this in the inspector because of testing
    //This dictionary is passing the env MainCPort and the RelationshipNode with the action it is committing
    public void ActionSelection(Dictionary<CharacterMainCPort, SubIdentifierRelationshipNodeInfo> envCPortToSubIdMap)
    {
        double ultimateLargestPositivePredictorValue = double.MinValue;
        double ultimateLargestPositivePredictorChange = double.MinValue;
        double ultimateLargestNegativePredictorValue = double.MaxValue;
        double ultimateLargestNegativePredictorChange = double.MaxValue;
        EnumPersonalityStats ultimatePositiveTargetStatType = EnumPersonalityStats.None;
        EnumPersonalityStats ultimateNegativeTargetStatType = EnumPersonalityStats.None;
        bool isUltimateActionNe = false;
        bool isUltimateActionNi = false;
        bool isSafeEnough = false;
        bool isRewardingEnough = false;
        RelationshipNode ultimateNePositiveRelationshipNode = null;
        RelationshipNode ultimateNeNegativeDecisionNode = null;
        RelationshipDecisionNode ultimateNiPositiveDecisionNode = null;
        RelationshipDecisionNode ultimateNiNegativeDecisionNode = null;

        CharacterPsyche selfPsyche = selfMainCPort.characterPsyche;
        CharacterPhysical selfPhysical = selfMainCPort.characterPhysical;

        //Go through all the env
        foreach (CharacterMainCPort envMainCPort in envCPortToSubIdMap.Keys)
        {     
            //1. Cohesive Planning
            //2. Blurred Planning
            //3. Incohesive Planning

            //Do I not use the env Subidentifier node?? It doesn't seem like it...
            SubIdentifierNode envSubIdentifier = envCPortToSubIdMap[envMainCPort].SubIdentifierNode;
            RelationshipNode envRelationshipNode = envCPortToSubIdMap[envMainCPort].RelationshipNode;
            //COHESIVE PLANNING
            //Si - Ne, Se - Ni
            //i.e. I am hungry, I'm thinking about the kitchen (Si - Ne)
            //i.e. There is a threat, I will fight it (Se - Ni)

            //1. Find whether crave is internal or external (Si or Se)
            //2. Find the appropriate response (Ne or Ni)
            //a. Utilize Planning Flexibility Stat to determine whether stick with appropriate S - N pairings or not
            //3. Pick the best decision

            //1.
            //Future: statOfinterest will go beyond L, DB, NB, and could be friend, env, or other stats (this means will consider effects on others)
            //Internal
            (EnumPersonalityStats lowestSiStatType, double lowestSiValue) = DecisionMakingFunctions.FindStatOfInterest(selfMainCPort, envMainCPort, envRelationshipNode);

            //External: Largest env change
            double[] allSeModRValues =
            {
                //scale with survival
                DMCalculationFunctions.ScaleSurvivalStatChange(envRelationshipNode.ModRValues.LivelihoodValue, selfPhysical.Stats.L),
                DMCalculationFunctions.ScaleSurvivalStatChange(envRelationshipNode.ModRValues.DefensiveBelongingValue, selfPhysical.Stats.DB),
                DMCalculationFunctions.ScaleSurvivalStatChange(envRelationshipNode.ModRValues.NurtureBelongingValue, selfPhysical.Stats.NB)
            };

            //Ni largest change
            double niLChange = selfMainCPort.characterPsyche.L_LearnedResponseDecisions.Count > 0 ? 
                DMCalculationFunctions.ScaleSurvivalStatChange(selfPsyche.L_LearnedResponseDecisions.First().ModRValues.LivelihoodValue, selfPhysical.Stats.L): 0;
            double niDBChange = selfMainCPort.characterPsyche.DB_LearnedResponseDecisions.Count > 0 ?
                DMCalculationFunctions.ScaleSurvivalStatChange(selfPsyche.DB_LearnedResponseDecisions.First().ModRValues.DefensiveBelongingValue, selfPhysical.Stats.DB) : 0;
            double niNBChange = selfMainCPort.characterPsyche.NB_LearnedResponseDecisions.Count > 0 ? 
                DMCalculationFunctions.ScaleSurvivalStatChange(selfPsyche.NB_LearnedResponseDecisions.First().ModRValues.NurtureBelongingValue, selfPhysical.Stats.NB) : 0;

            double[] allNiModRValues =
            {
                niLChange,
                niDBChange,
                niNBChange
            };

            double largestNiChange = System.Math.Abs(allNiModRValues.OrderByDescending(v => System.Math.Abs(v)).First());
            ScenarioMemory scenarioMemoryBank = selfMainCPort.characterPsyche.ScenarioMemoryBank;
            //largest Ne Change scaled with survival
            List<RelationshipNode> l_LearnedScenario = scenarioMemoryBank.GetConsideredScenarios(EnumPersonalityStats.L, 1);
            List<RelationshipNode> db_LearnedScenario = scenarioMemoryBank.GetConsideredScenarios(EnumPersonalityStats.DB, 1);
            List<RelationshipNode> nb_LearnedScenario = scenarioMemoryBank.GetConsideredScenarios(EnumPersonalityStats.NB, 1);

            double neLChange = l_LearnedScenario.Count > 0 ? 
                DMCalculationFunctions.ScaleSurvivalStatChange(l_LearnedScenario.First().ModRValues.LivelihoodValue, selfPhysical.Stats.L) : 0;
            double neDBChange = db_LearnedScenario.Count > 0 ? 
                DMCalculationFunctions.ScaleSurvivalStatChange(db_LearnedScenario.First().ModRValues.DefensiveBelongingValue, selfPhysical.Stats.DB) : 0;
            double neNBChange = nb_LearnedScenario.Count > 0 ? 
                DMCalculationFunctions.ScaleSurvivalStatChange(nb_LearnedScenario.First().ModRValues.NurtureBelongingValue, selfPhysical.Stats.NB) : 0;
            
            var allNeModRValues = new Dictionary<EnumPersonalityStats, double>
            {
                { EnumPersonalityStats.L ,neLChange },
                {EnumPersonalityStats.DB, neDBChange },
                {EnumPersonalityStats.NB, neNBChange }
            };

            //Get the largest change of value
            //Aggregate compares each 2 and slides window to the right
            var largestNeChangeDict = allNeModRValues.Aggregate((l, r) => Math.Abs(l.Value) > Math.Abs(r.Value) ? l : r);

            double largestNeChange = largestNeChangeDict.Value;
            EnumPersonalityStats largestNePersonalityStat = largestNeChangeDict.Key;

            //Get largest change (whether positive or negative)
            double largestSeChange = System.Math.Abs(allSeModRValues.OrderByDescending(v => System.Math.Abs(v)).First());
            
            //Note: Even if considering env stats, will be considered internal
            //i.e. Friend's hunger is internal and Friend yelling at you is external
            //If internal, you are more worried about your friend being hungry rather than them yelling at you right now
            //Whether your hunger or friend's hunger focus should be based on empathy

            //can delete isInternalCrave (changed to craveType)
            bool isInternalCrave = DecisionMakingFunctions.IsInternalCrave(selfMainCPort.characterPsyche.InternalMotivationLevel, lowestSiValue, largestSeChange, externalMotivationCutoff);
            CraveType craveType = DecisionMakingFunctions.DetermineCraveType(lowestSiValue, largestNiChange, largestSeChange, largestNeChange, selfMainCPort.characterPsyche.InternalMotivationLevel, 
                selfMainCPort.characterPsyche.AbstractInclination, externalMotivationCutoff);
            DebugManager.Instance?.SetActionSelectionDebugValue("Crave Type: ", craveType.ToString());
            DebugManager.Instance?.SetActionSelectionDebugValue("Target stat type", lowestSiStatType);
            //2. Find the appropriate response (Ne or Ni) 

            //Get all the Decisions (done before) based on the env Relationship Node
            List<RelationshipDecisionNode> niDecisionNodes = null;
            List<RelationshipNode> neRelationshipNodes = null;

            //Getting the options from the crave
            if (craveType == CraveType.Si)
            {
                //Ne

                //To-Do: Incorporate Planning Flexibility Stat to not always be Ne (or Ni if external crave)
                //To-Do: Figure out what to do if internal crave is Env's stats (i.e. my friend has low DB)

                

                if (lowestSiStatType == EnumPersonalityStats.L || lowestSiStatType == EnumPersonalityStats.NL)
                {
                    neRelationshipNodes = scenarioMemoryBank.GetConsideredScenarios(EnumPersonalityStats.L);
                }
                else if (lowestSiStatType == EnumPersonalityStats.DB || lowestSiStatType == EnumPersonalityStats.NDB)
                {
                    neRelationshipNodes = scenarioMemoryBank.GetConsideredScenarios(EnumPersonalityStats.DB);
                }
                else if (lowestSiStatType == EnumPersonalityStats.NB || lowestSiStatType == EnumPersonalityStats.NNB)
                {
                    neRelationshipNodes = scenarioMemoryBank.GetConsideredScenarios(EnumPersonalityStats.NB);
                }

            }
            //External crave
            else if (craveType == CraveType.Se)
            {
                //Ni
                niDecisionNodes = envRelationshipNode.ResponseNodes;
            }
            
            //Ne - Ni for now
            else if (craveType == CraveType.Ne)
            {
                //Get all the relevant Ne of interest. Then get all the action plan nodes for them
                List<RelationshipNode> neImagedScenario = scenarioMemoryBank.GetConsideredScenarios(largestNePersonalityStat);
                foreach(RelationshipNode neScenario in neImagedScenario)
                {
                    if(neScenario.ActionPlanNodes != null && neScenario.ActionPlanNodes.Count > 0)
                    {
                        if(niDecisionNodes == null)
                        {
                            niDecisionNodes = new List<RelationshipDecisionNode>();
                        }
                        niDecisionNodes.AddRange(neScenario.ActionPlanNodes);
                    }
                }
            }

                //Action Selection Logic 

                //Si - Ne
            if (craveType == CraveType.Si)
            {
                //TO-DO: Will start to pass incoherent and blurry planning which will require changes in implementation (since not all of the time it is Si)

                Stats selfStats = selfMainCPort.characterPhysical.Stats;
                Stats envStats = envMainCPort.characterPhysical.Stats;
                AllStats allInitialStats = new AllStats(selfStats.L, selfStats.DB, selfStats.NB, envStats.L, envStats.DB, envStats.NB);

                //Original return: (largestPositivePredictorValue, largestPositivePredictorChange, targetStatType, neDecisionNode)
                //Accounts for habits, opportunism, risk aversion, and reward cutoff
                ReturnDecision returnSiNeDecision = DecisionMakingFunctions.CalculateSiNeDecisions(neRelationshipNodes, lowestSiStatType, allInitialStats, selfMainCPort.characterPsyche);

                //Commit action
                //To-Do: Make this more abstracted so i'm not setting this manually (instead it will just be a function)
                if (returnSiNeDecision.WillCommitAction)
                {
                    if (returnSiNeDecision.LargestPositivePredictorChange > ultimateLargestPositivePredictorChange)
                    {
                        ultimateLargestPositivePredictorChange = returnSiNeDecision.LargestPositivePredictorChange;
                        ultimateLargestPositivePredictorValue = returnSiNeDecision.LargestPositivePredictorValue;
                        ultimatePositiveTargetStatType = returnSiNeDecision.TargetPositiveStatOfInterest;
                        ultimateNePositiveRelationshipNode = returnSiNeDecision.NePositiveDecision;
                        isUltimateActionNe = true;
                        selfMainCPort.characterPsyche.ScenarioMemoryBank.AddScenario(returnSiNeDecision.NePositiveDecision);
                        isSafeEnough = returnSiNeDecision.IsSafeEnough;
                        DebugManager.Instance?.SetActionSelectionDebugValue("Planning Style", "Safe and Rewarding Si-Ne");
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                        DebugManager.Instance?.SetActionSelectionDebugValue("Ne Positive Relationship Node", ultimateNePositiveRelationshipNode.Name);
                    }
                }


                //Risky action
                if (!returnSiNeDecision.IsSafeEnough)
                {
                    if (returnSiNeDecision.LargestNegativePredictorChange < ultimateLargestNegativePredictorChange)
                    {
                        ultimateLargestNegativePredictorChange = returnSiNeDecision.LargestNegativePredictorChange;
                        ultimateLargestNegativePredictorValue = returnSiNeDecision.LargestNegativePredictorValue;
                        ultimateNegativeTargetStatType = returnSiNeDecision.TargetNegativeStatOfInterest;
                        ultimateNeNegativeDecisionNode = returnSiNeDecision.NeNegativeDecision;
                        isSafeEnough = false;
                        DebugManager.Instance?.SetActionSelectionDebugValue("Planning Style", "Too Risky Si-Ne");
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                        DebugManager.Instance?.SetActionSelectionDebugValue("Ne Positive Relationship Node", ultimateNePositiveRelationshipNode.Name);
                    }
                }

                //Not rewarding action
                if (!returnSiNeDecision.IsRewardingEnough)
                {
                    //Could add more descriptors
                    ultimateLargestPositivePredictorChange = returnSiNeDecision.LargestPositivePredictorChange;
                    ultimateLargestPositivePredictorValue = returnSiNeDecision.LargestPositivePredictorValue;
                    ultimatePositiveTargetStatType = returnSiNeDecision.TargetPositiveStatOfInterest;
                    ultimateNePositiveRelationshipNode = returnSiNeDecision.NePositiveDecision;
                    isRewardingEnough = false;
                    DebugManager.Instance?.SetActionSelectionDebugValue("Planning Style", "No Rewards Available for Si-Ne");
                    DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                    DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                    DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                    DebugManager.Instance?.SetActionSelectionDebugValue("Ne Positive Relationship Node", ultimateNePositiveRelationshipNode.Name);
                }
            }
            //Se - Ni

            //Should have action selection be decision be Ni
            else if (craveType == CraveType.Se)
            {
                //1. Pass through the action nodes and crave stat
                //2. Get the largest change from the crave stat with function
                //3. Update the ultimate values if larger than current
                //TODO: Will start to pass incoherent and blurry planning which will require changes in implementation (since not all of the time it is Ni)
                Stats selfStats = selfMainCPort.characterPhysical.Stats;
                Stats envStats = envMainCPort.characterPhysical.Stats;

                AllStats allInitialStats = new AllStats(selfStats.L, selfStats.DB, selfStats.NB, envStats.L, envStats.DB, envStats.NB);
                ReturnDecision returnSeNiDecision = DecisionMakingFunctions.CalculateSeNiDecisions(niDecisionNodes, lowestSiStatType, allInitialStats, selfMainCPort, envMainCPort, envRelationshipNode);


                if (returnSeNiDecision.IsNiDecision)
                {
                    //Commit action
                    if (returnSeNiDecision.WillCommitAction)
                    {
                        if (returnSeNiDecision.LargestPositivePredictorChange > ultimateLargestPositivePredictorChange)
                        {
                            ultimateLargestPositivePredictorChange = returnSeNiDecision.LargestPositivePredictorChange;
                            ultimateLargestPositivePredictorValue = returnSeNiDecision.LargestPositivePredictorValue;
                            ultimateLargestNegativePredictorValue = returnSeNiDecision.LargestNegativePredictorValue;
                            ultimateLargestNegativePredictorChange = returnSeNiDecision.LargestNegativePredictorChange;
                            ultimatePositiveTargetStatType = returnSeNiDecision.TargetPositiveStatOfInterest;
                            ultimateNiPositiveDecisionNode = returnSeNiDecision.NiPositiveDecision;
                            isUltimateActionNi = true;
                            isSafeEnough = returnSeNiDecision.IsSafeEnough;
                            isRewardingEnough = returnSeNiDecision.IsRewardingEnough;
                            DebugManager.Instance?.SetActionSelectionDebugValue("Planning Style", "Safe and Reward Se-Ni");
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Negative Predictor Change", ultimateLargestNegativePredictorChange);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Negative Predictor Value", ultimateLargestNegativePredictorValue);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                            DebugManager.Instance?.SetActionSelectionDebugValue("Ni Positive Decision Node", ultimateNiPositiveDecisionNode.Decision.Name);
                        }
                    }


                    //Risky action
                    if (!returnSeNiDecision.IsSafeEnough)
                    {
                        if (returnSeNiDecision.LargestNegativePredictorChange < ultimateLargestNegativePredictorChange)
                        {
                            ultimateLargestPositivePredictorChange = returnSeNiDecision.LargestPositivePredictorChange;
                            ultimateLargestPositivePredictorValue = returnSeNiDecision.LargestPositivePredictorValue;
                            ultimateLargestNegativePredictorChange = returnSeNiDecision.LargestNegativePredictorChange;
                            ultimateLargestNegativePredictorValue = returnSeNiDecision.LargestNegativePredictorValue;
                            ultimatePositiveTargetStatType = returnSeNiDecision.TargetPositiveStatOfInterest;
                            ultimateNegativeTargetStatType = returnSeNiDecision.TargetNegativeStatOfInterest;
                            ultimateNiPositiveDecisionNode = returnSeNiDecision.NiPositiveDecision;
                            ultimateNiNegativeDecisionNode = returnSeNiDecision.NiNegativeDecision;
                            isSafeEnough = false;
                            isUltimateActionNi = true;
                            DebugManager.Instance?.SetActionSelectionDebugValue("Planning Style", "Too Risky Se-Ni");
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Negative Predictor Change", ultimateLargestNegativePredictorChange);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Negative Predictor Value", ultimateLargestNegativePredictorValue);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                            DebugManager.Instance?.SetActionSelectionDebugValue("Ni Positive Decision Node", ultimateNiPositiveDecisionNode.Decision.Name);
                        }
                    }

                    //Not rewarding action
                    if (!returnSeNiDecision.IsRewardingEnough)
                    {
                        //Could add more descriptors
                        ultimateLargestPositivePredictorChange = returnSeNiDecision.LargestPositivePredictorChange;
                        ultimateLargestPositivePredictorValue = returnSeNiDecision.LargestPositivePredictorValue;
                        ultimatePositiveTargetStatType = returnSeNiDecision.TargetPositiveStatOfInterest;
                        ultimateNiPositiveDecisionNode = returnSeNiDecision.NiPositiveDecision;
                        isRewardingEnough = false;
                        //Debug.Log("No rewarding decisions found for Si - Ne action selection.");
                        DebugManager.Instance?.SetActionSelectionDebugValue("Planning Style", "No rewarding decisions found for Se-Ni");
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                        DebugManager.Instance?.SetActionSelectionDebugValue("Ni Positive Decision Node", ultimateNiPositiveDecisionNode.Decision.Name);
                    }
                }
                else
                {
                    Debug.LogWarning("Se - Ni decision node is not a Ni decision. This should not happen in the current implementation.");
                }

            }

        }


        //Can test final output
        //Final output

        //Ni
        if (isUltimateActionNi)
        {
                Debug.Log("Ultimate largest positive predictor value: " + ultimateLargestPositivePredictorValue +
                " Ultimate largest positive predictor change: " + ultimateLargestPositivePredictorChange +
                " Ultimate target stat type: " + ultimatePositiveTargetStatType +
                " Is ultimate action Ni: " + isUltimateActionNi +
                " Is safe enough: " + isSafeEnough +
                " Is rewarding enough: " + isRewardingEnough +
                " Ultimate Ni decision node: " + (ultimateNiPositiveDecisionNode != null ? ultimateNiPositiveDecisionNode.Decision.name : "null"));
        }

        if (isUltimateActionNe)
        {
            Debug.Log("Ultimate largest positive predictor value: " + ultimateLargestPositivePredictorValue +
            " Ultimate largest positive predictor change: " + ultimateLargestPositivePredictorChange +
            " Ultimate target stat type: " + ultimatePositiveTargetStatType +
            " Is ultimate action Ne: " + isUltimateActionNe +
            " Is safe enough: " + isSafeEnough +
            " Is rewarding enough: " + isRewardingEnough +
            " Ultimate Ne decision node: " + (ultimateNePositiveRelationshipNode != null ? ultimateNePositiveRelationshipNode.Name : "null"));
        }
            

    }


    }

    