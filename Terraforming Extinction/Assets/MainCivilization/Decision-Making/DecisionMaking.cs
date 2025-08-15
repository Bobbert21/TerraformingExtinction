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
        RelationshipNode ultimateNePositiveDecisionNode = null;
        RelationshipNode ultimateNeNegativeDecisionNode = null;
        RelationshipDecisionNode ultimateNiPositiveDecisionNode = null;
        RelationshipDecisionNode ultimateNiNegativeDecisionNode = null;
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
            //i.e. I am hungry, lets go to the kitchen (Si - Ne)
            //i.e. There is a threat, I will fight it (Se - Ni)

            //1. Find whether crave is internal or external (Si or Se)
            //2. Find the appropriate response (Ne or Ni)
            //a. Utilize Planning Flexibility Stat to determine whether stick with appropriate S - N pairings or not
            //3. Pick the best decision

            //1.

            //Internal
            (EnumPersonalityStats targetStatType, double targetStatInitialValue) = DecisionMakingFunctions.FindStatOfInterest(selfMainCPort, envMainCPort, envRelationshipNode);

            //External: Largest env change
            double[] allEnvModRValues =
            {
                envRelationshipNode.ModRValues.LivelihoodValue,
                envRelationshipNode.ModRValues.DefensiveBelongingValue,
                envRelationshipNode.ModRValues.NurtureBelongingValue
            };

            //Get largest change (whether positive or negative)
            double envChange = System.Math.Abs(allEnvModRValues.OrderByDescending(v => System.Math.Abs(v)).First());
            //Note: Even if considering env stats, will be considered internal
            //i.e. Friend's hunger is internal and Friend yelling at you is external
            //If internal, you are more worried about your friend being hungry rather than them yelling at you right now
            //Whether your hunger or friend's hunger focus should be based on empathy

            bool isInternalCrave = DecisionMakingFunctions.IsInternalCrave(selfMainCPort.characterPsyche.InternalMotivationLevel, targetStatInitialValue, envChange, externalMotivationCutoff);

            DebugManager.Instance?.SetActionSelectionDebugValue("Is internal crave (Si - Ne)", isInternalCrave);
            DebugManager.Instance?.SetActionSelectionDebugValue("Target stat type", targetStatType);
            //2. Find the appropriate response (Ne or Ni)

            //Get all the Decisions (done before) based on the env Relationship Node
            List<RelationshipDecisionNode> niResponseNodes = null;
            List<RelationshipNode> neRelationshipNodes = null;

            //Getting the crave
            if (isInternalCrave)
            {
                //Ne

                //To-Do: Incorporate Planning Flexibility Stat to not always be Ne (or Ni if external crave)
                //To-Do: Figure out what to do if internal crave is Env's stats (i.e. my friend has low DB)
                
                if (targetStatType == EnumPersonalityStats.L || targetStatType == EnumPersonalityStats.NL)
                {
                    neRelationshipNodes = selfMainCPort.characterPsyche.L_LearnedEnvironment;
                }
                else if (targetStatType == EnumPersonalityStats.DB || targetStatType == EnumPersonalityStats.NDB)
                {
                    neRelationshipNodes = selfMainCPort.characterPsyche.DB_LearnedEnvironment;
                }
                else if (targetStatType == EnumPersonalityStats.NB || targetStatType == EnumPersonalityStats.NNB)
                {
                    neRelationshipNodes = selfMainCPort.characterPsyche.NB_LearnedEnvironment;
                }
                
            }
            //External crave
            else
            {
                //Ni
                niResponseNodes = envRelationshipNode.ResponseNodes;
            }

            //Action Selection Logic 

            //Si - Ne
            if (isInternalCrave)
            {
                //TO-DO: Will start to pass incoherent and blurry planning which will require changes in implementation (since not all of the time it is Si)

                Stats selfStats = selfMainCPort.characterPhysical.Stats;
                Stats envStats = envMainCPort.characterPhysical.Stats;
                AllStats allInitialStats = new AllStats(selfStats.L, selfStats.DB, selfStats.NB, envStats.L, envStats.DB, envStats.NB);

                //Original return: (largestPositivePredictorValue, largestPositivePredictorChange, targetStatType, neDecisionNode)
                //Accounts for habits, opportunism, risk aversion, and reward cutoff
                ReturnDecision returnSiNeDecision = DecisionMakingFunctions.CalculateSiNeDecisions(neRelationshipNodes, targetStatType, allInitialStats, selfMainCPort.characterPsyche);

                //Commit action
                //To-Do: Make this more abstracted so i'm not setting this manually (instead it will just be a function)
                if (returnSiNeDecision.WillCommitAction)
                {
                    if(returnSiNeDecision.LargestPositivePredictorChange > ultimateLargestPositivePredictorChange){
                        ultimateLargestPositivePredictorChange = returnSiNeDecision.LargestPositivePredictorChange;
                        ultimateLargestPositivePredictorValue = returnSiNeDecision.LargestPositivePredictorValue;
                        ultimatePositiveTargetStatType = returnSiNeDecision.TargetPositiveStatOfInterest;
                        ultimateNePositiveDecisionNode = returnSiNeDecision.NePositiveDecision;
                        isUltimateActionNe = true;
                        isSafeEnough = returnSiNeDecision.IsSafeEnough;
                        DebugManager.Instance?.SetActionSelectionDebugValue("Is safe enough for Si - Ne action selection", "True");
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                        DebugManager.Instance?.SetActionSelectionDebugValue("Ne Positive Decision Node", ultimateNePositiveDecisionNode.Name);
                    }
                }


                //Risky action
                if (!returnSiNeDecision.IsSafeEnough)
                { 
                    if(returnSiNeDecision.LargestNegativePredictorChange < ultimateLargestNegativePredictorChange)
                    {
                        ultimateLargestNegativePredictorChange = returnSiNeDecision.LargestNegativePredictorChange;
                        ultimateLargestNegativePredictorValue = returnSiNeDecision.LargestNegativePredictorValue;
                        ultimateNegativeTargetStatType = returnSiNeDecision.TargetNegativeStatOfInterest;
                        ultimateNeNegativeDecisionNode = returnSiNeDecision.NeNegativeDecision;
                        isSafeEnough = false;
                        DebugManager.Instance?.SetActionSelectionDebugValue("Too risky of decision for Si - Ne action selection", "True");
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                        DebugManager.Instance?.SetActionSelectionDebugValue("Ne Positive Decision Node", ultimateNePositiveDecisionNode.Name);
                    }
                }

                //Not rewarding action
                if(!returnSiNeDecision.IsRewardingEnough)
                {
                    //Could add more descriptors
                    ultimateLargestPositivePredictorChange = returnSiNeDecision.LargestPositivePredictorChange;
                    ultimateLargestPositivePredictorValue = returnSiNeDecision.LargestPositivePredictorValue;
                    ultimatePositiveTargetStatType = returnSiNeDecision.TargetPositiveStatOfInterest;
                    ultimateNePositiveDecisionNode = returnSiNeDecision.NePositiveDecision;
                    isRewardingEnough = false;
                    DebugManager.Instance?.SetActionSelectionDebugValue("No rewarding decisions found for Si - Ne action selection", "True");
                    DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                    DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                    DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                    DebugManager.Instance?.SetActionSelectionDebugValue("Ne Positive Decision Node", ultimateNePositiveDecisionNode.Name);
                }
            }
            //Se - Ni
            else
            {
                //1. Pass through the action nodes and crave stat
                //2. Get the largest change from the crave stat with function
                //3. Update the ultimate values if larger than current
                //TODO: Will start to pass incoherent and blurry planning which will require changes in implementation (since not all of the time it is Ni)
                Stats selfStats = selfMainCPort.characterPhysical.Stats;
                Stats envStats = envMainCPort.characterPhysical.Stats;

                AllStats allInitialStats = new AllStats(selfStats.L, selfStats.DB, selfStats.NB, envStats.L, envStats.DB, envStats.NB);
                ReturnDecision returnSeNiDecision = DecisionMakingFunctions.CalculateSeNiDecisions(niResponseNodes, targetStatType, allInitialStats, selfMainCPort, envMainCPort, envRelationshipNode);
                
                
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
                            DebugManager.Instance?.SetActionSelectionDebugValue("Rewarding decisions found for Se - Ni action selection", "True");
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Negative Predictor Change", ultimateLargestNegativePredictorChange);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Negative Predictor Value", ultimateLargestNegativePredictorValue);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                            DebugManager.Instance?.SetActionSelectionDebugValue("Ni Positive Decision Node", ultimateNiPositiveDecisionNode);
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
                            DebugManager.Instance?.SetActionSelectionDebugValue("Too risky of decisions found for Se - Ni action selection", "True");
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Negative Predictor Change", ultimateLargestNegativePredictorChange);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Largest Negative Predictor Value", ultimateLargestNegativePredictorValue);
                            DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                            DebugManager.Instance?.SetActionSelectionDebugValue("Ni Positive Decision Node", ultimateNiPositiveDecisionNode);
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
                        DebugManager.Instance?.SetActionSelectionDebugValue("No rewarding decisions found for Se - Ni action selection", "True");
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Change", ultimateLargestPositivePredictorChange);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Largest Positive Predictor Value", ultimateLargestPositivePredictorValue);
                        DebugManager.Instance?.SetActionSelectionDebugValue("Positive Target Stat Type", ultimatePositiveTargetStatType.ToString());
                        DebugManager.Instance?.SetActionSelectionDebugValue("Ni Positive Decision Node", ultimateNiPositiveDecisionNode);
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
            " Ultimate Ne decision node: " + (ultimateNePositiveDecisionNode != null ? ultimateNePositiveDecisionNode.Name : "null"));
        }
            

    }


    }

    