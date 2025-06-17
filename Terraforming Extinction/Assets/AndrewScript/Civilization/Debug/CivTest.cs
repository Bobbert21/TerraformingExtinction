    using System;
using System.Collections;
using System.Collections.Generic;
using System.Data; // For expression evaluation
using UnityEngine;
using System.Linq;


public class CivTest : MonoBehaviour
{
    

    public List<DecisionMakingSO> DMTestings;
    public CharactersTestSO Character1;
    public CharactersTestSO Character2;
    public int ComplexGoalStep;
    public List<CharactersTestSO> CharactersDictionary;
    //the center of where the stat scale to 1
    private double FulcrumStatScale = 30;

    // 1. Pick the most positive ModR to boost its stats (Could skip since I'm automatically inputting the action i want to test
    // 2. Pick a few focus of the perspectives based on what Stat the character is needing
    // 3. Calculate the predictor of some of the perspectives
    // 4. Model to see which one causes the most change from start to finish
    // 5. Decides whether to commit to action or not
    // 6. Add in a context consideration before making action

    //2. Selecting the right action based on characters stats and prediction
    public void ActionSelection()
    {
        double ultimateLargestPositivePredictorChange = double.MinValue;
        string ultimatePerspective = string.Empty;
        int actionIndexToCommit = -1;

        List<DecisionMakingSO> sortedDMTestings = DMTestings.OrderByDescending(d => d.HabitCounter).ToList();

        for (int i = 0;  i < sortedDMTestings.Count && i < Character1.CognitiveStamina; i++)
        {
            List<Perspective> perspectives = DMTestings[i].Perspectives;
            // Find which stat is most concerning
            (EnumPersonalityStats targetStatType, double targetStatInitialValue) = FindStatOfInterest(Character1, Character2);
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

        if (actionIndexToCommit == -1) {
            Debug.Log("No actions to commit to");
        }
        else
        {
            Debug.Log("Will commit action: " + DMTestings[actionIndexToCommit].name + " because of " + ultimatePerspective);
        }
        

    }


    private double HabitContribution(int habitCounter, int maxHabitCounter, double habitualTendencies)
    {
        double habitContribution = (double)habitCounter / (double)maxHabitCounter * habitualTendencies;
        return habitContribution;
    }



    private (double, double, double, double, string, string) CalculateSimplePositiveAndNegativePredictorChange(List<Perspective> perspectives, string targetStatString, int perspectiveAbility, bool isInternalOpportunity)
    {
        double largestPositivePredictorValue = double.MinValue;
        double largestNegativePredictorValue = double.MaxValue;
        double largestPositivePredictorChange = double.MinValue;
        double largestNegativePredictorChange = double.MaxValue;
        string positivePerspective = "";
        string negativePerspective = "";

        //sort by perspectives
        List<Perspective> sortedHabitPerspectives = perspectives.OrderByDescending(p => p.HabitCounter).ToList();

        //get the targetStat change that matters
        for (int i = 0; i < sortedHabitPerspectives.Count && i < perspectiveAbility; i++)
        {
            string target = sortedHabitPerspectives[i].Target;
            //check for internal opportunities. See greatest change for what is highest need for agent
            if (target == targetStatString || isInternalOpportunity == false)
            {
                (double predictorValue, double changeValue) = Translate_String_To_Formula_Calculations(sortedHabitPerspectives[i].Predictor, sortedHabitPerspectives[i].Target, Character1, Character2);
                Debug.Log(sortedHabitPerspectives[i].Name + " predictor value: " + predictorValue + " Change value: " + changeValue);


                //evaluate the greatest change for perspectives
                if (changeValue > largestPositivePredictorChange)
                {
                    largestPositivePredictorValue = predictorValue;
                    largestPositivePredictorChange = changeValue;
                    positivePerspective = sortedHabitPerspectives[i].Name;
                }

                if (changeValue < largestNegativePredictorChange)
                {
                    Debug.Log("Checking negative predictor with: " + sortedHabitPerspectives[i].Name);
                    largestNegativePredictorValue = predictorValue;
                    largestNegativePredictorChange = changeValue;
                    negativePerspective = sortedHabitPerspectives[i].Name;
                }
            }

        }

        return (largestPositivePredictorValue, largestNegativePredictorValue, largestPositivePredictorChange, largestNegativePredictorChange, positivePerspective, negativePerspective);
    }

    private (double, double, double, double, string, string) CalculateComplexPositiveAndNegativePredictorChange(DecisionMakingSO decisionMakingSO, string targetStatString, int cognitiveStamina, bool isInternalOpportunity)
    {
        double largestPositivePredictorValue = double.MinValue;
        double largestNegativePredictorValue = double.MaxValue;
        double largestPositivePredictorChange = double.MinValue;
        double largestNegativePredictorChange = double.MaxValue;
        string positivePerspective = "";
        string negativePerspective = "";

        List<Perspective> goalPerspectives = decisionMakingSO.Perspectives;
        List<Perspective> actionPerspectives = decisionMakingSO.ComplexGoalActions[ComplexGoalStep].Perspectives;

        List<Perspective> sortedHabitGoalPerspectives = goalPerspectives.OrderByDescending(p => p.HabitCounter).ToList();
        List<Perspective> sortedHabitActionPerspectives = actionPerspectives.OrderByDescending(p => p.HabitCounter).ToList();

        //get the targetStat change that matters
        for (int i = 0; i < sortedHabitGoalPerspectives.Count && i < cognitiveStamina; i++)
        {
            string target = sortedHabitGoalPerspectives[i].Target;
            //check for internal opportunities. See greatest change for what is highest need for agent
            //will constantly check if looks only for external opportunities (isInternalOpportunity = false)
            //External opportunities check for all regardless of whether it matches what it needs internally (targetStatString). May need to change this
            if (target == targetStatString || isInternalOpportunity == false)
            {

                (double predictorValue, double changeValue) = Translate_String_To_Formula_Calculations(sortedHabitGoalPerspectives[i].Predictor, sortedHabitGoalPerspectives[i].Target, Character1, Character2);
                //adjust value depending on which step and progress inclination
                
                //calculate selfefficacy score. This will change the amount of steps it is on
                //Adjusted so self-efficacy is maxed at 100
                double selfEfficacyScore = ((100 - Character1.SelfEfficacy * 150 / 100 + 50) / 100);
                //If high in self-efficacy, feels like there's not a lot of steps to get to goal. Lower self-efficacy score is better 
                double totalStepsPerceived = decisionMakingSO.ComplexGoalActions.Count * selfEfficacyScore;
                //how close are they to goal. Closer they are, the more they are enticed by it
                double progressPerceived = Math.Min(ComplexGoalStep + 1 / totalStepsPerceived, 1);

                //Adjusted score based on how close they feel they are to goal and how much they care about progress with how much progress they feel they are making
                //predictorValue - changeValue = starting value.
                //So calculate the change adjusted and add with the starting value at end
                double goalAdjustedPredictorValue = changeValue * progressPerceived + Character1.ProgressInclination / totalStepsPerceived + (predictorValue - changeValue); 

                double goalChangeValue = changeValue + goalAdjustedPredictorValue - predictorValue;

                Debug.Log("Name of goal perspective: " + sortedHabitGoalPerspectives[i].Name + " Predictor Value: " + predictorValue + " Goal predicted value with progression values: " + goalAdjustedPredictorValue + " Goal adjusted change value: " + goalChangeValue);

                changeValue = goalChangeValue;

                //evaluate the greatest change for perspectives
                if (changeValue > largestPositivePredictorChange)
                {
                    largestPositivePredictorValue = goalAdjustedPredictorValue;
                    largestPositivePredictorChange = changeValue;
                    positivePerspective = sortedHabitGoalPerspectives[i].Name;
                }

                if (changeValue < largestNegativePredictorChange)
                {
                    largestNegativePredictorValue = goalAdjustedPredictorValue;
                    largestNegativePredictorChange = changeValue;
                    negativePerspective = sortedHabitGoalPerspectives[i].Name;
                }
            }

        }

        //get the targetStat change that matters
        for (int i = 0; i < sortedHabitActionPerspectives.Count && i < cognitiveStamina; i++)
        {
            string target = sortedHabitActionPerspectives[i].Target;
            //check for internal opportunities. See greatest change for what is highest need for agent
            if (target == targetStatString || isInternalOpportunity == false)
            {


                (double predictorValue, double changeValue) = Translate_String_To_Formula_Calculations(sortedHabitActionPerspectives[i].Predictor, sortedHabitActionPerspectives[i].Target, Character1, Character2);
                //adjust value depending on which step and progress inclination

                Debug.Log(sortedHabitActionPerspectives[i].Name + " Step predictor value: " + predictorValue
                    + " Step Change value: " + changeValue);



                //evaluate the greatest change for perspectives
                if (changeValue > largestPositivePredictorChange)
                {
                    largestPositivePredictorValue = predictorValue;
                    largestPositivePredictorChange = changeValue;
                    positivePerspective = sortedHabitActionPerspectives[i].Name;
                }

                if (changeValue < largestNegativePredictorChange)
                {
                    largestNegativePredictorValue = predictorValue;
                    largestNegativePredictorChange = changeValue;
                    negativePerspective = sortedHabitActionPerspectives[i].Name;
                }
            }

            
        }

        return (largestPositivePredictorValue, largestNegativePredictorValue, largestPositivePredictorChange, largestNegativePredictorChange, positivePerspective, negativePerspective);
    }

    private double ScaleSurvivalStatChange(double survivalChange, double initialStat)
    {
        if(initialStat < 1)
        {
            initialStat = 1;
        }

        double scalingFactor = FulcrumStatScale / initialStat;

        return survivalChange * scalingFactor;
    }

    private (EnumPersonalityStats, double) FindStatOfInterest(CharactersTestSO character, CharactersTestSO env)
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
        int friendCount = character.FriendsTest.Count;

        // Accumulate friends' stats and env
        foreach (CharactersTestSO friend in character.FriendsTest)
        {
            //find the friend in PR to see how much they care overall
            foreach (RelationshipValueTest prCharacter in character.RelationshipSheet.PR)
            {

                if(prCharacter.Identifiers == env)
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


    private string TranslateEnumToString(EnumPersonalityStats stat)
    {
        if(stat == EnumPersonalityStats.L)
        {
            return "L(A)";
        }else if(stat == EnumPersonalityStats.NB)
        {
            return "NB(A)";
        }
        else if(stat == EnumPersonalityStats.DB)
        {
            return "DB(A)";
        }else if(stat == EnumPersonalityStats.FL)
        {
            return "L(F)";
        }else if(stat == EnumPersonalityStats.FDB)
        {
            return "DB(F)";
        }else if(stat == EnumPersonalityStats.FNB)
        {
            return "NB(F)";
        }else if(stat == EnumPersonalityStats.NL)
        {
            return "L(N)";
        }
        else if(stat == EnumPersonalityStats.NNB)
        {
            return "NB(N)";
        }
        else if(stat == EnumPersonalityStats.NDB)
        {
            return "DB(N)";
        }
        return "";
    }

    (double predictor, double change) Translate_String_To_Formula_Calculations(string formula, string target, CharactersTestSO agent, CharactersTestSO env)
    {
        double fdb = 0;
        double fl = 0;
        double fnb = 0;
        double fModLNTemp = 0;
        double fModDbNTemp = 0;
        double fModNbNTemp = 0;
        double fModDbN = 0;
        double fModLN = 0;
        double fModNbN = 0;
        double friendCount = 0;
        foreach (CharactersTestSO friend in agent.FriendsTest) 
        {
            fl += friend.Stats.L;
            fdb += friend.Stats.DB;
            fnb += friend.Stats.NB;
            double individualFriendPRScale = 1;

            //find the relationship with the friend. This is found with the average of B in the PR of agent towards friend
            foreach(RelationshipValueTest prFriend in agent.RelationshipSheet.PR)
            {
                if(prFriend.Identifiers == friend)
                {
                    individualFriendPRScale = (prFriend.Stats.NB + prFriend.Stats.DB) / 2;
                    individualFriendPRScale /= 100;
                    break;
                }
            }
            //get all the modR value of friends and scaled with how connected they are with each people
            (fModLNTemp, fModDbNTemp, fModNbNTemp) = ReturnModValuesWithCharacter(friend, env, individualFriendPRScale);
            fModDbN += fModDbNTemp;
            fModLN += fModLNTemp;
            fModNbN += fModNbNTemp;
            friendCount++;
        }

        if (friendCount > 0) 
        {
            //ERROR: Didn't adjust empathy levels with fl, fdb, fnb. Also, I think I didn't adjust accurately for empathy since didn't use CalculateAdjustedEmpathyPRStat
            fl /= friendCount;
            fdb /= friendCount;
            fnb /= friendCount;
            fModDbN /= friendCount;
            fModNbN /= friendCount;
            fModLN /= friendCount;
        }

        // Parse `E(n)` values dynamically
        //Dictionary<string, double> dynamicEnemyStats = ParseRelationshipStats(agent, env);

        // Function to parse dynamic terms like N-ModR:L(E(1))
        //Could add F- or friend into this later on
        double ParseComplexTerm(string term)
        {
            var match = System.Text.RegularExpressions.Regex.Match(term,
            @"(?<entity>F\((?<entity_index>-?\d+)\)|A|N)-(?<relation>ModR|PR|SR):(?<stat>L|DB|NB|B)\((?<target>E\((?<target_index>-?\d+)\)|N|A|-(?<specific>\w+)-)\)");

            if (match.Success)
            {
                //1. Find Entity
                //2. Find Target
                //3. Find Relationship from Entity
                //4. Find Target in Relationship (from Entity)
                //5. Find average if needed
                string entity = match.Groups["entity"].Value;
                string relationType = match.Groups["relation"].Value;
                string stat = match.Groups["stat"].Value;
                string target = match.Groups["target"].Value;
                //see if it is averaging target. Usually this means this has multiple targets
                List<CharactersTestSO> entityObjects = new();
                //1. Get entity
                if (entity == "A")
                {
                    entityObjects.Add(agent);
                }
                else if (entity == "N")
                {
                    entityObjects.Add(env);
                } else if (entity.StartsWith("F("))
                {
                    int entityIndex = int.Parse(match.Groups["entity_index"].Value);
                    if(entityIndex == -1)
                    {
                        entityObjects = agent.FriendsTest.ToList();
                    }
                    else
                    {
                        entityObjects.Add(agent.FriendsTest[entityIndex]);
                    }
                }

                //2. Obtain Target
                List<CharactersTestSO> targetSO = new();
                
                if (target == "A")
                {
                    targetSO.Add(agent);
                } else if (target == "N")
                {
                    targetSO.Add(env);
                    //enemy. Default is agent's enemies. If want to do env enemies, could do N-env and would have to implement
                } else if (target.StartsWith("E("))
                {
                    int targetIndex = int.Parse(match.Groups["target_index"].Value);
                    //this is when E(-1). So want to see all of the average of the enemy
                    //in future if needed not average but culmulative sum, then can use E(-2)
                    if (targetIndex == -1)
                    {
                        //shallow copy so not by reference
                        targetSO = agent.EnemyTest.ToList();
                    }
                    //normal. Like E(0) or E(1)
                    else
                    {
                        targetSO.Add(GetEnemyByIndex(agent, targetIndex));
                    }
                }else if(target.StartsWith("-")){
                    foreach(CharactersTestSO character in CharactersDictionary)
                    {
                        string specific = match.Groups["specific"].Value;
                        if(character.Name == specific)
                        {
                            targetSO.Add(character);
                            break;
                        }
                    }
                }

                if (targetSO != null)
                {
                    // 1. Get total of all stats of interest for target
                    // 2. Divide by totalCount to get average
                    double statOfInterest = 0;
                    int totalCount = 0;
                    //loop through all entities
                    foreach (CharactersTestSO entityObject in entityObjects) 
                    {
                        // Select the appropriate relationship sheet based on the relation type

                        List<CharactersTestSO> copyTargetSO = targetSO.ToList();
                        var relationshipList = relationType switch
                        {
                            "ModR" => entityObject.RelationshipSheet.ModR,
                            "PR" => entityObject.RelationshipSheet.PR,
                            "SR" => entityObject.RelationshipSheet.SR,
                            _ => throw new ArgumentException($"Unknown relationship type: {relationType}")
                        };

                        if (targetSO.Count== 0)
                        {
                            Debug.Log("No targetSO found when calculating average. Returning 0.");
                            return 0;
                        }else if(entityObjects.Count == 0)
                        {
                            Debug.Log("No entitySO found when calculating average. Returning 0.");
                            return 0;
                        }
                        else
                        {
                            // Look for the target in the selected relationship list
                            foreach (var relationship in relationshipList)
                            {
                                if (copyTargetSO.Contains(relationship.Identifiers))
                                {
                                    statOfInterest += stat switch
                                    {
                                        "L" => relationship.Stats.L,
                                        "DB" => relationship.Stats.DB,
                                        "NB" => relationship.Stats.NB,
                                        "B" => (relationship.Stats.DB + relationship.Stats.NB) / 2,
                                        _ => throw new ArgumentException($"Unknown stat: {stat}")
                                    };

                                    //Found a stat and record an increase in the count
                                    totalCount += 1;

                                    copyTargetSO.Remove(relationship.Identifiers);
                                    if (copyTargetSO.Count == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    //return average
                    Debug.Log("Average Target of " + target + " is " + statOfInterest / totalCount + " Total count: " + totalCount + " stat of interest value: " + statOfInterest);
                    return statOfInterest / totalCount;
                }
            }
            throw new ArgumentException($"Unable to parse complex term: {term}");
        }
        
        double ParseScaleChangeTerm(string term)
        {
            // Match the pattern "ScaleChg(10,3)"
            var match = System.Text.RegularExpressions.Regex.Match(term, @"ScaleChg\((?<initial_value>\d+)\,(?<change_value>\d+)\)");
            if (match.Success)
            {
                string special = match.Groups["special"].Value;
                double initialValue = double.Parse(match.Groups["initial_value"].Value);
                double changeValue = double.Parse(match.Groups["change_value"].Value);
                
                return ScaleSurvivalStatChange(changeValue, initialValue);
                

                // Handle unsupported special terms
                throw new ArgumentException($"Unsupported special term: {term}");
            }

            // Handle unmatched terms
            throw new ArgumentException($"Unable to parse special term: {term}");
        }

        double ParseEmpTerm(string term)
        {
            // Match the pattern "Emp(A-N)" or "Emp(N-A)"
            var match = System.Text.RegularExpressions.Regex.Match(term, @"Emp\((?<origin>A|N)-(?<end>A|N)\)");

            if (match.Success)
            {
                string origin = match.Groups["origin"].Value;
                string end = match.Groups["end"].Value;

                // Determine origin empathy level and character
                double originEmpathyLevel = 0;
                CharactersTestSO originCharacter = null;
                CharactersTestSO endCharacter = null;

                if (origin == "N")
                {
                    originEmpathyLevel = env.EmpathyLevel;
                    originCharacter = env;
                }
                else if (origin == "A")
                {
                    originEmpathyLevel = agent.EmpathyLevel;
                    originCharacter = agent;
                }

                // Determine end character that empathy is towards
                if (end == "A")
                    endCharacter = agent;
                else if (end == "N")
                    endCharacter = env;

                // Calculate the PR scale
                double prScale = 0;
                bool foundPrScale = false;
                foreach (RelationshipValueTest prCharacter in originCharacter.RelationshipSheet.PR)
                {
                    if (prCharacter.Identifiers == endCharacter)
                    {
                        prScale = (prCharacter.Stats.NB + prCharacter.Stats.DB) / 2;
                        prScale /= 100;
                        foundPrScale = true;
                        break;
                    }
                }

                // Handle missing relationship gracefully
                if (!foundPrScale)
                    throw new InvalidOperationException($"No PR scale found between {originCharacter} and {endCharacter}");

                return CalculateFormulaAdjustedEmpathyPRStat(originEmpathyLevel, prScale);
            }

            // Handle unmatched terms
            throw new ArgumentException($"Unable to parse special term: {term}");
        }


        double ParseSimpleTerm(string term)
        {
            // Simple terms such as L(E), DB(N), etc.
            var match = System.Text.RegularExpressions.Regex.Match(term, @"(?<stat>L|DB|NB)\((?<target>A|N|E\((?<index>-?\d+)\))\)");

            if (match.Success)
            {
                // Extract the stat and target from the matched term
                string stat = match.Groups["stat"].Value;
                string target = match.Groups["target"].Value;

                // Select the correct stat based on the target
                if (target == "A")
                {
                    // Return the value from the agent's stats based on the stat
                    return stat switch
                    {
                        "L" => agent.Stats.L,
                        "DB" => agent.Stats.DB,
                        "NB" => agent.Stats.NB,
                        _ => throw new ArgumentException($"Unknown stat: {stat}")
                    };
                }
                else if (target == "N")
                {
                    // Return the value from the env's stats based on the stat
                    return stat switch
                    {
                        "L" => env.Stats.L,
                        "DB" => env.Stats.DB,
                        "NB" => env.Stats.NB,
                        _ => throw new ArgumentException($"Unknown stat: {stat}")
                    };
                }
                else if (target.StartsWith("E("))
                {
                    // Handle case when target is "E"
                    int index = int.Parse(match.Groups["index"].Value);

                    if (index == -1)
                    {
                        // Collect the stats from all enemies in the EnemyTest list
                        List<double> enemyStats = new List<double>();

                        foreach (var enemy in agent.EnemyTest)
                        {

                            double enemyStat = stat switch
                            {
                                "L" => enemy.Stats.L,
                                "DB" => enemy.Stats.DB,
                                "NB" => enemy.Stats.NB,
                                _ => throw new ArgumentException($"Unknown stat: {stat}")
                            };

                            // Add the stat value to the list
                            enemyStats.Add(enemyStat);
                        }

                        // Calculate the average of the collected stats
                        if (enemyStats.Count > 0)
                        {
                            return enemyStats.Average();
                        }
                        else
                        {
                            throw new ArgumentException("No enemies found in EnemyTest list.");
                        }
                    }
                    else
                    {
                        // If index is not -1, handle the individual enemy
                        var enemy = agent.EnemyTest.ElementAtOrDefault(index);
                        if (enemy != null)
                        {

                            return stat switch
                            {
                                "L" => enemy.Stats.L,
                                "DB" => enemy.Stats.DB,
                                "NB" => enemy.Stats.NB,
                                _ => throw new ArgumentException($"Unknown stat: {stat}")
                            };
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid enemy index: {index}");
                        }
                    }
                }
                else
                {
                    // Handle unmatched targets
                    throw new ArgumentException($"Unsupported target: {target}");
                }
            }

            // Handle unmatched terms
            throw new ArgumentException($"Unable to parse simple term: {term}");
        }


        // Replace special terms in the formula
        while (System.Text.RegularExpressions.Regex.IsMatch(formula, @"Emp\((A|N)\-(A|N)\)"))
        {
            formula = System.Text.RegularExpressions.Regex.Replace(formula,
                @"Emp\((A|N)\-(A|N)\)",
                match => ParseEmpTerm(match.Value).ToString());
        }

        // Replace complex terms in the formula
        while (System.Text.RegularExpressions.Regex.IsMatch(formula, @"(F\(-?\d+\)|A|N)\-(ModR|PR|SR)\:(L|DB|NB|B)\((E\(-?\d+\)|A|N|\-(.*?)\-)\)"))
        {
            formula = System.Text.RegularExpressions.Regex.Replace(formula,
                @"(F\(-?\d+\)|A|N)\-(ModR|PR|SR)\:(L|DB|NB|B)\((E\(-?\d+\)|A|N|\-(.*?)\-)\)",
                match => ParseComplexTerm(match.Value).ToString());
        }

       

        // Replace simple terms in the formula
        while (System.Text.RegularExpressions.Regex.IsMatch(formula, @"(L|NB|DB)\((A|N|E\(-?\d+\))\)"))
        {
            formula = System.Text.RegularExpressions.Regex.Replace(formula,
                @"(L|NB|DB)\((A|N|E\(-?\d+\))\)",
                match => ParseSimpleTerm(match.Value).ToString());
        }

        // Replace scale terms (these scale the change in accordance to the initial values)
        while (System.Text.RegularExpressions.Regex.IsMatch(formula, @"ScaleChg\((\d+)\,(\d+)\)"))
        {
            formula = System.Text.RegularExpressions.Regex.Replace(formula,
                @"ScaleChg\((\d+)\,(\d+)\)",
                match => ParseScaleChangeTerm(match.Value).ToString());
        }

        //DELETE THE DICTIONARY EVENTUALLY AND USE THE PARSE FUNCTIONS (WHICH I STILL NEED TO FINISH)
        // Define a mapping for the variables in the formula
        Dictionary<string, double> simpleStatValueDictionary = new Dictionary<string, double>
        {
            { "F-ModR:L(N)", fModLN},
            { "F-ModR:DB(N)", fModDbN},
            { "F-ModR:NB(N)", fModNbN},
            { "DB(A)", agent.Stats.DB },
            { "DB(N)", env.Stats.DB },
            { "NB(A)", agent.Stats.NB },
            { "NB(N)", env.Stats.NB },
            { "L(A)", agent.Stats.L },
            { "L(N)", env.Stats.L },
            { "L(F)", fl },
            { "DB(F)", fdb },
            { "NB(F)", fnb },
        };

        //made 2 dictionary's temporarily to test the the parse functions. This one has the variables that are already used by ParseSimpleTerms
        Dictionary<string, double> simpleFormulaStatValueDictionary = new Dictionary<string, double>
        {
            { "L(F)", fl },
            { "DB(F)", fdb },
            { "NB(F)", fnb },
        };

        // Merge enemy stats into the static dictionary
        /*
        foreach (var enemyStat in dynamicEnemyStats)
        {
            statValueDictionary[enemyStat.Key] = enemyStat.Value;
        }
        */

        // Extract the target value from the dictionary
        if (!simpleStatValueDictionary.TryGetValue(target, out double targetValue))
        {
            throw new ArgumentException($"The target '{target}' was not found in the variable mapping.");
        }
        

        // Replace variable placeholders in the formula with their actual values
        foreach (var variable in simpleFormulaStatValueDictionary)
        {
            formula = formula.Replace($"{variable.Key}", variable.Value.ToString());
        }

        // Evaluate custom functions
        formula = EvaluateCustomFunctionsUntilDone(formula);

        double predictorValue = EvaluateBasicExpression(formula);


        
        //check if it is calculating friend value
        /*
         * DON'T NEED BECAUSE USELESS. WILL USE SPECIAL TERMS TO ADJUST FOR EMPATHY
        bool isOtherVariable = target.Contains("(F)") || target.Contains("(N)");

        //if friend then adjust the predictor with empathy
        if (isOtherVariable)
        {
            double adjustedEmpathyLevel = agent.EmpathyLevel / 10;
            Debug.Log("Relationship value with friend for predictor: " + adjustedEmpathyLevel + " predictor value: " + predictorValue + " adjusted predictor value: " + predictorValue * adjustedEmpathyLevel);
            predictorValue *= adjustedEmpathyLevel;
            
        }
        */

        // Evaluate the final expression (without functions)
        return (predictor: predictorValue, change: predictorValue - targetValue);
    }

    private double CalculateTargetAdjustedEmpathyPRStat(double empathyLevel, double individualPRScale)
    {
        return 1+ (10 - empathyLevel) / 3 + (1 - individualPRScale);
    }

    private double CalculateFormulaAdjustedEmpathyPRStat(double empathyLevel, double individualPRScale)
    {
        return empathyLevel/10 + individualPRScale * 2/3;
    }

    private CharactersTestSO GetEnemyByIndex(CharactersTestSO agent, int index)
    {
        if (index <= 0 || index > agent.EnemyTest.Count)
        {
            return null;
        }

        return agent.EnemyTest[index - 1];
    }

    //get all the mod values for calculating formula/predictor adjusted with relationship value
    private (double l, double db, double nb) ReturnModValuesWithCharacter(CharactersTestSO character, CharactersTestSO targetCharacter, double relationshipValueWithCharacter = 1)
    {
        double l = 0;
        double db = 0;
        double nb = 0;
  
        foreach (RelationshipValueTest mod in character.RelationshipSheet.ModR) 
        { 
            if(mod.Identifiers == targetCharacter)
            {
                l += mod.Stats.L * relationshipValueWithCharacter;
                db += mod.Stats.DB * relationshipValueWithCharacter;
                nb += mod.Stats.NB * relationshipValueWithCharacter;
                break;
            }
        }

        return (l:  l, db: db, nb: nb);
    }


    //Working with E variables
    private Dictionary<string, double> ParseRelationshipStats(CharactersTestSO agent, CharactersTestSO env)
    {
        Dictionary<string, double> enemyStats = new Dictionary<string, double>();
        double lowestBelonging = double.MaxValue;
        CharactersTestSO lowestEnemy = null;

        // Identify enemy with the lowest average belonging
        foreach (var enemy in agent.EnemyTest)
        {
            double averageBelonging = (enemy.Stats.DB + enemy.Stats.NB) / 2;
            if (averageBelonging < lowestBelonging)
            {
                lowestBelonging = averageBelonging;
                lowestEnemy = enemy;
            }
        }

        if (lowestEnemy != null)
        {
            enemyStats["E(1)"] = lowestEnemy.Stats.L; // Replace with dynamic computation
            enemyStats["B(A)"] = GetRelationshipValue(agent, lowestEnemy); // Compute B(A)
        }

        // Add additional enemies dynamically (e.g., E(2), E(3))
        int index = 2;
        foreach (var enemy in agent.EnemyTest)
        {
            if (enemy != lowestEnemy)
            {
                enemyStats[$"E({index})"] = enemy.Stats.L; // Example stat
                index++;
            }
        }

        return enemyStats;
    }


    private double GetRelationshipValue(CharactersTestSO agent, CharactersTestSO target)
    {
        foreach (var relationship in agent.RelationshipSheet.PR)
        {
            if (relationship.Identifiers == target)
            {
                return (relationship.Stats.DB + relationship.Stats.NB) / 2;
            }
        }
        return 0; // Default if no relationship found
    }


    string EvaluateCustomFunctionsUntilDone(string formula)
    {
        bool containsCustomFunctions = true;

        // Keep evaluating custom functions until no custom functions remain
        while (containsCustomFunctions)
        {
            string previousFormula = formula;

            // Resolve custom functions (Mathf.Max, Mathf.Min) in the formula
            formula = EvaluateCustomFunctions(formula);

            // If the formula has changed after evaluation, continue processing
            containsCustomFunctions = !formula.Equals(previousFormula);
        }

        // Once all custom functions are resolved, evaluate the basic expression
        return EvaluateBasicExpression(formula).ToString();
    }

    string EvaluateCustomFunctions(string formula)
    {
        bool hasCustomFunction = false;

        // Keep looping until no more Mathf.Max or Mathf.Min are present
        while (formula.Contains("Mathf.Max") || formula.Contains("Mathf.Min"))
        {
            Debug.Log("Formula while looping for custom functions: " + formula);
            // Look for the next occurrence of Mathf.Max or Mathf.Min
            int startIndex = formula.LastIndexOf("Mathf.Max");

            bool isMaxFunction = true;
            if (startIndex < formula.LastIndexOf("Mathf.Min"))
            {
                startIndex = formula.LastIndexOf("Mathf.Min");
                isMaxFunction = false;
            }

            if (startIndex == -1) break;

            string functionName = isMaxFunction ? "Mathf.Max" : "Mathf.Min";
            int openParen = formula.IndexOf('(', startIndex);
            int closeParen = FindMatchingParenthesis(formula, openParen);

            // Extract the arguments between parentheses
            string args = formula.Substring(openParen + 1, closeParen - openParen - 1).Trim();

            // Split the arguments properly
            string[] splitArgs = SplitArguments(args);

            if (splitArgs.Length != 2)
            {
                Debug.LogError("Error: Expected exactly 2 arguments for " + functionName + " but found " + splitArgs.Length);
                return formula; // Or handle the error appropriately
            }

            // Now resolve the arguments by evaluating them fully before computing the result
            double arg1 = EvaluateBasicExpression(splitArgs[0].Trim());
            double arg2 = EvaluateBasicExpression(splitArgs[1].Trim());

            // Compute result using Mathf.Max or Mathf.Min
            double result = isMaxFunction
                ? Mathf.Max((float)arg1, (float)arg2)
                : Mathf.Min((float)arg1, (float)arg2);

            // Replace the function call with its result
            formula = formula.Substring(0, startIndex) + result.ToString() + formula.Substring(closeParen + 1);

            hasCustomFunction = true;  // We found and processed a custom function
        }

        // If no custom functions were found, return the formula as is
        if (!hasCustomFunction)
        {
            return formula;
        }

        return formula;
    }

    // Helper method to split arguments while handling nested parentheses
    string[] SplitArguments(string args)
    {
        var parts = new List<string>();
        int depth = 0;
        string currentPart = "";

        for (int i = 0; i < args.Length; i++)
        {
            char c = args[i];

            if (c == ',' && depth == 0)
            {
                parts.Add(currentPart.Trim());
                currentPart = "";
            }
            else
            {
                currentPart += c;

                if (c == '(') depth++;
                if (c == ')') depth--;
            }
        }

        if (currentPart.Length > 0)
            parts.Add(currentPart.Trim());

        return parts.ToArray();

    }
    /// <summary>
    /// Finds the matching closing parenthesis for a given opening parenthesis.
    /// </summary>
    int FindMatchingParenthesis(string formula, int openParenIndex)
    {
        int balance = 1;
        for (int i = openParenIndex + 1; i < formula.Length; i++)
        {
            if (formula[i] == '(') balance++;
            if (formula[i] == ')') balance--;
            if (balance == 0) return i;
        }
        throw new Exception("Unmatched parenthesis in formula.");
    }


    /// Evaluates a mathematical expression as a double.

    double EvaluateBasicExpression(string expression)
    {
        try
        {
            DataTable table = new DataTable();
            object result = table.Compute(expression, string.Empty);
            return Convert.ToDouble(result);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error evaluating expression: {expression}. Exception: {ex.Message}");
            return 0;
        }
    }
}
