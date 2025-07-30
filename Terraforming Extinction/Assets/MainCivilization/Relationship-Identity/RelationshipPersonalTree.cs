using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

//make it so it can edit. SO should not be edited
public class RelationshipPersonalTree
{
    //public RelationshipPersonalTreeSO RelationshipPersonalTreeSO;
    //DELETE THESE TESTS (FOR REPERCUSSION)
    public float lValueTest;
    public float dbValueTest;
    public float nbValueTest;
    public float learningRateTest;
    public EnumActionCharacteristics actionEnumTest;
    [Header("Do Not Edit")]
    public List<IdentifierNode> RootIdentifiers = new();
    //This is to set the structure of the tree (currently not using very well...)
    public IdentifierMasterTreeSO MasterTree = null;
    public EnumIdentifiers SelfIdentifier = EnumIdentifiers.None;
    public SubIdentifierNode SelfSubIdentifier = null;
    // Start is called before the first frame update
    public RelationshipPersonalTree(RelationshipPersonalTreeSO RelationshipPersonalTreeSO)
    {
        
        RelationshipPersonalTreeSO relationshipPersonalTreeSODeepCopy = RelationshipPersonalTreeSO.DeepCopy();

        RootIdentifiers = relationshipPersonalTreeSODeepCopy.RootIdentifiers;
        Debug.Log("Root Identifier copying into RPT with relationship node: " + RootIdentifiers[0].RelationshipNodes.Count);
        MasterTree = relationshipPersonalTreeSODeepCopy.MasterTree;
        SelfIdentifier = relationshipPersonalTreeSODeepCopy.identifier;
        SelfSubIdentifier = relationshipPersonalTreeSODeepCopy.subIdentifier;
        

    }


    public void AddValuesToSubIdentifier(SubIdentifierNode subIdentifierNode, float existingNodesDistinctiveAbility, float judgmentLevel, float extrapolationLevel, float generalizationAbility, int value = 1)
    {
        //1. Add values to characteristics
        //2. Determine if at zero point or not
        AdaptiveIdentifierFunctions.AddValueToAllCharacteristicsInSubidentity(subIdentifierNode, value);

        //2a. Not at zero point
        //2b. Update Learnng Period values too
        //2c. Check zero point
        if (subIdentifierNode.isZeroPoint == false) {
            AdaptiveIdentifierFunctions.AddValueToAllCharacteristicsInSubidentityDuringLearningPeriod(subIdentifierNode, value);
            RunZeroPoint(subIdentifierNode, existingNodesDistinctiveAbility);
        }
        //2a. At or past zero point
        else
        {
            //Relocate - If becomes too different from anchor
            Relocate(subIdentifierNode, existingNodesDistinctiveAbility);
            //Trickle
            Trickle(subIdentifierNode, judgmentLevel);
            //Transcend
            Transcend(subIdentifierNode, extrapolationLevel);
            //Generalization - Combine versions together (Collective New Anchor)
            Generalization(subIdentifierNode, existingNodesDistinctiveAbility, generalizationAbility);
        }

        //DELETE LATER. Debug the repercussion system
        //RepercussionFunctions.ImplementRepercussion(subIdentifierNode, actionEnumTest, lValueTest, dbValueTest, nbValueTest, learningRateTest);

    }

    private void Relocate(SubIdentifierNode subIdentifierNode, float existingNodesDistinctiveAbility) 
    {
        Debug.Log("Processing Relocate");
        
        //removing
        if(subIdentifierNode.Heuristic != null)
        {
            subIdentifierNode.Heuristic.Specifics.Remove(subIdentifierNode);
        }

        subIdentifierNode.Heuristic = null;

        SubIdentifierNode highestLikenessAnchorNode = null;
        float highestLikenessScore = -1;

        IdentifierNode identifierNode = subIdentifierNode.Parent;
        List<SubIdentifierNode> rootSubIdentifierNodes = identifierNode.SubIdentifiers;

        foreach(SubIdentifierNode rootSubIdentifierNode in rootSubIdentifierNodes)
        {
            RelocateTraverseSubIdentifierNodesRecursive(subIdentifierNode, rootSubIdentifierNode, existingNodesDistinctiveAbility, ref highestLikenessAnchorNode, ref highestLikenessScore);
        }

        if(highestLikenessAnchorNode != null)
        {
            Debug.Log("Run Relocate with new anchor: " +  highestLikenessAnchorNode.SubIdentifierName);
            highestLikenessAnchorNode.Specifics.Add(subIdentifierNode);
            subIdentifierNode.Heuristic = highestLikenessAnchorNode;
        }
        //if no found highest likeness anchor node, then it will still have no heuristic
    }

    private void RelocateTraverseSubIdentifierNodesRecursive(SubIdentifierNode subIdentifierNodeOfInterest, SubIdentifierNode comparingAnchorNode, float existingNodesDistinctiveAbility, ref SubIdentifierNode highestLikenessAnchorNode, ref float highestLikenessScore)
    {
        if(comparingAnchorNode == null) return;

        if(comparingAnchorNode.isAnchor == true)
        {
            float likenessScore = AdaptiveIdentifierFunctions.GetExistingNodesLikenessScore(subIdentifierNodeOfInterest, comparingAnchorNode);
            //>= highestLikenessScore because it should find the lowest node if same
            if (likenessScore > existingNodesDistinctiveAbility && likenessScore >= highestLikenessScore)
            {
                highestLikenessAnchorNode = comparingAnchorNode;
                highestLikenessScore = likenessScore;
            }
        }

        foreach (SubIdentifierNode childSubIdentifierNode in comparingAnchorNode.Specifics)
        {
            RelocateTraverseSubIdentifierNodesRecursive(subIdentifierNodeOfInterest, childSubIdentifierNode, existingNodesDistinctiveAbility, ref highestLikenessAnchorNode, ref highestLikenessScore);
        }
    }

    private void Generalization(SubIdentifierNode subIdentifierNode, float existingNodesDistinctiveAbility, float generalizationAbility)
    {
        Debug.Log("Processing Generalization"); 
        //1. See if any other specific node at same level has likeness score greater than existingNodesDistinctiveAbility
        //2. Make a collective anchor for subIdentifierNode and this specific node if so
        SubIdentifierNode highestLikenessSpecificNode = null;
        float highestLikenessScore = -1;

        float currentAnchorLikenessScore = -1;
        float generalizedMultiplier = 10 / generalizationAbility;


        //Get other specifics
        List<SubIdentifierNode> otherSpecifics = new();
        if (subIdentifierNode.Heuristic != null)
        {
            otherSpecifics = subIdentifierNode.Heuristic.Specifics;
            currentAnchorLikenessScore = AdaptiveIdentifierFunctions.GetExistingNodesLikenessScore(subIdentifierNode, subIdentifierNode.Heuristic);
        }
        else
        {
            otherSpecifics = subIdentifierNode.Parent.SubIdentifiers;
        }

        Debug.Log("Generalization Multiplier: " + generalizedMultiplier + " current anchor likeness score: " + currentAnchorLikenessScore);


        foreach (SubIdentifierNode specificNode in otherSpecifics) 
        { 
            if(specificNode != subIdentifierNode && specificNode.isAnchor == false && specificNode.isZeroPoint == true)
            {
                float likenessScore = AdaptiveIdentifierFunctions.GetExistingNodesLikenessScore(specificNode, subIdentifierNode);
                //need to above the distinctive ability and more than the current anchor score with multiplier
                if (likenessScore > Mathf.Max(existingNodesDistinctiveAbility, highestLikenessScore) && likenessScore > currentAnchorLikenessScore * generalizedMultiplier)
                {
                    highestLikenessScore = likenessScore;
                    highestLikenessSpecificNode=specificNode;
                }
            }
        }

        if (highestLikenessSpecificNode != null)
        {
            Debug.Log("Running Generalization and create new collective anchor for " + subIdentifierNode.SubIdentifierName + " and " + highestLikenessSpecificNode.SubIdentifierName);
            AdaptiveIdentifierFunctions.NewCollectiveAnchor(subIdentifierNode, highestLikenessSpecificNode);
        }

    }

    private void Transcend(SubIdentifierNode specificNode, float extrapolationLevel)
    {
        
        if(specificNode.isZeroPoint == true)
        {
            Debug.Log("Processing Transcend");
            SubIdentifierNode heuristicNode = specificNode.Heuristic;
            float extrapolationPercent = extrapolationLevel / 100;
            if (heuristicNode != null)
            {
                foreach (AppearanceCharacteristicWithValue appearanceCharacteristicWithValue in specificNode.AppearanceCharacteristicsWithValue)
                {
                    float appearanceValue = appearanceCharacteristicWithValue.Value * extrapolationPercent;
                    heuristicNode.AddAppearanceCharacteristic(appearanceCharacteristicWithValue.CharacteristicType, appearanceValue);
                    Debug.Log("Transcending Appearance " + appearanceCharacteristicWithValue.CharacteristicType.ToString() + " by " + appearanceValue);
                }

                foreach (ActionCharacteristicWithValue actionCharacteristicWithValue in specificNode.ActionCharacteristicsWithValue)
                {
                    float actionValue = actionCharacteristicWithValue.Value * extrapolationPercent;
                    heuristicNode.AddActionCharacteristic(actionCharacteristicWithValue.CharacteristicType, actionValue);
                    Debug.Log("Transcending Action " + actionCharacteristicWithValue.CharacteristicType.ToString() + " by " + actionValue);
                }

                //Transcend relationship
                foreach (RelationshipNode relationshipNode in specificNode.RelationshipNodes)
                {
                    RelationshipNode newRelationshipNode = new RelationshipNode(relationshipNode);

                    //loop through pr and modr values and adjust them
                    //Create new array with PR and ModR and loop through them
                    foreach (RelationshipValues values in new[] { newRelationshipNode.PRValues, newRelationshipNode.ModRValues })
                    {
                        values.LivelihoodValue *= extrapolationPercent;
                        values.NurtureBelongingValue *= extrapolationPercent;
                        values.DefensiveBelongingValue = extrapolationPercent;
                    }

                    Debug.Log("Transcending relationship node" + newRelationshipNode + " to " + heuristicNode.SubIdentifierName);
                    heuristicNode.AddRelationshipNode(newRelationshipNode);
                }
            }
            else
            {
                foreach (RelationshipNode relationshipNode in specificNode.RelationshipNodes)
                {
                    RelationshipNode newRelationshipNode = new RelationshipNode(relationshipNode);

                    //loop through pr and modr values and adjust them
                    //Create new array with PR and ModR and loop through them
                    foreach (RelationshipValues values in new[] { newRelationshipNode.PRValues, newRelationshipNode.ModRValues })
                    {
                        values.LivelihoodValue *= extrapolationPercent;
                        values.NurtureBelongingValue *= extrapolationPercent;
                        values.DefensiveBelongingValue = extrapolationPercent;
                    }
                    Debug.Log("Transcending relationship node " + newRelationshipNode + " to " + specificNode.Parent.Identifier);
                    specificNode.Parent.AddRelationshipNode(newRelationshipNode);
                }
            }
        }
        
    }

    private void Trickle(SubIdentifierNode specificNode, float judgmentLevel)
    {
        if(specificNode.isZeroPoint == true)
        {
            Debug.Log("Processing Trickle");
            SubIdentifierNode heuristicNode = specificNode.Heuristic;
            float judgmentPercent = judgmentLevel / 100;
            if (heuristicNode != null)
            {
                //Trickle appearance
                foreach (AppearanceCharacteristicWithValue appearanceCharacteristicWithValue in heuristicNode.AppearanceCharacteristicsWithValue)
                {
                    float appearanceValue = appearanceCharacteristicWithValue.Value * judgmentPercent;
                    specificNode.AddAppearanceCharacteristic(appearanceCharacteristicWithValue.CharacteristicType, appearanceValue);
                    Debug.Log("Trickling Appearance " + appearanceCharacteristicWithValue.CharacteristicType.ToString() + " by " + appearanceValue);
                }


                //Trickle action
                foreach (ActionCharacteristicWithValue actionCharacteristicWithValue in heuristicNode.ActionCharacteristicsWithValue)
                {
                    float actionValue = actionCharacteristicWithValue.Value * judgmentPercent;
                    specificNode.AddActionCharacteristic(actionCharacteristicWithValue.CharacteristicType, actionValue);
                    Debug.Log("Trickling Action " + actionCharacteristicWithValue.CharacteristicType.ToString() + " by " + actionValue);
                }

                //Trickle relationship
                foreach (RelationshipNode relationshipNode in heuristicNode.RelationshipNodes)
                {
                    RelationshipNode newRelationshipNode = new RelationshipNode(relationshipNode);

                    //loop through pr and modr values and adjust them
                    //Create new array with PR and ModR and loop through them
                    foreach (RelationshipValues values in new[] { newRelationshipNode.PRValues, newRelationshipNode.ModRValues })
                    {
                        values.LivelihoodValue *= judgmentPercent;
                        values.NurtureBelongingValue *= judgmentPercent;
                        values.DefensiveBelongingValue = judgmentPercent;
                    }
                    Debug.Log("Trickling relationship node" + newRelationshipNode + " to " + heuristicNode.SubIdentifierName);
                    specificNode.AddRelationshipNode(newRelationshipNode);
                }
            }
            //if not heuristic subidentifiers, then use identifier to trickle
            //can only trickle relationship
            else
            {
                foreach (RelationshipNode relationshipNode in specificNode.Parent.RelationshipNodes)
                {
                    RelationshipNode newRelationshipNode = new RelationshipNode(relationshipNode);

                    //loop through pr and modr values and adjust them
                    //Create new array with PR and ModR and loop through them
                    foreach (RelationshipValues values in new[] { newRelationshipNode.PRValues, newRelationshipNode.ModRValues })
                    {
                        values.LivelihoodValue *= judgmentPercent;
                        values.NurtureBelongingValue *= judgmentPercent;
                        values.DefensiveBelongingValue = judgmentPercent;
                    }
                    Debug.Log("Trickling relationship node " + newRelationshipNode + " to " + specificNode.Parent.Identifier);
                    specificNode.AddRelationshipNode(newRelationshipNode);
                }
            }
        }
    }

    private void RunZeroPoint(SubIdentifierNode subIdentifierNode, float anchorDistinctiveAbility)
    {
        //check if hits zero point
        if (AdaptiveIdentifierFunctions.CheckZeroPoint(subIdentifierNode))
        {
            subIdentifierNode.isZeroPoint = true;
            
            SubIdentifierNode heuristicSubIdentifier = subIdentifierNode.Heuristic;

            //if there is no heuristic then don't need to care
            if (heuristicSubIdentifier != null)
            {
                float anchorLikenessScore = AdaptiveIdentifierFunctions.GetExistingNodesLikenessScore(subIdentifierNode, heuristicSubIdentifier);


                //When spread is different
                //1. the anchorlikeness score is below the cutoff of anchor distinctive ability or if it is not an anchor it can stay at
                //2. Do distanced post-zero point

                if(heuristicSubIdentifier.isAnchor == false)
                {
                    if(anchorLikenessScore >= anchorDistinctiveAbility)
                    {
                        //Create a New Collective Anchor based on both stats if heuristic is not an anchor
                        Debug.Log("Processing new close zero point because anchor likeness score " + anchorLikenessScore + " is greater than distinctive ability " + anchorDistinctiveAbility);
                        AdaptiveIdentifierFunctions.NewClosePostZero(subIdentifierNode, heuristicSubIdentifier);
                    }
                    else
                    {
                        Debug.Log("Processing distanced post zero because the heuristic is not an anchor and anchor likeness score " + anchorLikenessScore + " is less than distinctive ability " + anchorDistinctiveAbility);
                        AdaptiveIdentifierFunctions.DistancedPostZero(subIdentifierNode, anchorDistinctiveAbility);
                    }
                }else if(heuristicSubIdentifier.isAnchor == true)
                {
                    if(anchorLikenessScore < anchorDistinctiveAbility)
                    {
                        Debug.Log("Processing distanced post zero because anchor likeness score " + anchorLikenessScore + " is less than distinctive ability " + anchorDistinctiveAbility);
                        AdaptiveIdentifierFunctions.DistancedPostZero(subIdentifierNode, anchorDistinctiveAbility);
                    }
                    //else do nothing and should stay there. 
                }
            }
            
            
        }
    }

    
}
