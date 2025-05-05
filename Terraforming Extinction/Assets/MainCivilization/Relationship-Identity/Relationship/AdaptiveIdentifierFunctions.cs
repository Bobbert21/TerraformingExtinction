using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;

//The functions that manipulates identifiers
//Uses IdentityUtils
public static class AdaptiveIdentifierFunctions
{
    private const int ZeroPointCutoff = 5;

    public static bool CheckZeroPoint(SubIdentifierNode subIdentifierNode)
    {
        float totalCount = 0;
        if (subIdentifierNode.isZeroPoint == false)
        {
            foreach (AppearanceCharacteristicWithValue appearanceCharacteristicWithValue in subIdentifierNode.AppearanceCharacteristicsWithValue)
            {
                totalCount += appearanceCharacteristicWithValue.Value;
            }

            foreach (ActionCharacteristicWithValue actionCharacteristicWithValue in subIdentifierNode.ActionCharacteristicsWithValue)
            {
                totalCount += actionCharacteristicWithValue.Value;
            }

            if (totalCount >= ZeroPointCutoff)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            throw new System.Exception("SubIdentifierNode is already a Zero Point! This is not allowed here.");
        }
    }

    public static void AddValueToSubidentity(SubIdentifierNode subIdentifier, ICharacteristics characteristic, int value = 1)
    {
        /*
        IdentityUtils.AddValueAndTranscend(IdentifierTrackerTree.RootIdentifiers, subIdentifier, characteristic, value, ZeroPointCutoff);
        */
    }

    public static void AddValueToAllCharacteristicsInSubidentityDuringLearningPeriod(SubIdentifierNode subIdentifierNode, int value = 1)
    {
        foreach (AppearanceCharacteristicWithValue appearanceCharacteristicWithValue in subIdentifierNode.LearningPeriodAppearanceCharacteristicsWithValue)
        {
            appearanceCharacteristicWithValue.Value += 1;
        }

        foreach (ActionCharacteristicWithValue actionCharacteristicWithValue in subIdentifierNode.LearningPeriodActionCharacteristicsWithValue)
        {
            actionCharacteristicWithValue.Value += 1;
        }
    }

    //
    public static void AddValueToAllCharacteristicsInSubidentity(SubIdentifierNode subIdentifierNode, int value = 1)
    {
        foreach (AppearanceCharacteristicWithValue appearanceCharacteristicWithValue in subIdentifierNode.AppearanceCharacteristicsWithValue)
        {
            appearanceCharacteristicWithValue.Value += 1;
        }

        foreach (ActionCharacteristicWithValue actionCharacteristicWithValue in subIdentifierNode.ActionCharacteristicsWithValue)
        {
            actionCharacteristicWithValue.Value += 1;
        }
    }

    // Deep copy creation, can still be moved to utility
    private static RelationshipPersonalTreeSO CreateCopy(RelationshipPersonalTreeSO original)
    {
        RelationshipPersonalTreeSO copy = ScriptableObject.CreateInstance<RelationshipPersonalTreeSO>();
        copy.MasterTree = original.MasterTree;
        copy.RootIdentifiers = new List<IdentifierNode>();

        foreach (var rootIdentifier in original.RootIdentifiers)
        {
            copy.RootIdentifiers.Add(AdaptiveIdentityUtils.CopyIdentifierNode(rootIdentifier));
        }

        return copy;
    }

    //find identifiernode with the identifier enum
    public static IdentifierNode FindIdentifierNode(RelationshipPersonalTree relationshipPersonalTree, EnumIdentifiers identifierEnum)
    {
        List<IdentifierNode> rootIdentifiers = relationshipPersonalTree.RootIdentifiers;

        foreach (IdentifierNode rootIdentifier in rootIdentifiers)
        {
            IdentifierNode foundIdentifierNode = FindIdentifierNodeRecursive(rootIdentifier, identifierEnum);
            if(foundIdentifierNode != null)
            {
                return foundIdentifierNode;
            }
        }
        return null;   
    }

    //find subidentifier node
    public static SubIdentifierNode FindSubidentifierNodeWithAppearanceAndAction(RelationshipPersonalTree relationshipPersonalTree, EnumIdentifiers identifierEnum, List<EnumAppearanceCharacteristics> appearanceEnums, List<EnumActionCharacteristics> actionEnums) {
        IdentifierNode foundIdentifierNode = FindIdentifierNode(relationshipPersonalTree, identifierEnum);

        if (foundIdentifierNode != null) 
        {
            foreach (SubIdentifierNode subIdentifierNode in foundIdentifierNode.SubIdentifiers) 
            {
                SubIdentifierNode foundSub = FindSubidentifierNodeRecursive(subIdentifierNode, appearanceEnums, actionEnums);
                if(foundSub != null)
                {
                    return foundSub;
                }
            }
        }

        return null; 
    }

    //Highest likeliness score with 
    public static (SubIdentifierNode, float) FindSubidentifierNodeWithAppearanceAndActionWithLikenessScore(RelationshipPersonalTree relationshipPersonalTree, EnumIdentifiers identifierEnum, 
        List<EnumAppearanceCharacteristics> appearanceEnums, List<EnumActionCharacteristics> actionEnums, string name, float distinctiveAbility = -1)
    {
        IdentifierNode foundIdentifierNode = FindIdentifierNode(relationshipPersonalTree, identifierEnum);

        if (foundIdentifierNode != null)
        {
            foreach (SubIdentifierNode subIdentifierNode in foundIdentifierNode.SubIdentifiers)
            {
                SubIdentifierNode foundSub = null;
                float likenessScore = -1;
                (foundSub, likenessScore)= FindSubidentifierNodeWithLikenessRecursive(subIdentifierNode, appearanceEnums, actionEnums, distinctiveAbility, -1, null, name);
                if (foundSub != null)
                {
                    return (foundSub, likenessScore);
                }
            }
        }

        return (null, -1);
    }

    private static (SubIdentifierNode, float) FindSubidentifierNodeWithLikenessRecursive(SubIdentifierNode subIdentifierNode, List<EnumAppearanceCharacteristics> appearanceEnumsInNewVersion, 
        List<EnumActionCharacteristics> actionEnumsInNewVersion, float distinctiveAbility, float highestLikenessScore, SubIdentifierNode highestLikenessNode, string name)
    {
        //get all the subidentifiers appearance
        HashSet<EnumAppearanceCharacteristics> subIdentifierAppearanceCharacteristics = new();
        foreach (AppearanceCharacteristicWithValue appearanceWithValue in subIdentifierNode.AppearanceCharacteristicsWithValue)
        {
            subIdentifierAppearanceCharacteristics.Add(appearanceWithValue.CharacteristicType);
        }

        //add action
        HashSet<EnumActionCharacteristics> subIdentifierActionCharacteristics = new();
        foreach (ActionCharacteristicWithValue actionWithValue in subIdentifierNode.ActionCharacteristicsWithValue)
        {
            subIdentifierActionCharacteristics.Add(actionWithValue.CharacteristicType);
        }

        float likenessScore = 0;
        int unmatchedNewVersionToExistingCharacteristicsCount = 0;
        int matchedNewVersionToExistingCharacteristicsCount = 0;

        foreach(EnumAppearanceCharacteristics appearanceCharacteristic in appearanceEnumsInNewVersion)
        {
            if (subIdentifierAppearanceCharacteristics.Contains(appearanceCharacteristic))
            {
                AppearanceCharacteristicWithValue matchedAppearanceCharacteristicWithValue = subIdentifierNode.AppearanceCharacteristicsWithValue
                    .FirstOrDefault(a => a.CharacteristicType == appearanceCharacteristic);

                if (matchedAppearanceCharacteristicWithValue != null) 
                {
                    matchedNewVersionToExistingCharacteristicsCount++;
                    likenessScore += (float)System.Math.Pow(matchedAppearanceCharacteristicWithValue.Value, 1.5);
                }
            }
            else
            {
                unmatchedNewVersionToExistingCharacteristicsCount++;
            }
        }


        foreach (EnumActionCharacteristics actionCharacteristic in actionEnumsInNewVersion)
        {
            if (subIdentifierActionCharacteristics.Contains(actionCharacteristic))
            {
                ActionCharacteristicWithValue matchedActionCharacteristicWithValue = subIdentifierNode.ActionCharacteristicsWithValue
                    .FirstOrDefault(a => a.CharacteristicType == actionCharacteristic);

                if (matchedActionCharacteristicWithValue != null)
                {
                    matchedNewVersionToExistingCharacteristicsCount++;
                    likenessScore += (float)System.Math.Pow(matchedActionCharacteristicWithValue.Value, 1.5);
                }
            }
            else
            {
                unmatchedNewVersionToExistingCharacteristicsCount++;
            }
        }
        //calculated characteristics in this existing node that is not in subidentifier
        int unmatchedExistingToNewVersionCharacteristicsCount = (appearanceEnumsInNewVersion.Count + actionEnumsInNewVersion.Count - matchedNewVersionToExistingCharacteristicsCount);

        //subtract from mismatches from both ends
        likenessScore -= (float)System.Math.Pow(unmatchedExistingToNewVersionCharacteristicsCount, 1.5);
        likenessScore -= (float)System.Math.Pow(unmatchedNewVersionToExistingCharacteristicsCount, 1.5);

        //If everything matches
        if (unmatchedExistingToNewVersionCharacteristicsCount == 0 && unmatchedNewVersionToExistingCharacteristicsCount == 0 && subIdentifierNode.SubIdentifierName == name)
        {
            return (subIdentifierNode, likenessScore);
        }

        if (likenessScore >= highestLikenessScore && likenessScore >= distinctiveAbility)
        {

            //only replaces the current one if the current one is not possibly the Version/Specific. 
            if(likenessScore == highestLikenessScore)
            {
                if (highestLikenessNode.SubIdentifierName != name || !highestLikenessNode.isAnchor)
                {
                    highestLikenessScore = likenessScore;
                    highestLikenessNode = subIdentifierNode;
                }
            }
            
           
        }


        //continue to find through 
        foreach (SubIdentifierNode specificSubIdentifierNode in subIdentifierNode.Specifics)
        {
            SubIdentifierNode foundSubIdentiferNode = null;
            (foundSubIdentiferNode, highestLikenessScore) = FindSubidentifierNodeWithLikenessRecursive(specificSubIdentifierNode, appearanceEnumsInNewVersion, actionEnumsInNewVersion, distinctiveAbility, highestLikenessScore, highestLikenessNode, name);
            if (foundSubIdentiferNode != null) { return (foundSubIdentiferNode, highestLikenessScore); }
        }

        //if no perfect match, then return the closest if over the distinctive ability
        if (highestLikenessScore >= distinctiveAbility)
        {
            return (highestLikenessNode, highestLikenessScore);
        }

        //else returns none
        return (null, -1);
    }

    private static SubIdentifierNode FindSubidentifierNodeRecursive(SubIdentifierNode subIdentifierNode, List<EnumAppearanceCharacteristics> appearanceEnums, List<EnumActionCharacteristics> actionEnums)
    {
        //get all the subidentifiers appearance
        HashSet<EnumAppearanceCharacteristics> subIdentifierAppearanceCharacteristics = new();
        foreach (AppearanceCharacteristicWithValue appearanceWithValue in subIdentifierNode.AppearanceCharacteristicsWithValue)
        {
            subIdentifierAppearanceCharacteristics.Add(appearanceWithValue.CharacteristicType);
        }

        //add action
        HashSet<EnumActionCharacteristics> subIdentifierActionCharacteristics = new();
        foreach(ActionCharacteristicWithValue actionWithValue in subIdentifierNode.ActionCharacteristicsWithValue)
        {
            subIdentifierActionCharacteristics.Add(actionWithValue.CharacteristicType);
        }

        //check for one on one match
        if (new HashSet<EnumAppearanceCharacteristics>(appearanceEnums)
            .SetEquals(subIdentifierAppearanceCharacteristics) &&
        new HashSet<EnumActionCharacteristics>(actionEnums)
            .SetEquals(subIdentifierActionCharacteristics))
        {
            return subIdentifierNode;
        }


        foreach (SubIdentifierNode specificSubIdentifierNode in subIdentifierNode.Specifics)
        {
            SubIdentifierNode foundSubIdentiferNode = FindSubidentifierNodeRecursive(specificSubIdentifierNode, appearanceEnums, actionEnums);
            if (foundSubIdentiferNode != null) { return foundSubIdentiferNode; }
        }

        return null;
    }

    //helper function to find the right identifier node with enum identifier
    private static IdentifierNode FindIdentifierNodeRecursive(IdentifierNode identifierNode, EnumIdentifiers identifierEnum) { 
        if(identifierNode.Identifier == identifierEnum)
        {
            return identifierNode;
        }

        //search for children if not right node
        foreach(IdentifierNode childIdentifier in identifierNode.Children)
        {
            IdentifierNode foundIdentifierNode = FindIdentifierNodeRecursive(childIdentifier, identifierEnum);
            if (foundIdentifierNode != null)
            {
                return foundIdentifierNode;
            }
        }

        return null;

    }

    //max likeness score
    public static float GetMaxLikenessScorePossible(SubIdentifierNode subIdentifierNode)
    {
        List<ActionCharacteristicWithValue> actionCharacteristicsWithValue = subIdentifierNode.ActionCharacteristicsWithValue;
        List<AppearanceCharacteristicWithValue> appearanceCharacteristicsWithValue = subIdentifierNode.AppearanceCharacteristicsWithValue;

        float maxLikenessScore = 0;

        foreach (ActionCharacteristicWithValue actionCharacteristicWithValue in actionCharacteristicsWithValue) 
        { 
            maxLikenessScore += (float)System.Math.Pow(actionCharacteristicWithValue.Value, 2);
        }

        foreach (AppearanceCharacteristicWithValue appearanceCharacteristicWithValue in appearanceCharacteristicsWithValue)
        {
            maxLikenessScore += (float)System.Math.Pow(appearanceCharacteristicWithValue.Value, 2);
        }

        return maxLikenessScore;

    }


    //get likeness score
    public static float GetExistingNodesLikenessScore(SubIdentifierNode subIdentifierNode1, SubIdentifierNode subIdentifierNode2)
    {
        List<ActionCharacteristicWithValue> comparingActionCharacteristicsWithValue = subIdentifierNode1.ActionCharacteristicsWithValue;
        List<AppearanceCharacteristicWithValue> comparingAppearanceCharacteristicsWithValues = subIdentifierNode1.AppearanceCharacteristicsWithValue;
        List<ActionCharacteristicWithValue> targetActionCharacteristicsWithValue = subIdentifierNode2.ActionCharacteristicsWithValue;
        List<AppearanceCharacteristicWithValue> targetAppearanceCharacteristicsWithValue = subIdentifierNode2.AppearanceCharacteristicsWithValue;

        // 1. Comparing Action Characteristics with the sum of its value (although I don't think i need to add it all with the Sum function)
        Dictionary<EnumActionCharacteristics, float> comparingActionRatios = comparingActionCharacteristicsWithValue
            .GroupBy(ac => ac.CharacteristicType)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(ac => (float)ac.Value)
            );

        float totalComparingActions = comparingActionRatios.Values.Sum();

        //divide by all the total sum value to get the ratios
        comparingActionRatios = comparingActionRatios
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value / totalComparingActions);

        // 2. Comparing Appearance Characteristics
        Dictionary<EnumAppearanceCharacteristics, float> comparingAppearanceRatios = comparingAppearanceCharacteristicsWithValues
            .GroupBy(ac => ac.CharacteristicType)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(ac => (float)ac.Value)
            );

        float totalComparingAppearances = comparingAppearanceRatios.Values.Sum();

        comparingAppearanceRatios = comparingAppearanceRatios
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value / totalComparingAppearances);


        // 3. Target Action Characteristics
        Dictionary<EnumActionCharacteristics, float> targetActionRatios = targetActionCharacteristicsWithValue
            .GroupBy(ac => ac.CharacteristicType)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(ac => (float)ac.Value)
            );

        float totalTargetActions = targetActionRatios.Values.Sum();

        targetActionRatios = targetActionRatios
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value / totalTargetActions);


        // 4. Target Appearance Characteristics
        Dictionary<EnumAppearanceCharacteristics, float> targetAppearanceRatios = targetAppearanceCharacteristicsWithValue
            .GroupBy(ac => ac.CharacteristicType)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(ac => (float)ac.Value)
            );

        float totalTargetAppearances = targetAppearanceRatios.Values.Sum();

        targetAppearanceRatios = targetAppearanceRatios
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value / totalTargetAppearances);

        float preAnchorLikenessScore = 0;

        //get all the matching and add the unmatched comparing
        foreach(var ac in comparingActionRatios)
        {
            if(targetActionRatios.TryGetValue(ac.Key, out float targetValue))
            {
                //when matched
                float diff = ac.Value - targetValue;
                preAnchorLikenessScore += diff * diff;
            }
            else
            {
                //when unmatched
                preAnchorLikenessScore += ac.Value * ac.Value;
            }
        }

        //add the unmatched in target
        foreach(var ac in targetActionRatios)
        {
            if (!comparingActionRatios.ContainsKey(ac.Key))
            {
                preAnchorLikenessScore += ac.Value * ac.Value;
            }
        }

        //get all the matching and add the unmatched comparing
        foreach (var ac in comparingAppearanceRatios)
        {
            if (targetAppearanceRatios.TryGetValue(ac.Key, out float targetValue))
            {
                //matched
                float diff = ac.Value - targetValue;
                preAnchorLikenessScore += diff * diff;
            }
            else
            {
                //unmatched
                preAnchorLikenessScore += ac.Value * ac.Value;
            }
        }

        //add the unmatched in target
        foreach (var ac in targetAppearanceRatios)
        {
            if (!comparingAppearanceRatios.ContainsKey(ac.Key))
            {
                preAnchorLikenessScore += ac.Value * ac.Value;
            }
        }

        float anchorLikenessScore = 2 - preAnchorLikenessScore;

        return anchorLikenessScore;
    }

    //Will build new collective anchor off of the anchor node of subIdentifierNode2 not 1
    public static void NewCollectiveAnchor(SubIdentifierNode subIdentifierNode1, SubIdentifierNode subIdentifierNode2)
    {
        SubIdentifierNode currentAnchor = subIdentifierNode2.Heuristic;

        //keep looping until you find an anchor
        while (currentAnchor != null && currentAnchor.isAnchor == false) 
        {
            currentAnchor = currentAnchor.Heuristic;
        }

        SubIdentifierNode newCollectiveAnchor = new(currentAnchor.Parent);

        newCollectiveAnchor.Heuristic = currentAnchor;

        newCollectiveAnchor.Specifics.Add(subIdentifierNode1);
        newCollectiveAnchor.Specifics.Add(subIdentifierNode2);
        newCollectiveAnchor.isAnchor = true;
        newCollectiveAnchor.isZeroPoint = true;
        newCollectiveAnchor.SubIdentifierName = subIdentifierNode2.SubIdentifierName + " " + subIdentifierNode1.SubIdentifierName;


        foreach (ActionCharacteristicWithValue actionCharacteristicWithValue in subIdentifierNode1.ActionCharacteristicsWithValue)
        {
            newCollectiveAnchor.AddActionCharacteristic(actionCharacteristicWithValue.CharacteristicType, actionCharacteristicWithValue.Value);
        }

        foreach (ActionCharacteristicWithValue actionCharacteristicWithValue in subIdentifierNode2.ActionCharacteristicsWithValue) 
        {
            newCollectiveAnchor.AddActionCharacteristic(actionCharacteristicWithValue.CharacteristicType, actionCharacteristicWithValue.Value);
        }

        foreach(AppearanceCharacteristicWithValue appearanceCharacteristicWithValue in subIdentifierNode1.AppearanceCharacteristicsWithValue)
        {
            newCollectiveAnchor.AddAppearanceCharacteristic(appearanceCharacteristicWithValue.CharacteristicType, appearanceCharacteristicWithValue.Value);
        }

        foreach (AppearanceCharacteristicWithValue appearanceCharacteristicWithValue in subIdentifierNode2.AppearanceCharacteristicsWithValue)
        {
            newCollectiveAnchor.AddAppearanceCharacteristic(appearanceCharacteristicWithValue.CharacteristicType, appearanceCharacteristicWithValue.Value);
        }

        //Mixing relationship nodes
        //Deep copy
        newCollectiveAnchor.RelationshipNodes = subIdentifierNode1.RelationshipNodes
    .Select(rn => new RelationshipNode(rn))
    .ToList();


        foreach (RelationshipNode relationshipNode in subIdentifierNode1.RelationshipNodes)
        {
            //Mixing means that it will add values together if they both exist in there
            newCollectiveAnchor.AddRelationshipNode(relationshipNode);
        }
    }

    public static void DistancedPostZero(SubIdentifierNode subIdentifierNode, float anchorDistinctiveAbility)
    {
        SubIdentifierNode heuristicSubIdentifier = subIdentifierNode.Heuristic;
        //reset to null in case if it doesn't find an anchor
        subIdentifierNode.Heuristic = null;
        float anchorLikenessScore = 0;

        //loop through all the parents to see which anchor to fit under based on likeness score
        while (heuristicSubIdentifier != null)
        {
            heuristicSubIdentifier = heuristicSubIdentifier.Heuristic;
            if (heuristicSubIdentifier.isAnchor)
            {
                //will set new heuristic as this if works
                anchorLikenessScore = GetExistingNodesLikenessScore(subIdentifierNode, heuristicSubIdentifier);
                if (anchorLikenessScore >= anchorDistinctiveAbility)
                {
                    subIdentifierNode.Heuristic = heuristicSubIdentifier;
                    break;
                }
            }
        }
    }

    public static void NewClosePostZero(SubIdentifierNode subIdentifierNode, SubIdentifierNode heuristicSubIdentfierNode)
    {
        //Create a new collective anchor
        NewCollectiveAnchor(subIdentifierNode, heuristicSubIdentfierNode);
    }
}



/*
    // Add value to sub-identities with specific action
    public void AddValueToSubIdentities(Identifiers identifier, Characteristics action, int value = 1)
    {
        //find the identifier node with the right enum (so can get the subidentities)
        IdentifierNode node = IdentityUtils.FindIdentifierNodeByEnum(IdentifierTrackerTree.RootIdentifiers, identifier);
        if (node != null)
        {
            AddValueToSubIdentifiersWithNode(node, action, value);
        }
    }

    // Adding value and zero-point logic to sub-identities
    // Possibly move to Identity Utils
    private void AddValueToSubIdentifiersWithNode(IdentifierNode node, Characteristics action, int value)
    {
        int characteristicsOccurrences = 0;

        foreach (SubIdentifier subIdentifier in node.SubIdentifiers)
        {
            foreach (CharacteristicWithValue characteristicWithValue in subIdentifier.Characteristics)
            {
                if (characteristicWithValue.Characteristic == action)
                {
                    characteristicWithValue.Value += value;
                }

                // Handle zero point logic
                if (!subIdentifier.isZeroPoint)
                {
                    characteristicsOccurrences += characteristicWithValue.Value;
                    if (characteristicsOccurrences >= ZeroPointCutoff)
                    {
                        subIdentifier.isZeroPoint = true;
                    }
                }
            }
        }
    }
    */