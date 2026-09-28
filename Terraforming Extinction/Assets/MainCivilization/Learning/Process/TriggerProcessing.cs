using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TriggerProcessing : MonoBehaviour
{
    private CharacterMainCPort selfMainCPort;
    private LearningProcess learningProcess;
    private void Start()
    {
        selfMainCPort = GetComponent<CharacterMainCPort>();
        learningProcess = GetComponent<LearningProcess>();
    }
    public void CheckExpectationalTrigger(Dictionary<CharacterMainCPort, SubIdentifierRelationshipNodeInfo> envCPortToSubIdMap)
    {
        //1. Check if the change in stats (with survival adjusted) exceeds character's expectation threshold
        //2. If so trigger the learning process

        //Notes: Have to consider using risk inclinement for negative triggers. 
        foreach (CharacterMainCPort envMainCPort in envCPortToSubIdMap.Keys)
        {
            SubIdentifierRelationshipNodeInfo subIdentifierRelationshipNodeInfo = envCPortToSubIdMap[envMainCPort];
            RelationshipValues envModRValues = envCPortToSubIdMap[envMainCPort].RelationshipNode.ModRValues;

            var allEnvModRValues = new Dictionary<EnumPersonalityStats, double>
            {
                { EnumPersonalityStats.L , envModRValues.LivelihoodValue},
                {EnumPersonalityStats.DB, envModRValues.DefensiveBelongingValue},
                {EnumPersonalityStats.NB, envModRValues.NurtureBelongingValue}
            };

            EnumPersonalityStats highestModRStat = default;
            double highestModRValue = double.MinValue;

            foreach (var kvp in allEnvModRValues)
            {
                if (kvp.Value > highestModRValue)
                {
                    highestModRValue = kvp.Value;
                    highestModRStat = kvp.Key;
                }
            }



            EnumPersonalityStats lowestModRStat = default;
            double lowestModRValue = double.MaxValue;

            foreach (var kvp in allEnvModRValues)
            {
                if (kvp.Value < lowestModRValue)
                {
                    lowestModRValue = kvp.Value;
                    lowestModRStat = kvp.Key;
                }
            }

            EnumPersonalityStats mostImpactfulModRStat = default;
            double mostImpactfulModRValue = 0;
            double characterCorrespondingStatValue = 0;
            bool couldBePositiveTrigger = false;

            //Process highest modR Values

            if (highestModRValue < 0)
            {
                couldBePositiveTrigger = false;
            }
            else
            {
                couldBePositiveTrigger = true;
                //Find the corresponding stat of the character
                if (highestModRStat == EnumPersonalityStats.L)
                {
                    characterCorrespondingStatValue = selfMainCPort.characterPhysical.Stats.L;
                }
                else if (highestModRStat == EnumPersonalityStats.DB)
                {
                    characterCorrespondingStatValue = selfMainCPort.characterPhysical.Stats.DB;
                }
                else if (highestModRStat == EnumPersonalityStats.NB)
                {
                    characterCorrespondingStatValue = selfMainCPort.characterPhysical.Stats.NB;
                }

                highestModRValue = DMCalculationFunctions.ScaleSurvivalStatChange(highestModRValue, characterCorrespondingStatValue);
            }

            if (lowestModRStat == EnumPersonalityStats.L)
            {
                characterCorrespondingStatValue = selfMainCPort.characterPhysical.Stats.L;
            }
            else if (lowestModRStat == EnumPersonalityStats.DB)
            {
                characterCorrespondingStatValue = selfMainCPort.characterPhysical.Stats.DB;
            }
            else if (lowestModRStat == EnumPersonalityStats.NB)
            {
                characterCorrespondingStatValue = selfMainCPort.characterPhysical.Stats.NB;
            }

            lowestModRValue = DMCalculationFunctions.ScaleSurvivalStatChange(lowestModRValue, characterCorrespondingStatValue);
            lowestModRValue = DMCalculationFunctions.RiskAdjustment(lowestModRValue, selfMainCPort.characterPsyche.RiskAversion);

            //Default to positive trigger. e.g. negatve: 3, positive: 5, deafult: positive 5
            if (lowestModRValue >= 0)
            {
                mostImpactfulModRStat = highestModRStat;
                mostImpactfulModRValue = highestModRValue;

                //default to negative trigger. (Positive trigger is actually negative. e.g. Positive: -2, Negative: -5, default -5)
            }
            else if (!couldBePositiveTrigger)
            {
                mostImpactfulModRStat = lowestModRStat;
                mostImpactfulModRValue = lowestModRValue;
            }
            // e.g. Positive: 5, Negative: -2, now actually compare
            else if (couldBePositiveTrigger)
            {

                //Now actually compare
                if (Math.Abs(lowestModRValue) >= Math.Abs(highestModRValue))
                {
                    mostImpactfulModRStat = lowestModRStat;
                    mostImpactfulModRValue = lowestModRValue;
                }
                else
                {
                    mostImpactfulModRStat = highestModRStat;
                    mostImpactfulModRValue = highestModRValue;
                }
            }



            if (mostImpactfulModRValue >= selfMainCPort.characterPsyche.ExpectationLearningThreshold)
            {
                //Trigger learning process 
                learningProcess.LearningProcessTrigger();
            }
        }
    }

    //called from repercussion
    public void CheckGroundedTrigger(double lValue, double dbValue, double nbValue)
    {
        double maxValue = Math.Max(lValue, Math.Max(dbValue, nbValue));

        double groundedThreshold = selfMainCPort.characterPsyche.GroundedLearningThreshold;

        if (maxValue >= groundedThreshold)
        {
            //Trigger learning process 
            learningProcess.LearningProcessTrigger();
        }
    }

    // Grounded trigger using environmental relationships. Not from repercussions but from checking surroundings (identity detector)
    public void CheckGroundedTrigger(
        Dictionary<CharacterMainCPort, SubIdentifierRelationshipNodeInfo> envCPortToSubIdMap)
    {
        double groundedThreshold =
            selfMainCPort.characterPsyche.GroundedLearningThreshold;

        foreach (CharacterMainCPort envMainCPort in envCPortToSubIdMap.Keys)
        {
            RelationshipValues modRValues =
                envCPortToSubIdMap[envMainCPort].RelationshipNode.ModRValues;

            double maxValue = Math.Max(
                modRValues.LivelihoodValue,
                Math.Max(
                    modRValues.DefensiveBelongingValue,
                    modRValues.NurtureBelongingValue
                )
            );

            if (maxValue >= groundedThreshold)
            {
                learningProcess.LearningProcessTrigger();
                return;
            }
        }
    }
}
