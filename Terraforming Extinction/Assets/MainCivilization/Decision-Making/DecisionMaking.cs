using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DecisionMaking : MonoBehaviour
{
    //Placeholders
    public CharacterPsycheSO CharacterPsycheSO1Test;
    public CharacterPsycheSO CharacterPsycheSO2Test;

    private ICharacterPsyche CharacterPsyche1Test;
    private ICharacterPsyche CharacterPsyche2Test;
    private double externalMotivationCutoff = 30;

    //Will normally character pass parameters to this with the psyche. Implement later. Have this in the inspector because of testing
    public void ActionSelection()
    {
        //Interface will only be used if there are subset of characters. Will need to implement interface
        //CharacterPsyche1Test = CharacterPsycheSO1Test as ICharacterPsyche;
        //CharacterPsyche2Test = CharacterPsycheSO2Test as ICharacterPsyche;

        //Decide whether it is I/E motivated
        (EnumPersonalityStats targetStatType, double targetStatInitialValue) = FindStatOfInterest(CharacterPsyche1Test, CharacterPsyche2Test);

        //Need to implement. Be able to read the surroundings. 
        double envChange = 0;


        bool isInternalMotivation = DecisionMakingFunctions.IsInternalMotivation(CharacterPsycheSO1Test.InternalMotivationLevel, targetStatInitialValue, envChange, externalMotivationCutoff);

        double ultimateLargestPositivePredictorChange = double.MinValue;
        string ultimatePerspective = string.Empty;
        int actionIndexToCommit = -1;

        List<DecisionMakingSO> sortedDMTestings = DMTestings.OrderByDescending(d => d.HabitCounter).ToList();

        for (int i = 0; i < sortedDMTestings.Count && i < Character1.CognitiveStamina; i++)
        {
            List<Perspective> perspectives = DMTestings[i].Perspectives;
            // Find which stat is most concerning
            
            string targetStatString = TranslateEnumToString(targetStatType);
            bool isInternalOpportunity = Character1.MentalOpportunities == EnumMentalOpportunities.Internal;
            bool isComplexGoal = DMTestings[i].DMType == DMTypes.Complex;
            // Variables to store the results of predictor calculations
            double largestPositivePredictorValue = double.MinValue;
            double largestNegativePredictorValue = double.MaxValue;
            double largestPositivePredictorChange = 0;
            double largestNegativePredictorChange = 0;

            string positivePerspective = string.Empty;
            string negativePerspective = string.Empty;

            // Get the stat value
            if (isComplexGoal)
            {
                (largestPositivePredictorValue, largestNegativePredictorValue, largestPositivePredictorChange,
                    largestNegativePredictorChange, positivePerspective, negativePerspective)
                    = CalculateComplexPositiveAndNegativePredictorChange(DMTestings[i], targetStatString, Character1.PerspectiveAbility, isInternalOpportunity);

            }
            else
            {
                (largestPositivePredictorValue, largestNegativePredictorValue, largestPositivePredictorChange,
                    largestNegativePredictorChange, positivePerspective, negativePerspective)
                    = CalculateSimplePositiveAndNegativePredictorChange(DMTestings[i].Perspectives, targetStatString, Character1.PerspectiveAbility, isInternalOpportunity);
            }

            //HABIT TENDENCIES
            // Add the habit tendencies
            double habitContribution = HabitContribution(DMTestings[i].HabitCounter, Character1.MaxHabitCounter, Character1.HabitualTendencies);
            largestPositivePredictorChange += habitContribution;
            largestNegativePredictorChange -= habitContribution;
            largestPositivePredictorValue += habitContribution;
            largestNegativePredictorValue -= habitContribution;

            Debug.Log("Habit contribution: " + habitContribution + " Habit counter: " + DMTestings[i].HabitCounter + " Habit tendencies: " + Character1.HabitualTendencies + " Max habit counter: " + Character1.MaxHabitCounter);
            // Scale the changed value according to the starting stat of the player
            double adjustedLargestPositivePredictorChange = ScaleSurvivalStatChange(largestPositivePredictorChange, targetStatInitialValue);
            double adjustedLargestNegativePredictorChange = ScaleSurvivalStatChange(largestNegativePredictorChange, targetStatInitialValue);





            //multiply because if i do same with reward inclination and have cut off, i still need to determine what to prioritize
            //brain will see which has bigger signal anyways
            adjustedLargestNegativePredictorChange *= Character1.RiskAversion;

            // Determine whether to commit the action
            //check is the positive is better than negative
            if (adjustedLargestPositivePredictorChange > -adjustedLargestNegativePredictorChange)
            {
                if (adjustedLargestPositivePredictorChange > Character1.RewardInclination && adjustedLargestPositivePredictorChange > ultimateLargestPositivePredictorChange)
                {
                    ultimateLargestPositivePredictorChange = adjustedLargestPositivePredictorChange;
                    ultimatePerspective = positivePerspective;
                    actionIndexToCommit = i;
                    Debug.Log("Will consider action: " + DMTestings[i].name + " because of " + positivePerspective);
                    Debug.Log("Predictor CHANGE: " + largestPositivePredictorChange +
                              " Adjusted predictor CHANGE: " + adjustedLargestPositivePredictorChange +
                              " Predictor VALUE: " + largestPositivePredictorValue +
                              " Initial value: " + targetStatInitialValue);
                }
                else
                {
                    Debug.Log("Will not consider action: " + DMTestings[i].name + " because " + positivePerspective + " not worth it for the reward inclination of " + Character1.RewardInclination);
                    Debug.Log("Predictor CHANGE: " + largestPositivePredictorChange +
                              " Adjusted predictor CHANGE: " + adjustedLargestPositivePredictorChange +
                              " Predictor VALUE: " + largestPositivePredictorValue +
                              " Initial value: " + targetStatInitialValue);
                }
            }
            //risk too high once adjusted
            else
            {
                Debug.Log("Will not consider action: " + DMTestings[i].name + " because not worth risk of " + negativePerspective + " for the " + positivePerspective);
                Debug.Log("Largest predictor change: " + largestNegativePredictorChange +
                          " Adjusted predictor change: " + adjustedLargestNegativePredictorChange +
                          " Largest predictor value: " + largestNegativePredictorValue +
                          " Initial value: " + targetStatInitialValue);
            }
        }

        if (actionIndexToCommit == -1)
        {
            Debug.Log("No actions to commit to");
        }
        else
        {
            Debug.Log("Will commit action: " + DMTestings[actionIndexToCommit].name + " because of " + ultimatePerspective);
        }

    }

    //Can move to DM Functions
    //Env should be changed. This is for testing of character relationship with environment
    private (EnumPersonalityStats, double) FindStatOfInterest(CharacterPsycheSO character, CharacterPsycheSO env)
    {
        double L = character.Stats.L;
        double B = 0;

        // Determine the B value based on BIdentity
        if (character.BIdentity == EnumPersonalityStats.NB)
        {
            B = character.Stats.NB;
        }
        else
        {
            B = character.Stats.DB;
        }

        double FL = 0;
        double FDB = 0;
        double FNB = 0;
        double agentFriendPRScale = 0;
        double agentEnvPRScale = 0;
        int friendCount = character.Friends.Count;

        // Accumulate friends' stats and env
        foreach (CharacterPsycheSO friend in character.Friends)
        {
            //find the friend in PR to see how much they care overall
            foreach (RelationshipValueTest prCharacter in character.RelationshipSheet.PR)
            {

                if (prCharacter.Identifiers == env)
                {
                    agentEnvPRScale = (prCharacter.Stats.NB + prCharacter.Stats.DB) / 2;
                    agentEnvPRScale /= 100;
                }
                if (prCharacter.Identifiers == friend)
                {
                    agentFriendPRScale = (prCharacter.Stats.NB + prCharacter.Stats.DB) / 2;
                    agentFriendPRScale /= 100;
                }
            }
            //it ups the stats when calculating the stats to find the lowest stat to focus on. i.e. if bad relationship with friend, then will inflate stats of friend so wouldn't care too much
            //11 is arbitrary. Just sets limit to 10 for highest empathy level
            double adjustedEmpathyFriendPRScale = CalculateTargetAdjustedEmpathyPRStat(character.EmpathyLevel, agentFriendPRScale);

            FL += friend.Stats.L * adjustedEmpathyFriendPRScale;
            FDB += friend.Stats.DB * adjustedEmpathyFriendPRScale;
            FNB += friend.Stats.NB * adjustedEmpathyFriendPRScale;


        }

        // Calculate averages if there are friends
        if (friendCount > 0)
        {
            FL /= friendCount;
            FDB /= friendCount;
            FNB /= friendCount;
        }

        double adjustedEmpathyEnvPRScale = CalculateTargetAdjustedEmpathyPRStat(character.EmpathyLevel, agentEnvPRScale);

        //calculate N(Stats) values
        double envL = env.Stats.L * adjustedEmpathyEnvPRScale;
        double envNB = env.Stats.NB * adjustedEmpathyEnvPRScale;
        double envDB = env.Stats.DB * adjustedEmpathyEnvPRScale;

        // Create a dictionary mapping enum values to their corresponding scores
        Dictionary<EnumPersonalityStats, double> statValues = new()
        {
            { EnumPersonalityStats.L, L },
            { EnumPersonalityStats.DB, character.BIdentity == EnumPersonalityStats.DB ? B : double.MaxValue },
            { EnumPersonalityStats.NB, character.BIdentity == EnumPersonalityStats.NB ? B : double.MaxValue },
            { EnumPersonalityStats.FL, FL },
            { EnumPersonalityStats.FDB, FDB },
            { EnumPersonalityStats.FNB, FNB },
            { EnumPersonalityStats.NL, envL },
            { EnumPersonalityStats.NDB, envDB },
            { EnumPersonalityStats.NNB, envNB }
        };

        // Find the enum with the smallest value
        EnumPersonalityStats minStat = EnumPersonalityStats.L;
        double minValue = double.MaxValue;

        foreach (var stat in statValues)
        {
            if (stat.Value < minValue)
            {
                minValue = stat.Value;
                minStat = stat.Key;
            }
        }
        Debug.Log("minimum value: " + minValue + " minStat: " + minStat);
        return (minStat, minValue);
    }
}
