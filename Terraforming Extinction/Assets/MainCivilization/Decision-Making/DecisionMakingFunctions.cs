using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ReturnDecision
{
    public double LargestPositivePredictorValue;
    public double LargestNegativePredictorValue;
    public double LargestPositivePredictorChange;
    public double LargestNegativePredictorChange;
    public EnumPersonalityStats TargetPositiveStatOfInterest;
    public EnumPersonalityStats TargetNegativeStatOfInterest;
    public RelationshipNode NePositiveDecision;
    public RelationshipNode NeNegativeDecision;
    public Perspective PositivePerspective;
    public Perspective NegativePerspective;
    public RelationshipDecisionNode NiPositiveDecision;
    public RelationshipDecisionNode NiNegativeDecision;
    public bool IsNiDecision = false;
    public bool IsNeDecision = false;
    public bool WillCommitAction = false;
    public bool IsSafeEnough = true;
    public bool IsRewardingEnough = true;
    //Only for Ne Actions (can have both risky action and positive committed action)
    public bool HasRiskyAction = false;

    public void SetNeValuesWithCommit(double largestPositivePredictorValue, double largestPositivePredictorChange, EnumPersonalityStats targetStatOfInterest, RelationshipNode nePositiveDecision)
    {
        IsNeDecision = true;
        IsNiDecision = false;
        LargestPositivePredictorValue = largestPositivePredictorValue;
        LargestPositivePredictorChange = largestPositivePredictorChange;
        TargetPositiveStatOfInterest = targetStatOfInterest;
        IsRewardingEnough = true;
        IsSafeEnough = true;
        WillCommitAction = DecideCommitAction(IsRewardingEnough, IsSafeEnough);
    }

    public void SetNeValuesWithoutCommitDueToNotRewarding(double largestPositivePredictorValue, double largestPositivePredictorChange, EnumPersonalityStats targetStatOfInterest, RelationshipNode nePositiveDecision)
    {
        IsNeDecision = true;
        IsNiDecision = false;
        LargestPositivePredictorValue = largestPositivePredictorValue;
        LargestPositivePredictorChange = largestPositivePredictorChange;
        TargetPositiveStatOfInterest = targetStatOfInterest;
        NePositiveDecision = nePositiveDecision;
        IsSafeEnough = true;
        IsRewardingEnough = false;
        WillCommitAction = DecideCommitAction(IsRewardingEnough, IsSafeEnough);
    }

    //Ne can return with risky action along with committed positive action
    public void SetNeValuesWithRisk(double largestNegativePredictorValue, double largestNegativePredictorChange, EnumPersonalityStats targetNegativeStatOfInterest,
        RelationshipNode neNegativeDecision)
    {
        HasRiskyAction = true;
        LargestNegativePredictorValue = largestNegativePredictorValue;
        LargestNegativePredictorChange = largestNegativePredictorChange;
        TargetNegativeStatOfInterest = targetNegativeStatOfInterest;
        NeNegativeDecision = neNegativeDecision;
    }

    public void SetNiValuesWithCommit(double largestPositivePredictorValue, double largestPositivePredictorChange, EnumPersonalityStats targetStatOfInterest, RelationshipDecisionNode niDecision)
    {
        IsNeDecision = false;
        IsNiDecision = true;
        LargestPositivePredictorValue = largestPositivePredictorValue;
        LargestPositivePredictorChange = largestPositivePredictorChange;
        TargetPositiveStatOfInterest = targetStatOfInterest;
        NiPositiveDecision = niDecision;
        IsSafeEnough = true;
        IsRewardingEnough = true;
        WillCommitAction = DecideCommitAction(IsRewardingEnough, IsSafeEnough);
    }

    public void SetNiValuesWithoutCommitDueToNotRewarding(double largestPositivePredictorValue, double largestPositivePredictorChange, EnumPersonalityStats targetStatOfInterest, Perspective positivePerspective, RelationshipDecisionNode niPositiveDecision)
    {
        IsNeDecision = false;
        IsNiDecision = true;
        LargestPositivePredictorValue = largestPositivePredictorValue;
        LargestPositivePredictorChange = largestPositivePredictorChange;
        TargetPositiveStatOfInterest = targetStatOfInterest;
        NiPositiveDecision = niPositiveDecision;
        PositivePerspective = positivePerspective;
        IsSafeEnough = true;
        IsRewardingEnough = false;
        WillCommitAction = DecideCommitAction(IsRewardingEnough, IsSafeEnough);
    }

    //Unlike Ni, Risky won't prevent an action. It will however consider a risky action
    public void SetNiValuesWithRisk(double largestPositivePredictorValue, double largestPositivePredictorChange, double largestNegativePredictorValue, 
        double largestNegativePredictorChange, EnumPersonalityStats targetPositiveStatOfInterest, EnumPersonalityStats targetNegativeStatOfInterest, 
        Perspective positivePerspective, Perspective negativePerspective, 
        RelationshipDecisionNode niPositiveDecision, RelationshipDecisionNode niNegativeDecision)
    {
        LargestPositivePredictorValue = largestPositivePredictorValue;
        LargestPositivePredictorChange = largestPositivePredictorChange;
        LargestNegativePredictorValue = largestNegativePredictorValue;
        LargestNegativePredictorChange = largestNegativePredictorChange;
        TargetPositiveStatOfInterest = targetPositiveStatOfInterest;
        TargetNegativeStatOfInterest = targetNegativeStatOfInterest;
        PositivePerspective = positivePerspective;
        NegativePerspective = negativePerspective;
        NiPositiveDecision = niPositiveDecision;
        NiNegativeDecision = niNegativeDecision;
        IsSafeEnough = false;
        WillCommitAction = DecideCommitAction(IsRewardingEnough, IsSafeEnough);
    }

    private bool DecideCommitAction(bool IsRewarding, bool IsSafe)
    {
        if (IsRewarding && IsSafe) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}

public static class DecisionMakingFunctions 
{

    private static double FulcrumStatScale = 30;

    private static readonly Regex ComplexTermRegex = new(
       @"(?<entity>F\((?<entity_index>-?\d+)\)|A|N)-(?<relation>ModR|PR|SR):(?<stat>L|DB|NB|B)\((?<target>E\((?<target_index>-?\d+)\)|N|A|-(?<specific>\w+)-)\)",
       RegexOptions.Compiled);
    //A-ModR:DB(N)

    private static readonly Regex SimpleTermRegex = new(
    @"^(?:(?<stat>L|DB|NB)\((?<target>A|N|F\((?<fIndex>-?\d+)\))\)|E\((?<eIndex>-?\d+)\))$",
    RegexOptions.Compiled);

    private static readonly Regex EmpTermRegex = new(
        @"ScaleChg\((?<initial_value>\d+)\,(?<change_value>\d+)\)",
        RegexOptions.Compiled);

    private static readonly Regex ScaleChgRegex = new(
        @"ScaleChg\((?<initial_value>\d+)\,(?<change_value>\d+)\)",
        RegexOptions.Compiled);

    


    public static bool IsInternalCrave(double internalMotivationLevel, double currentLowestStat, double envChange, double cutOff)
    {
        double adjustedCutoff = cutOff * (100 - internalMotivationLevel) / 50 * currentLowestStat / 50;

        //If environment isn't concerning enough, then will be internal motive
        if (Math.Abs(envChange) < adjustedCutoff)
        {
            return true;
        }
        return false;
    }

    

    //Predictor value, predictor change, stat of interest, and relationship node
    public static ReturnDecision CalculateSiNeDecisions(List<RelationshipNode> neRelationshipNodes, EnumPersonalityStats statOfInterest, AllStats allInitialStats, CharacterPsyche characterPsyche)
    {
        // Order by habits
        neRelationshipNodes = neRelationshipNodes.OrderByDescending(rn => rn.HabitCounter).ToList();
        double largestPositivePredictorValue = double.MinValue;
        double largestPositivePredictorChange = double.MinValue;
        double largestNegativePredictorValue = double.MaxValue;
        double largestNegativePredictorChange = double.MaxValue;
        RelationshipNode largestNegativePredictorNode = null;
        RelationshipNode largestPositivePredictorNode = null;
        EnumPersonalityStats largestPositivePredictorStat = statOfInterest; // Default to the stat of interest
        EnumPersonalityStats largestNegativePredictorStat = statOfInterest; // Default to the stat of interest
        int perspectivesExplored = 0;

        for(int i = 0; i < neRelationshipNodes.Count && i < characterPsyche.CognitiveStamina; i++)
        {
            RelationshipNode neRelationshipNode = neRelationshipNodes[i];
            double predictorValueOfInterest = double.MinValue;
            // Get the ModR values
            RelationshipValues modRValues = neRelationshipNode.ModRValues;
            Dictionary<EnumPersonalityStats, double> alternativeStatPerspectivesValues = new Dictionary<EnumPersonalityStats, double>();
            // TO-DO: Would having env (i.e. NL instead of L) affect how these are calculated? Should the results be scaled less?
            if (statOfInterest == EnumPersonalityStats.L || statOfInterest == EnumPersonalityStats.NL)
            {
                predictorValueOfInterest = modRValues.LivelihoodValue;
                if(perspectivesExplored < characterPsyche.PerspectiveAbility)
                {
                    if(statOfInterest == EnumPersonalityStats.L)
                    {
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.DB, modRValues.DefensiveBelongingValue);
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.NB, modRValues.NurtureBelongingValue);
                        perspectivesExplored++;
                    }
                    else
                    {
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.NNB, modRValues.NurtureBelongingValue);
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.NDB, modRValues.DefensiveBelongingValue);
                        perspectivesExplored++;
                    }
                    
                }
            }
            else if(statOfInterest == EnumPersonalityStats.DB || statOfInterest == EnumPersonalityStats.NDB)
            {
                predictorValueOfInterest = modRValues.DefensiveBelongingValue;
                if (perspectivesExplored < characterPsyche.PerspectiveAbility)
                {
                    if (statOfInterest == EnumPersonalityStats.DB)
                    {
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.L, modRValues.LivelihoodValue);
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.NB, modRValues.NurtureBelongingValue);
                        perspectivesExplored++;
                    }
                    else
                    {
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.NL, modRValues.LivelihoodValue);
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.NNB, modRValues.NurtureBelongingValue);
                        perspectivesExplored++;
                    }

                }
            }
            else if(statOfInterest == EnumPersonalityStats.NB || statOfInterest == EnumPersonalityStats.NNB)
            {
                predictorValueOfInterest = modRValues.NurtureBelongingValue;
                if (perspectivesExplored < characterPsyche.PerspectiveAbility)
                {
                    if (statOfInterest == EnumPersonalityStats.NB)
                    {
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.DB, modRValues.DefensiveBelongingValue);
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.L, modRValues.LivelihoodValue);
                        perspectivesExplored++;
                    }
                    else
                    {
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.NDB, modRValues.DefensiveBelongingValue);
                        alternativeStatPerspectivesValues.Add(EnumPersonalityStats.NL, modRValues.LivelihoodValue);
                        perspectivesExplored++;
                    }

                }
            }
            else
            {
                Debug.LogError("Unknown stat of interest: " + statOfInterest);
                continue; // Skip to the next iteration if the stat is unknown
            }

            //Adjust based on initial stats
            double changeValueOfInterest = predictorValueOfInterest - allInitialStats.StatOfInterest(statOfInterest.ToString());
            double adjustedChangeValueOfInterest = DMCalculationFunctions.ScaleSurvivalStatChange(changeValueOfInterest, allInitialStats.StatOfInterest(statOfInterest.ToString()));

            //Add the habit contribution
            double habitContribution = DMCalculationFunctions.HabitContribution(neRelationshipNode.HabitCounter, characterPsyche.MaxHabitCounter, characterPsyche.HabitualTendencies);

            //habit contribution makes positive and negative perspectives more likely
            double adjustedPositiveChangeValueOfInterest = adjustedChangeValueOfInterest + habitContribution;
            double adjustedNegativeChangeValueOfInterest = adjustedChangeValueOfInterest - habitContribution;

            // Check if this is the largest positive predictor value for stat of interest or more than the reward cutoff for character
            if (adjustedPositiveChangeValueOfInterest > largestPositivePredictorChange)
            {
                //add hbit to make it better than it is
                largestPositivePredictorValue = predictorValueOfInterest + habitContribution;
                largestPositivePredictorNode = neRelationshipNode;
                largestPositivePredictorChange = adjustedPositiveChangeValueOfInterest;
                largestPositivePredictorStat = statOfInterest;
            }

            //for negative predictor value for stat of interest

            if (adjustedNegativeChangeValueOfInterest < largestNegativePredictorChange) 
            { 
                //Subtracting to make it worse than it is
                largestNegativePredictorValue = predictorValueOfInterest - habitContribution;
                largestNegativePredictorNode = neRelationshipNode;
                largestNegativePredictorChange = adjustedNegativeChangeValueOfInterest;
                largestNegativePredictorStat = statOfInterest;
            }

            //Check other values to see
            foreach (KeyValuePair<EnumPersonalityStats, double> alternateStatPerspectiveValue in alternativeStatPerspectivesValues)
            {
                double predictorValue = alternateStatPerspectiveValue.Value;
                double changeValue = predictorValue - allInitialStats.StatOfInterest(alternateStatPerspectiveValue.Key.ToString());

                //Adjust values based on initial and opportunism level
                double adjustedChangeValue = DMCalculationFunctions.ScaleSurvivalStatChange(changeValue, allInitialStats.StatOfInterest(alternateStatPerspectiveValue.Key.ToString()));
                //Opportunism Adjustment debuffs because it's not target of interest
                adjustedChangeValue = DMCalculationFunctions.OpportunismAdjustment(adjustedChangeValue, characterPsyche.OpportunismLevel);

                //Add habit contribution to m
                double adjustedPositiveChangeValue = adjustedChangeValue + habitContribution;
                double adjustedNegativeChangeValue = adjustedChangeValue - habitContribution;

                if (adjustedPositiveChangeValue > largestPositivePredictorChange)
                {
                    largestPositivePredictorValue = predictorValue + habitContribution;
                    largestPositivePredictorChange = adjustedPositiveChangeValue;
                    largestPositivePredictorNode = neRelationshipNode;
                    largestPositivePredictorStat = alternateStatPerspectiveValue.Key;
                }

                if(adjustedNegativeChangeValue < largestNegativePredictorChange)
                {
                    largestNegativePredictorValue = predictorValueOfInterest - habitContribution;
                    largestNegativePredictorNode = neRelationshipNode;
                    largestNegativePredictorChange = adjustedNegativeChangeValue;
                    largestNegativePredictorStat = alternateStatPerspectiveValue.Key;
                }
            }

            
            
        }

        //Check if goes above the reward inclination
        //Check if risk is above risk cutoff

        double largestNegativePredictorAdjustedChangeWithRisk = largestNegativePredictorChange * characterPsyche.RiskAversion;
        ReturnDecision returnDecision = new ReturnDecision();

        //Input risking decision (can have both risky and commited decision)
        if (Math.Abs(largestNegativePredictorAdjustedChangeWithRisk) > characterPsyche.RiskCutoff)
        {
            returnDecision.SetNeValuesWithRisk(largestNegativePredictorValue, largestNegativePredictorAdjustedChangeWithRisk, largestNegativePredictorStat, largestNegativePredictorNode);
        }

        //Not rewarding enough
        if(largestPositivePredictorChange < characterPsyche.RewardCutoff)
        {
            returnDecision.SetNeValuesWithoutCommitDueToNotRewarding(largestPositivePredictorValue, largestPositivePredictorChange, largestPositivePredictorStat, largestPositivePredictorNode);
            return returnDecision;
        }


        

        returnDecision.SetNeValuesWithCommit(largestPositivePredictorValue, largestPositivePredictorChange, largestPositivePredictorStat, largestPositivePredictorNode);

        return returnDecision;

    }

    //Goes through all the decisions and perspectives for each decision
    //return predictor value, changebalue, personality stats of interest, and decision node
    public static ReturnDecision CalculateSeNiDecisions(List<RelationshipDecisionNode> niResponseNodes, EnumPersonalityStats statOfInterest, AllStats allInitialStats, CharacterMainCPort agent, CharacterMainCPort env, RelationshipNode envInAgentRPTNode, EnumActionCharacteristics actionContext)
    {
        // Order by habit counter
        niResponseNodes = niResponseNodes.OrderByDescending(rn => rn.HabitCounter).ToList();

        double ultimateLargestPositivePredictorValue = double.MinValue;
        double ultimateLargestPositivePredictorAdjustedChange = double.MinValue;
        double ultimateLargestNegativePredictorValue = double.MaxValue;
        double ultimateLargestNegativePredictorAdjustedChange = double.MaxValue;
        RelationshipDecisionNode ultimateLargestPositivePredictorNode = null;
        RelationshipDecisionNode ultimateLargestNegativePredictorNode = null;
        EnumPersonalityStats ultimateLargestPositivePredictorStat = statOfInterest; // Default to the stat of interest
        EnumPersonalityStats ultimateLargestNegativePredictorStat = statOfInterest; // Default to the stat of interest
        Perspective ultimateLargestPositivePredictorPerspective = null;
        Perspective ultimateLargestNegativePredictorPerspective = null;
        bool isSafeEnough = true;
        bool isRewardingEnough = true;
        bool hasCommitedAction = false;

        for (int i = 0; i < niResponseNodes.Count && i < agent.characterPsyche.CognitiveStamina; i++)
        {
            double largestPositivePredictorValue = double.MinValue;
            double largestPositivePredictorAdjustedChange = double.MinValue;
            double largestNegativePredictorValue = double.MaxValue;
            double largestNegativePredictorAdjustedChange = double.MaxValue;
            EnumPersonalityStats largestPositivePredictorStat = statOfInterest; // Default to the stat of interest
            EnumPersonalityStats largestNegativePredictorStat = statOfInterest; // Default to the stat of interest
            Perspective largestPositivePredictorPerspective = null;
            Perspective largestNegativePredictorPerspective = null;


            RelationshipDecisionNode niResponseNode = niResponseNodes[i];
            bool isComplexGoal = niResponseNode.Decision.DMType == DMTypes.Complex;

            //Goes through all the perspectives and picks the worst and best perspectives
            //Already does habit contribution with these calculations
            if (isComplexGoal)
            {
                (largestPositivePredictorValue, largestNegativePredictorValue, largestPositivePredictorAdjustedChange, largestNegativePredictorAdjustedChange,
                    largestPositivePredictorStat, largestNegativePredictorStat, largestPositivePredictorPerspective, largestNegativePredictorPerspective) =
                    DMCalculationFunctions.CalculateComplexPositiveAndNegativePredictorChange(niResponseNode.Decision, statOfInterest, allInitialStats, 
                    niResponseNode.Decision.HabitCounter, agent, env, envInAgentRPTNode, actionContext);
            }
            else
            {
                (largestPositivePredictorValue, largestNegativePredictorValue, largestPositivePredictorAdjustedChange, largestNegativePredictorAdjustedChange, 
                    largestPositivePredictorStat, largestNegativePredictorStat, largestPositivePredictorPerspective, largestNegativePredictorPerspective) =
                    DMCalculationFunctions.CalculateSimplePositiveAndNegativePredictorChange(niResponseNode.Decision.Perspectives, statOfInterest, allInitialStats, 
                    niResponseNode.Decision.HabitCounter, agent, env, envInAgentRPTNode, actionContext);
            }

            //Calculate reward and risk cutoffs for all these decision's perspectives to see if worth 
            //Pick best safe and rewarding actions
            double largestNegativePredictorAdjustedChangeWithRisk = largestNegativePredictorAdjustedChange * agent.characterPsyche.RiskAversion;

            if(largestPositivePredictorAdjustedChange > Math.Abs(largestNegativePredictorAdjustedChangeWithRisk))
            {
                if(largestPositivePredictorAdjustedChange > agent.characterPsyche.RewardCutoff)
                {
                    //Will replace automatically if currently no safe nor rewarding actions
                    if(!hasCommitedAction)
                    {
                        ultimateLargestPositivePredictorValue = largestPositivePredictorValue;
                        ultimateLargestPositivePredictorAdjustedChange = largestPositivePredictorAdjustedChange;
                        ultimateLargestPositivePredictorNode = niResponseNode;
                        ultimateLargestPositivePredictorStat = largestPositivePredictorStat;
                        ultimateLargestPositivePredictorPerspective = largestPositivePredictorPerspective;
                        isSafeEnough = true;
                        isRewardingEnough = true;
                        hasCommitedAction = true;
                    }
                    else
                    {
                        //Already safe and rewarding action so have to see if worth it
                        if(largestPositivePredictorAdjustedChange > ultimateLargestPositivePredictorAdjustedChange)
                        {
                            ultimateLargestPositivePredictorValue = largestPositivePredictorValue;
                            ultimateLargestPositivePredictorAdjustedChange = largestPositivePredictorAdjustedChange;
                            ultimateLargestPositivePredictorNode = niResponseNode;
                            ultimateLargestPositivePredictorStat = largestPositivePredictorStat;
                            ultimateLargestPositivePredictorPerspective = largestPositivePredictorPerspective;
                            isSafeEnough = true;
                            isRewardingEnough = true;
                            hasCommitedAction = true;
                        }

                    }

                }
                //Not rewarding enough
                else
                {
                    //If there aren't good options, then will just document not reward enough
                    //If risky action exist already, then not worth document (risky actions matter more than not rewarding)
                    if (isSafeEnough)
                    {
                        if (!hasCommitedAction)
                        {
                            if (largestPositivePredictorAdjustedChange > ultimateLargestPositivePredictorAdjustedChange)
                            {
                                ultimateLargestPositivePredictorValue = largestPositivePredictorValue;
                                ultimateLargestPositivePredictorAdjustedChange = largestPositivePredictorAdjustedChange;
                                ultimateLargestPositivePredictorNode = niResponseNode;
                                ultimateLargestPositivePredictorStat = largestPositivePredictorStat;
                                ultimateLargestPositivePredictorPerspective = largestPositivePredictorPerspective;
                                isRewardingEnough = false;
                            }
                        }
                        
                    }
                }
            }
            //If too risky
            else
            {
                //If there's already a decision to consider then ignore
                if(!hasCommitedAction)
                {
                    if (largestNegativePredictorAdjustedChange < ultimateLargestNegativePredictorAdjustedChange)
                    {
                        ultimateLargestNegativePredictorValue = largestNegativePredictorValue;
                        ultimateLargestNegativePredictorAdjustedChange = largestNegativePredictorAdjustedChange;
                        ultimateLargestNegativePredictorNode = niResponseNode;
                        ultimateLargestNegativePredictorStat = largestNegativePredictorStat;
                        ultimateLargestNegativePredictorPerspective = largestNegativePredictorPerspective;
                        isSafeEnough = false;
                    }
                }
                
                    
                
            }

        }


        

        ReturnDecision returnDecision = new ReturnDecision();

        // Determine whether to commit the action
        //check is the positive is better than negative
       
        if (hasCommitedAction)
        {
            Debug.Log("Will consider action: " + ultimateLargestPositivePredictorNode.Decision.name + " because of " + ultimateLargestPositivePredictorPerspective.Name);
            Debug.Log("Predictor adjusted CHANGE: " + ultimateLargestPositivePredictorAdjustedChange +
                        " Predictor VALUE: " + ultimateLargestPositivePredictorValue +
                        " Initial value: " + allInitialStats.StatOfInterest(ultimateLargestPositivePredictorStat.ToString()));

            returnDecision.SetNiValuesWithCommit(ultimateLargestPositivePredictorValue, ultimateLargestPositivePredictorAdjustedChange, ultimateLargestPositivePredictorStat, ultimateLargestPositivePredictorNode);

            return returnDecision;
        }
        //Not rewarding enough
        else if(!isRewardingEnough)
        {
            Debug.Log("Will not consider action: " + ultimateLargestPositivePredictorNode.Decision.name + " because " + ultimateLargestPositivePredictorPerspective.Name + " not worth it for the reward inclination of " + agent.characterPsyche.RewardCutoff);
            Debug.Log(" Adjusted predictor CHANGE: " + ultimateLargestPositivePredictorAdjustedChange +
                        " Predictor VALUE: " + ultimateLargestPositivePredictorValue +
                        " Initial value: " + allInitialStats.StatOfInterest(ultimateLargestPositivePredictorStat.ToString()));

            returnDecision.SetNiValuesWithoutCommitDueToNotRewarding(ultimateLargestPositivePredictorValue, ultimateLargestPositivePredictorAdjustedChange, ultimateLargestPositivePredictorStat, ultimateLargestPositivePredictorPerspective, ultimateLargestPositivePredictorNode);
            
            return returnDecision;
        }
        
        //Too risky
        else if(!isSafeEnough)
        {
            Debug.Log("Will not consider action: " + ultimateLargestPositivePredictorNode.Decision.name + " because not worth risk of " + ultimateLargestNegativePredictorPerspective.Name + " for the negative action " + ultimateLargestNegativePredictorNode.Decision.name);
            Debug.Log(" Adjusted negative predictor change: " + ultimateLargestNegativePredictorNode +
                      " Largest predictor value: " + ultimateLargestNegativePredictorValue +
                      " Initial value: " + allInitialStats.StatOfInterest(ultimateLargestNegativePredictorStat.ToString()));

            returnDecision.SetNiValuesWithRisk(ultimateLargestPositivePredictorValue, ultimateLargestPositivePredictorAdjustedChange, ultimateLargestNegativePredictorValue, 
                ultimateLargestNegativePredictorAdjustedChange, ultimateLargestPositivePredictorStat, ultimateLargestNegativePredictorStat, 
                ultimateLargestPositivePredictorPerspective, ultimateLargestNegativePredictorPerspective, 
                ultimateLargestPositivePredictorNode, ultimateLargestNegativePredictorNode);

            return returnDecision;
        }

        Debug.LogWarning("No valid Ni-Se Decisions met the conditions. Returning default null.");

        return null; 

    }

}
