using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TextCore.Text;

//Utility functions for DM Functions
public static class DMCalculationFunctions
{
    //Need to be able to get Enemy entity index eventually
    private static readonly Regex ComplexTermRegex = new(
       @"(?<entity>F\((?<entity_index>-?\d+)\)|A|N)-(?<relation>ModR|PR|SR):(?<stat>L|DB|NB|B)\((?<target>E\((?<target_index>-?\d+)\)|N|A|-(?<specific>\w+)-)\)",
       RegexOptions.Compiled);
        //A-ModR:DB(N)

    private static readonly Regex SimpleTermRegex = new(
        @"(?:(?<stat>L|DB|NB)\((?<target>A|N|F\((?<fIndex>-?\d+)\))\)|E\((?<eIndex>-?\d+)\))",
        RegexOptions.Compiled);

    private static readonly Regex EmpTermRegex = new(
        @"Emp\((A|N)\-(A|N)\)",
        RegexOptions.Compiled);

    private static readonly Regex ScaleChgRegex = new(
        @"ScaleChg\((?<initial_value>\d+)\,(?<change_value>\d+)\)",
        RegexOptions.Compiled);
    // Function to parse dynamic terms like N-ModR:L(E(1))
    //Could add F- or friend into this later on. Friend as target too
    public static double ParseComplexTerm(Match match, CharacterMainCPort agent, CharacterMainCPort env, RelationshipNode envInAgentRPTNode)
    {

        if (!match.Success)
        {
            throw new ArgumentException("Invalid match passed to ParseComplexTerm");
        }
        //1. Find Entity
        //2. Find Target
        //3. Find Target in Relationship (from Entity)
        //4. Find average if needed
        string entity = match.Groups["entity"].Value;
        string relationType = match.Groups["relation"].Value;
        string stat = match.Groups["stat"].Value;
        string target = match.Groups["target"].Value;
        //see if it is averaging target. Usually this means this has multiple targets
        List<CharacterMainCPort> entityObjects = new();
        //1. Get entity
        if (entity == "A")
        {
            entityObjects.Add(agent);
        }
        else if (entity == "N")
        {
            entityObjects.Add(env);
        }
        else if (entity.StartsWith("F("))
        {
            int entityIndex = int.Parse(match.Groups["entity_index"].Value);
            if (entityIndex == -1)
            {
                entityObjects = agent.characterPsyche.FriendsRanked.ToList();
            }
            else
            {
                entityObjects.Add(agent.characterPsyche.FriendsRanked[entityIndex]);
            }
            //Need to do E(
        }

        //2. Obtain Target
        //Agent or env need to be found through tree  and enemies and friends need to be found through the list of enemies and friends
        //Agent or Env: Character Main CPort to include the character physical and psyche to find in the relationship trees
        //Enemies
        List<CharacterMainCPort> targetAgentOrEnv = new();
        List<SubIdentifierNode> targetEnemiesOrFriends = new();
        if (target == "A")
        {
            targetAgentOrEnv.Add(agent);
        }
        else if (target == "N")
        {
            targetAgentOrEnv.Add(env);
            //enemy. Default is agent's enemies. If want to do env enemies, could do N-env and would have to implement
        }
        else if (target.StartsWith("E("))
        {
            int targetIndex = int.Parse(match.Groups["target_index"].Value);
            //this is when E(-1). So want to see all of the average of the enemy
            //in future if needed not average but culmulative sum, then can use E(-2)
            if (targetIndex == -1)
            {
                //shallow copy so not by reference
                targetEnemiesOrFriends = agent.characterPsyche.EnemiesCPortToSubNode.Values.ToList();
            }
            //normal. Like E(0) or E(1)
            else
            {
                targetEnemiesOrFriends.Add(GetEnemyByIndex(agent, targetIndex));
            }
        }
        else if (target.StartsWith("-"))
        {
            //CharactersDictionary is the list of all characters. NEED TO DO THIS
            foreach (CharacterMainCPort character in GlobalCharacterDictionary.AllCharacters)
            {
                string specific = match.Groups["specific"].Value;
                if (character.Name == specific)
                {
                    targetAgentOrEnv.Add(character);
                    break;
                }
            }
        }

        if (targetAgentOrEnv != null && targetEnemiesOrFriends != null)
        {
            // 1. Get total of all stats of interest for target
            // 2. Divide by totalCount to get average
            double statOfInterest = 0;
            int totalCount = 0;
            //loop through all entities
            foreach (CharacterMainCPort entityObject in entityObjects)
            {
                // Select the appropriate relationship sheet based on the relation type

                //Get Target from Entity
                if (targetAgentOrEnv.Count == 0 && targetEnemiesOrFriends.Count == 0)
                {
                    Debug.Log("No targetSO found when calculating average. Returning 0.");
                    return 0;
                }
                else if (entityObjects.Count == 0)
                {
                    Debug.Log("No entitySO found when calculating average. Returning 0.");
                    return 0;
                }
                else
                {
                    //If entity is agent and target is env, then will use parameter
                    //If entity is agent and target is not env, then will have to find it either in enemy or friend
                    //If entity is not agent, then will have to find identity in the entity's RPT. If doesn't exist, then cannot calculate

                    //Find Target values in agent
                    List<RelationshipValues> targetInAgentRelationshipValues = null;
                    if (entity == "A" && target == "N")
                    {
                        //Only ModR right now because it is same as PR. Need to implement SR
                        targetInAgentRelationshipValues.Add(
                            relationType switch
                            {
                                "ModR" => envInAgentRPTNode.ModRValues,
                                _ => throw new ArgumentException($"Unknown relationship type: {relationType}")
                            }
                        );
                        //perception of self
                    }
                    else if (entity == "A" && target == "A")
                    {
                        //Find the main relationship node which is context = none
                        SubIdentifierNode selfIdentifierNode = agent.characterPsyche.GetSelfSubIdentifier();
                        RelationshipNode mainRelationshipNode = selfIdentifierNode.RelationshipNodes.FirstOrDefault(r => r.ActionContext == EnumActionCharacteristics.Main);

                        targetInAgentRelationshipValues.Add(
                                relationType switch
                                {
                                    "ModR" => mainRelationshipNode.ModRValues,
                                    _ => throw new ArgumentException($"Unknown relationship type: {relationType}")
                                }
                            );
                    }
                    else if (entity == "N" && target == "A")
                    {
                        List<EnumAppearanceCharacteristics> agentAppearances = agent.characterPhysical.appearanceCharacteristics;
                        List<EnumActionCharacteristics> agentActions = agent.characterPhysical.actionCharacteristics;
                        SubIdentifierNode envSubIdentifierNode = env.characterPsyche.GetSelfSubIdentifier();
                        SubIdentifierNode foundAgentInEnvRPT = AdaptiveIdentifierFunctions.FindSubidentifierNodeWithAppearanceAndAction(env.characterPsyche.RelationshipPersonalTree, envSubIdentifierNode.Parent.Identifier, agentAppearances, agentActions);

                        RelationshipNode mainRelationshipNode = foundAgentInEnvRPT.RelationshipNodes.FirstOrDefault(r => r.ActionContext == EnumActionCharacteristics.Main);

                        targetInAgentRelationshipValues.Add(
                                relationType switch
                                {
                                    "ModR" => mainRelationshipNode.ModRValues,
                                    _ => throw new ArgumentException($"Unknown relationship type: {relationType}")
                                }
                            );
                    }
                    else if (entity == "N" && target == "N")
                    {
                        SubIdentifierNode envSubIdentifierNode = env.characterPsyche.GetSelfSubIdentifier();
                        RelationshipNode mainRelationshipNode = envSubIdentifierNode.RelationshipNodes.FirstOrDefault(r => r.ActionContext == EnumActionCharacteristics.Main);
                        targetInAgentRelationshipValues.Add(
                                relationType switch
                                {
                                    "ModR" => mainRelationshipNode.ModRValues,
                                    _ => throw new ArgumentException($"Unknown relationship type: {relationType}")
                                }
                            );

                        //goes through all the enemy sheet
                    }
                    else if (target.StartsWith("E("))
                    {
                        foreach (SubIdentifierNode enemyNode in targetEnemiesOrFriends)
                        {
                            //Get main RN (action context = none)
                            RelationshipNode mainRelationshipNode = enemyNode.GetMainRelationshipNode();
                            if (mainRelationshipNode != null)
                            {
                                targetInAgentRelationshipValues.Add(
                                    relationType switch
                                    {
                                        "ModR" => mainRelationshipNode.ModRValues,
                                        _ => throw new ArgumentException($"Unknown relationship type: {relationType}")
                                    }
                                );
                            }
                            else
                            {
                                Debug.LogWarning("No main relationship node found for enemy node when parsing complex terms");
                            }

                        }
                    }


                    foreach (var targetInAgentRelationshipValue in targetInAgentRelationshipValues)
                    {
                        statOfInterest += stat switch
                        {
                            "L" => targetInAgentRelationshipValue.LivelihoodValue,
                            "DB" => targetInAgentRelationshipValue.DefensiveBelongingValue,
                            "NB" => targetInAgentRelationshipValue.NurtureBelongingValue,
                            "B" => (targetInAgentRelationshipValue.DefensiveBelongingValue + targetInAgentRelationshipValue.NurtureBelongingValue) / 2,
                            _ => throw new ArgumentException($"Unknown stat: {stat}")
                        };

                        //Found a stat and record an increase in the count
                        totalCount += 1;

                    }
                }
            }
            //return average
            Debug.Log("Average Target of " + target + " is " + statOfInterest / totalCount + " Total count: " + totalCount + " stat of interest value: " + statOfInterest);
            return statOfInterest / totalCount;
        }

        throw new ArgumentException($"Unable to parse complex term: {match}");
    }

    public static SubIdentifierNode GetEnemyByIndex(CharacterMainCPort agent, int index)
    {
        if (index <= 0 || index > agent.characterPsyche.EnemiesRanked.Count)
        {
            return null;
        }

        CharacterMainCPort enemy = agent.characterPsyche.EnemiesRanked[index - 1];

        return agent.characterPsyche.EnemiesCPortToSubNode[enemy];
    }

    public static double ParseScaleChangeTerm(Match match)
    {
        // Match the pattern "ScaleChg(10,3)"
        if (!match.Success)
        {
            throw new ArgumentException("Invalid match passed to ParseScaleChange");
        }



        string special = match.Groups["special"].Value;
        double initialValue = double.Parse(match.Groups["initial_value"].Value);
        double changeValue = double.Parse(match.Groups["change_value"].Value);

        return ScaleSurvivalStatChange(changeValue, initialValue);


        // Handle unmatched terms
        throw new ArgumentException($"Unable to parse special term: {match}");
    }

    public static double ParseSimpleTerm(Match match, CharacterMainCPort agent, CharacterMainCPort env)
    {
        // Simple terms such as L(E), DB(N), etc.
        if (!match.Success)
        {
            throw new ArgumentException("Invalid match passed to ParseSimpleTerm");
        }


        // Extract the stat and target from the matched term
        string stat = match.Groups["stat"].Value;
        string target = match.Groups["target"].Value;

        // Select the correct stat based on the target
        if (target == "A")
        {
            // Return the value from the agent's stats based on the stat
            return stat switch
            {
                "L" => agent.characterPhysical.Stats.L,
                "DB" => agent.characterPhysical.Stats.DB,
                "NB" => agent.characterPhysical.Stats.NB,
                _ => throw new ArgumentException($"Unknown stat: {stat}")
            };
        }
        else if (target == "N")
        {
            // Return the value from the env's stats based on the stat
            return stat switch
            {
                "L" => env.characterPhysical.Stats.L,
                "DB" => env.characterPhysical.Stats.DB,
                "NB" => env.characterPhysical.Stats.NB,
                _ => throw new ArgumentException($"Unknown stat: {stat}")
            };
        }
        else if (target.StartsWith("E(") || target.StartsWith("F("))
        {
            // Handle case when target is "E" or "F"
            int index = -2;

            List<CharacterMainCPort> targetEOrF = new List<CharacterMainCPort>();
            if (target.StartsWith("E("))
            {
                targetEOrF = agent.characterPsyche.EnemiesRanked;
                index = int.Parse(match.Groups["eIndex"].Value);
            }
            else if (target.StartsWith("F("))
            {
                targetEOrF = agent.characterPsyche.FriendsRanked;
                index = int.Parse(match.Groups["fIndex"].Value);
            }

            if (index == -1)
            {
                // Collect the stats from all enemies in the EnemyTest list
                List<double> targetStats = new List<double>();

                foreach (var eOrFCPort in targetEOrF)
                {

                    double targetStat = stat switch
                    {
                        "L" => eOrFCPort.characterPhysical.Stats.L,
                        "DB" => eOrFCPort.characterPhysical.Stats.DB,
                        "NB" => eOrFCPort.characterPhysical.Stats.NB,
                        _ => throw new ArgumentException($"Unknown stat: {stat}")
                    };

                    // Add the stat value to the list
                    targetStats.Add(targetStat);
                }

                // Calculate the average of the collected stats
                if (targetStats.Count > 0)
                {
                    return targetStats.Average();
                }
                else
                {
                    throw new ArgumentException("No friends or enemies found in character.");
                }
            }
            else
            {
                // If index is not -1, handle the individual enemy
                var eOrFCPort = targetEOrF.ElementAtOrDefault(index);
                if (eOrFCPort != null)
                {

                    return stat switch
                    {
                        "L" => eOrFCPort.characterPhysical.Stats.L,
                        "DB" => eOrFCPort.characterPhysical.Stats.DB,
                        "NB" => eOrFCPort.characterPhysical.Stats.NB,
                        _ => throw new ArgumentException($"Unknown stat: {stat}")
                    };
                }
                else
                {
                    throw new ArgumentException($"Invalid enemy or friend index: {index}");
                }
            }
        }
        else
        {
            // Handle unmatched targets
            throw new ArgumentException($"Unsupported target: {target}");
        }


        // Handle unmatched terms
        throw new ArgumentException($"Unable to parse simple term: {match}");
    }

    public static double ParseEmpTerm(Match match, CharacterMainCPort agent, CharacterMainCPort env, RelationshipNode envInAgentRPTRelationshipNode)
    {
        // Match the pattern "Emp(A-N)" or "Emp(N-A)"
        //NOTE: Could get rid of Emp(N-A). When is this ever going to be used??
        if (!match.Success)
        {
            throw new ArgumentException("Invalid match passed to ParseEmpTerm");
        }


        string origin = match.Groups["origin"].Value;
        string end = match.Groups["end"].Value;

        // Determine origin empathy level and character
        double originEmpathyLevel = 0;
        CharacterPsyche originCharacter = null;
        CharacterPsyche endCharacter = null;

        if (origin == "N")
        {
            originEmpathyLevel = env.characterPsyche.EmpathyLevel;
            originCharacter = env.characterPsyche;
        }
        else if (origin == "A")
        {
            originEmpathyLevel = agent.characterPsyche.EmpathyLevel;
            originCharacter = agent.characterPsyche;
        }

        // Determine end character that empathy is towards
        //NOTE: Will not use for now since I don't see how Emp(N-A) will be used so assume it is always Emp(A-N). If so, can simplify the regex just to detect Emp not Emp(N-A) or Emp(A-N)
        if (end == "A")
            endCharacter = agent.characterPsyche;
        else if (end == "N")
            endCharacter = env.characterPsyche;

        // Calculate the PR scale
        //TO-DO: Change PR to MODR since thinking about getting rid of PR
        RelationshipValues envInAgentRelationshipValues = envInAgentRPTRelationshipNode.ModRValues;
        double modRScale = envInAgentRelationshipValues.NurtureBelongingValue + envInAgentRelationshipValues.DefensiveBelongingValue;
        modRScale /= 100;

        return CalculateFormulaAdjustedEmpathyPRStat(originEmpathyLevel, modRScale);

        // Handle unmatched terms
        throw new ArgumentException($"Unable to parse special term: {match}");
    }

    public static double CalculateFormulaAdjustedEmpathyPRStat(double empathyLevel, double individualPRScale)
    {
        return empathyLevel / 10 + individualPRScale * 2 / 3;
    }

    //get all the mod values for calculating formula/predictor adjusted with relationship value
    //Will use the relationship node of the env from the character's view
    public static (double l, double db, double nb) ReturnModValuesWithCharacter(RelationshipNode envInCharacterRelationshipNode, double relationshipValueWithCharacter = 1)
    {
        RelationshipValues modRRelationshipValues = envInCharacterRelationshipNode.ModRValues;
        double l = modRRelationshipValues.LivelihoodValue * relationshipValueWithCharacter;
        double db = modRRelationshipValues.DefensiveBelongingValue * relationshipValueWithCharacter;
        double nb = modRRelationshipValues.NurtureBelongingValue * relationshipValueWithCharacter;

        return (l: l, db: db, nb: nb);
    }


    public static string EvaluateCustomFunctionsUntilDone(string formula)
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
        Debug.Log("Final formula: " + formula);
        // Once all custom functions are resolved, evaluate the basic expression
        return EvaluateBasicExpression(formula).ToString();
    }

    public static string EvaluateCustomFunctions(string formula)
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

    private static string TranslateEnumToString(EnumPersonalityStats stat)
    {
        if (stat == EnumPersonalityStats.L)
        {
            return "L(A)";
        }
        else if (stat == EnumPersonalityStats.NB)
        {
            return "NB(A)";
        }
        else if (stat == EnumPersonalityStats.DB)
        {
            return "DB(A)";
        }
        else if (stat == EnumPersonalityStats.FL)
        {
            return "L(F)";
        }
        else if (stat == EnumPersonalityStats.FDB)
        {
            return "DB(F)";
        }
        else if (stat == EnumPersonalityStats.FNB)
        {
            return "NB(F)";
        }
        else if (stat == EnumPersonalityStats.NL)
        {
            return "L(N)";
        }
        else if (stat == EnumPersonalityStats.NNB)
        {
            return "NB(N)";
        }
        else if (stat == EnumPersonalityStats.NDB)
        {
            return "DB(N)";
        }
        return "";
    }

    private static EnumPersonalityStats TranslatePersonalityStringToEnum(string target)
    {
        if (target == "L(A)")
        {
            return EnumPersonalityStats.L;
        }
        else if (target == "DB(A)")
        {
            return EnumPersonalityStats.DB;
        }
        else if (target == "NB(A)")
        {
            return EnumPersonalityStats.NB;
        }
        else if (target == "L(F)")
        {
            return EnumPersonalityStats.FL;
        }
        else if (target == "DB(F)")
        {
            return EnumPersonalityStats.FDB;
        }
        else if (target == "NB(F)")
        {
            return EnumPersonalityStats.FNB;
        }
        else if (target == "L(N)")
        {
            return EnumPersonalityStats.NL;
        }
        else if (target == "DB(N)")
        {
            return EnumPersonalityStats.NDB;
        }
        else if (target == "NB(N)")
        {
            return EnumPersonalityStats.NNB;
        }
        
        throw new ArgumentException($"Unknown target: {target}");
    }

    // Helper method to split arguments while handling nested parentheses
    private static string[] SplitArguments(string args)
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
    private static int FindMatchingParenthesis(string formula, int openParenIndex)
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
    public static double EvaluateBasicExpression(string expression)
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

    //Adjust change based on starting value
    public static double ScaleSurvivalStatChange(double delta, double current, double max = 100)
    {
        return delta * (1 + (max - current) / max);
    }

    //Ni decision making
    public static DMReturnPredictorCalculations CalculateSimplePositiveAndNegativePredictorChange(
        RelationshipValues modR, List<Perspective> perspectives, EnumPersonalityStats targetStat, AllStats allInitialStats, int habitCountDecision, 
        CharacterMainCPort agent, CharacterMainCPort env, RelationshipNode envInAgentRPTNode)
    {
        double largestPositivePredictorValue = double.MinValue;
        double largestNegativePredictorValue = double.MaxValue;
        double largestPositivePredictorChange = double.MinValue;
        double largestNegativePredictorChange = double.MaxValue;
        Perspective positivePerspective = null;
        Perspective negativePerspective = null;
        EnumPersonalityStats largestPositivePredictorStat = targetStat; // Default to the target stat
        EnumPersonalityStats largestNegativePredictorStat = targetStat; // Default to the target stat
        

        string targetStatString = TranslateEnumToString(targetStat);

        //Sort desired target at front (always at front regardless of habitcounter) then organize by habit
        List<Perspective> sortedHabitPerspectives = perspectives.OrderByDescending(p => p.Target == targetStatString)
            .ThenByDescending(p => p.HabitCounter)
            .ToList();

        //get the targetStat change that matters
        //This is the goal and perspective of the decision
        for (int i = 0; i < sortedHabitPerspectives.Count && i <= agent.characterPsyche.PerspectiveAbility; i++)
        {

            string target;
            double predictorValue = 0;
            double changeValue = 0;

            //Will only consider if not enough perspective ability to cover it all
            if (i == agent.characterPsyche.PerspectiveAbility)
            {
                target = targetStatString;
                //Don't have target looking into L(F), L(N), etc.
                switch (target)
                {
                    case "L(A)":
                        changeValue = modR.LivelihoodValue;
                        break;
                    case "DB(A)":
                        changeValue = modR.DefensiveBelongingValue;
                        break;
                    case "NB(A)":
                        changeValue = modR.NurtureBelongingValue;
                        break;
                    default:
                        continue; // Skip if target is not recognized (I don't have ModR for others yet)
                }

                predictorValue = changeValue + allInitialStats.StatOfInterest(target);

            }
            else
            {
                target = sortedHabitPerspectives[i].Target;
                (predictorValue, changeValue) = Translate_String_To_Formula_Calculations(sortedHabitPerspectives[i].Predictor,
                    sortedHabitPerspectives[i].Target, agent, env, envInAgentRPTNode);
            }
                


            //all stat calculations will be adjusted based on initial stats
            double adjustedChangeValue = ScaleSurvivalStatChange(changeValue, allInitialStats.StatOfInterest(target));

            Debug.Log(sortedHabitPerspectives[i].Name + " predictor value: " + predictorValue + " Change value: " + changeValue + "adjusted change value: " + adjustedChangeValue);

            //Add the habit contribution from both the decision and the perspective
            double habitDecisionContribution = HabitContribution(habitCountDecision, agent.characterPsyche.MaxHabitCounter, agent.characterPsyche.HabitualTendencies);
            double habitPerspectiveContribution = HabitContribution(sortedHabitPerspectives[i].HabitCounter, agent.characterPsyche.MaxHabitCounter, agent.characterPsyche.HabitualTendencies);

            double totalHabitContribution = habitDecisionContribution + habitPerspectiveContribution;

            double adjustedChangeValueWithHabit = adjustedChangeValue + totalHabitContribution;
            double predictorValueWithHabit = predictorValue + adjustedChangeValueWithHabit;
            Debug.Log("Adjusted change value with habit: " + adjustedChangeValueWithHabit + " predictor value with habit: " + predictorValueWithHabit);
            //evaluate the greatest change for perspectives
            if (adjustedChangeValueWithHabit > largestPositivePredictorChange)
            {
                largestPositivePredictorValue = predictorValueWithHabit;
                largestPositivePredictorChange = adjustedChangeValueWithHabit;
                positivePerspective = sortedHabitPerspectives[i];
                largestPositivePredictorStat = TranslatePersonalityStringToEnum(target);
            }

            if (adjustedChangeValueWithHabit < largestNegativePredictorChange)
            {
                Debug.Log("Checking negative predictor with: " + sortedHabitPerspectives[i].Name);
                largestNegativePredictorValue = predictorValueWithHabit;
                largestNegativePredictorChange = adjustedChangeValueWithHabit;
                negativePerspective = sortedHabitPerspectives[i];
                largestNegativePredictorStat = TranslatePersonalityStringToEnum(target);
            }


        }
        DMReturnPredictorCalculations returnValues = new();
        returnValues.SetPredictorValues(largestPositivePredictorValue, largestNegativePredictorValue);
        returnValues.SetChangeValues(largestPositivePredictorChange, largestNegativePredictorChange);
        returnValues.SetPerspectives(positivePerspective, negativePerspective);
        returnValues.SetPersonalityStats(largestPositivePredictorStat, largestNegativePredictorStat);


        return returnValues;
    }

    //Change return value to class
    public static DMReturnPredictorCalculations CalculateComplexPositiveAndNegativePredictorChange(
        RelationshipValues goalModR, DecisionSO decisionSO, EnumPersonalityStats targetStat, AllStats allInitialStats, int habitCountDecision,
        CharacterMainCPort agent, CharacterMainCPort env, RelationshipNode envInAgentRPTNode)
    {
        double largestPositivePredictorValue = double.MinValue;
        double largestNegativePredictorValue = double.MaxValue;
        double largestPositivePredictorChange = double.MinValue;
        double largestNegativePredictorChange = double.MaxValue;
        EnumPersonalityStats largestPositivePredictorStat = targetStat; // Default to the target stat
        EnumPersonalityStats largestNegativePredictorStat = targetStat; // Default to the target stat
        Perspective positivePerspective = null;
        Perspective negativePerspective = null;

        //Determine the complex goal step
        int complexGoalStep = 0;

        //Saved or remembered step in decision
        if (agent.characterPsyche.Decision_Step_Tracker.TryGetValue(decisionSO, out int step))
        {
            complexGoalStep = step;
        }
        else
        {
            //If doesn't exist, then create dictionary
            agent.characterPsyche.Decision_Step_Tracker[decisionSO] = 0;
        }

        //Perspective of overall goal
        List<Perspective> goalPerspectives = decisionSO.Perspectives;
        //Perspective of the action in complex action currently on
        DecisionSO actionDecisionSO = decisionSO.ComplexGoalListOfDecisions[complexGoalStep];
        List<Perspective> actionPerspectives = actionDecisionSO.Perspectives;

        List<Perspective> sortedHabitGoalPerspectives = goalPerspectives.OrderByDescending(p => p.Target == targetStat.ToString()).
            ThenByDescending(p => p.HabitCounter).ToList();
        List<Perspective> sortedHabitActionPerspectives = actionPerspectives.OrderByDescending(p => p.Target == targetStat.ToString()).
            ThenByDescending(p => p.HabitCounter).ToList();



        //get the targetStat change that matters
        for (int i = 0; i < sortedHabitGoalPerspectives.Count && i <= agent.characterPsyche.PerspectiveAbility; i++)
        {
            string target;
            double predictorValue = 0;
            double changeValue = 0;

            //if doesn't go through all perspectives, will consider ModR
            if (i == agent.characterPsyche.PerspectiveAbility)
            {
                target = targetStat.ToString();
                switch (target)
                {
                    case "L(A)":
                        changeValue = goalModR.LivelihoodValue;
                        break;
                    case "DB(A)":
                        changeValue = goalModR.DefensiveBelongingValue;
                        break;
                    case "NB(A)":
                        changeValue = goalModR.NurtureBelongingValue;
                        break;
                    default:
                        continue; // Skip if target is not recognized (I don't have ModR for others yet)
                }

                predictorValue = changeValue + allInitialStats.StatOfInterest(target);
            }
            else
            {
                target = sortedHabitGoalPerspectives[i].Target;

                (predictorValue, changeValue) = Translate_String_To_Formula_Calculations(sortedHabitGoalPerspectives[i].Predictor,
                    sortedHabitGoalPerspectives[i].Target, agent, env, envInAgentRPTNode);
            }



            double adjustedChangeValue = ScaleSurvivalStatChange(changeValue, allInitialStats.StatOfInterest(target));

            //adjust value depending on which step and progress inclination

            //calculate selfefficacy score. This will change the amount of steps it is on
            //Adjusted so self-efficacy is maxed at 100
            double selfEfficacyScore = ((100 - agent.characterPsyche.SelfEfficacy * 150 / 100 + 50) / 100);
            //If high in self-efficacy, feels like there's not a lot of steps to get to goal. Lower self-efficacy score is better 
            double totalStepsPerceived = decisionSO.ComplexGoalListOfDecisions.Count * selfEfficacyScore;
            //how close are they to goal. Closer they are, the more they are enticed by it
            double progressPerceived = Math.Min(complexGoalStep + 1 / totalStepsPerceived, 1);

            //Adjusted score based on how close they feel they are to goal and how much they care about progress with how much progress they feel they are making
            //predictorValue - changeValue = starting value.
            //So calculate the change adjusted and add with the starting value at end
            double goalAdjustedPredictorValue = changeValue * progressPerceived + agent.characterPsyche.ProgressInclination / totalStepsPerceived + (predictorValue - changeValue);

            double goalAdjustedChangeValue = changeValue + goalAdjustedPredictorValue - predictorValue;

            //Add habit contribution
            double habitDecisionContribution = HabitContribution(habitCountDecision, agent.characterPsyche.MaxHabitCounter, agent.characterPsyche.HabitualTendencies);
            double habitPerspectiveContribution = HabitContribution(sortedHabitGoalPerspectives[i].HabitCounter, agent.characterPsyche.MaxHabitCounter, agent.characterPsyche.HabitualTendencies);
            double totalHabitContribution = habitDecisionContribution + habitPerspectiveContribution;
            double goalAdjustedChangeValueWithHabit = goalAdjustedChangeValue + totalHabitContribution;
            double goalAdjustedPredictorValueWithHabit = goalAdjustedPredictorValue + totalHabitContribution;

            Debug.Log("Name of goal perspective: " + sortedHabitGoalPerspectives[i].Name + " Predictor Value: " + predictorValue +
                " Goal predicted value with progression values + habit: " + goalAdjustedPredictorValueWithHabit +
                " Goal adjusted change value with habit: " + goalAdjustedChangeValueWithHabit);


            //evaluate the greatest change for perspectives
            if (goalAdjustedChangeValueWithHabit > largestPositivePredictorChange)
            {
                largestPositivePredictorValue = goalAdjustedPredictorValueWithHabit;
                largestPositivePredictorChange = goalAdjustedChangeValueWithHabit;
                positivePerspective = sortedHabitGoalPerspectives[i];
                largestPositivePredictorStat = TranslatePersonalityStringToEnum(target);
            }

            if (goalAdjustedChangeValueWithHabit < largestNegativePredictorChange)
            {
                largestNegativePredictorValue = goalAdjustedPredictorValueWithHabit;
                largestNegativePredictorChange = goalAdjustedChangeValueWithHabit;
                negativePerspective = sortedHabitGoalPerspectives[i];
                largestNegativePredictorStat = TranslatePersonalityStringToEnum(target);
            }


        }

        //Alongside the goal, will also look at the specific action required
        for (int i = 0; i < sortedHabitActionPerspectives.Count && i <= agent.characterPsyche.PerspectiveAbility; i++)
        {
            string target;
            double predictorValue = 0;
            double changeValue = 0;

            //if doesn't go through all perspectives, will consider ModR
            if (i == agent.characterPsyche.PerspectiveAbility)
            {
                target = targetStat.ToString();

                //uses goal ModR
                switch (target)
                {
                    case "L(A)":
                        changeValue = goalModR.LivelihoodValue;
                        break;
                    case "DB(A)":
                        changeValue = goalModR.DefensiveBelongingValue;
                        break;
                    case "NB(A)":
                        changeValue = goalModR.NurtureBelongingValue;
                        break;
                    default:
                        continue; // Skip if target is not recognized (I don't have ModR for others yet)
                }

                predictorValue = changeValue + allInitialStats.StatOfInterest(target);
            }
            else
            {
                target = sortedHabitActionPerspectives[i].Target;
                //check for internal opportunities. See greatest change for what is highest need for 

                (predictorValue, changeValue) = Translate_String_To_Formula_Calculations(sortedHabitGoalPerspectives[i].Predictor,
                    sortedHabitGoalPerspectives[i].Target, agent, env, envInAgentRPTNode);
            }



            double adjustedChangeValue = ScaleSurvivalStatChange(changeValue, allInitialStats.StatOfInterest(target));

            //Get habit contribution
            double habitDecisionContribution = HabitContribution(decisionSO.HabitCounter, agent.characterPsyche.MaxHabitCounter, agent.characterPsyche.HabitualTendencies);
            double habitPerspectiveContribution = HabitContribution(sortedHabitActionPerspectives[i].HabitCounter, agent.characterPsyche.MaxHabitCounter, agent.characterPsyche.HabitualTendencies);
            double totalHabitContribution = habitDecisionContribution + habitPerspectiveContribution;

            double adjustedChangeValueWithHabit = adjustedChangeValue + totalHabitContribution;
            double predictorValueWithHabit = predictorValue + adjustedChangeValueWithHabit;

            //evaluate the greatest change for perspectives
            if (adjustedChangeValueWithHabit > largestPositivePredictorChange)
            {
                largestPositivePredictorValue = predictorValueWithHabit;
                largestPositivePredictorChange = adjustedChangeValueWithHabit;
                positivePerspective = sortedHabitActionPerspectives[i];
                largestPositivePredictorStat = TranslatePersonalityStringToEnum(target);
            }

            if (adjustedChangeValueWithHabit < largestNegativePredictorChange)
            {
                largestNegativePredictorValue = predictorValue;
                largestNegativePredictorChange = changeValue;
                negativePerspective = sortedHabitActionPerspectives[i];
                largestNegativePredictorStat = TranslatePersonalityStringToEnum(target);
            }



        }

        DMReturnPredictorCalculations returnValues = new DMReturnPredictorCalculations();
        returnValues.SetPredictorValues(largestPositivePredictorValue, largestNegativePredictorValue);
        returnValues.SetChangeValues(largestPositivePredictorChange, largestNegativePredictorChange);
        returnValues.SetPerspectives(positivePerspective, negativePerspective);
        returnValues.SetPersonalityStats(largestPositivePredictorStat, largestNegativePredictorStat);


        return returnValues;
    }

    public static double OpportunismAdjustment(double stat, double opportunismLevel)
    {
        return stat * opportunismLevel / 50;
    }

    public static double HabitContribution(int habitCounter, int maxHabitCounter, double habitualTendencies)
    {
        double habitContribution = (double)habitCounter / (double)maxHabitCounter * habitualTendencies;
        return habitContribution;
    }

    public static (double predictor, double change) Translate_String_To_Formula_Calculations(string formula, string target, CharacterMainCPort agent,
        CharacterMainCPort env, RelationshipNode envInAgentRPTNode)
    {

        // Replace special terms in the formula
        formula = RegexReplaceLoop(formula, EmpTermRegex, match =>
            ParseEmpTerm(match, agent, env, envInAgentRPTNode).ToString());

        // Step 3: Replace Complex Terms like N-ModR:DB(A)
        formula = RegexReplaceLoop(formula, ComplexTermRegex, match =>
            ParseComplexTerm(match, agent, env, envInAgentRPTNode).ToString());

        // Step 4: Replace Simple Terms like DB(A)
        formula = RegexReplaceLoop(formula, SimpleTermRegex, match =>
        {
            return ParseSimpleTerm(match, agent, env).ToString();
        });

        // Replace scale terms (these scale the change in accordance to the initial values)
        formula = RegexReplaceLoop(formula, ScaleChgRegex, match =>
            ParseScaleChangeTerm(match).ToString());
        // Evaluate custom functions
        formula = EvaluateCustomFunctionsUntilDone(formula);


        double predictorValue = EvaluateBasicExpression(formula);


        //Parse through the target
        while (SimpleTermRegex.IsMatch(target))
        {
            target = SimpleTermRegex.Replace(target,
                match => ParseSimpleTerm(match, agent, env).ToString());
        }

        while (ComplexTermRegex.IsMatch(target))
        {
            target = ComplexTermRegex.Replace(target,
                match => ParseComplexTerm(match, agent, env, envInAgentRPTNode).ToString());
        }

        double targetValue = double.Parse(target);

        // Evaluate the final expression (without functions)
        return (predictor: predictorValue, change: predictorValue - targetValue);
    }

    private static string RegexReplaceLoop(string input, Regex regex, Func<Match, string> replacer)
    {
        string result = input;
        while (regex.IsMatch(result))
        {
            result = regex.Replace(result, new MatchEvaluator(replacer));
        }
        return result;
    }


    public static double CalculateTargetAdjustedEmpathyPRStat(double empathyLevel, double individualPRScale)
    {
        return 1 + (10 - empathyLevel) / 3 + (1 - individualPRScale);
    }

}
