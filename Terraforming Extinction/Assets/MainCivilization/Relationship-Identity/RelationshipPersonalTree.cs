using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

//make it so it can edit. SO should not be edited
public class RelationshipPersonalTree : MonoBehaviour
{
    public RelationshipPersonalTreeSO RelationshipPersonalTreeSO;

    [Header("Do Not Edit")]
    public List<IdentifierNode> RootIdentifiers;
    public IdentifierMasterTreeSO MasterTree;
    public List<EnumIdentifiers> identifiers;
    public List<SubIdentifierNode> subIdentifiers;
    // Start is called before the first frame update
    void Start()
    {
        if(RootIdentifiers == null && MasterTree == null && identifiers == null && subIdentifiers == null)
        {
            RootIdentifiers = RelationshipPersonalTreeSO.RootIdentifiers;
            MasterTree = RelationshipPersonalTreeSO.MasterTree;
            identifiers = RelationshipPersonalTreeSO.identifiers;
            subIdentifiers = RelationshipPersonalTreeSO.subIdentifiers;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddValuesToSubIdentifier(SubIdentifierNode subIdentifierNode, float existingNodesDistinctiveAbility, float judgmentLevel, float extrapolationLevel, int value = 1)
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
            //Trickle
            Trickle(subIdentifierNode, judgmentLevel);
            //Transcend
            Transcend(subIdentifierNode, extrapolationLevel);
            //Generalization - Combine versions together (Collective New Anchor)
            Generalization(subIdentifierNode, existingNodesDistinctiveAbility);
        }
    }

    private void Relocate(SubIdentifierNode subIdentifierNode, float existingNodesDistinctiveAbility) 
    {
        //removing
        if(subIdentifierNode.Heuristic != null)
        {
            subIdentifierNode.Heuristic.Specifics.Remove(subIdentifierNode);
        }

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
            highestLikenessAnchorNode.Specifics.Add(subIdentifierNode);
        }
    }

    private void RelocateTraverseSubIdentifierNodesRecursive(SubIdentifierNode subIdentifierNodeOfInterest, SubIdentifierNode comparingAnchorNode, float existingNodesDistinctiveAbility, ref SubIdentifierNode highestLikenessAnchorNode, ref float highestLikenessScore)
    {
        if(comparingAnchorNode == null) return;

        if(comparingAnchorNode.isAnchor == true)
        {
            float likenessScore = AdaptiveIdentifierFunctions.GetExistingNodesLikenessScore(subIdentifierNodeOfInterest, comparingAnchorNode);
            if (likenessScore > Mathf.Max(highestLikenessScore, existingNodesDistinctiveAbility))
            {
                highestLikenessAnchorNode = comparingAnchorNode;
                highestLikenessScore = likenessScore;
            }
        }

        foreach (SubIdentifierNode childSubIdentifierNode in subIdentifierNodeOfInterest.Specifics)
        {
            RelocateTraverseSubIdentifierNodesRecursive(subIdentifierNodeOfInterest, childSubIdentifierNode, existingNodesDistinctiveAbility, ref highestLikenessAnchorNode, ref highestLikenessScore);
        }
    }

    private void Generalization(SubIdentifierNode subIdentifierNode, float existingNodesDistinctiveAbility)
    {
        //1. See if any specific node has likeness score greater than existingNodesDistinctiveAbility
        //2. Make a collective anchor for subIdentifierNode and this specific node if so
        SubIdentifierNode highestLikenessSpecificNode = null;
        float highestLikenessScore = -1;
        //Need to see if there are other heuristic to compare to
        foreach (SubIdentifierNode specificNode in subIdentifierNode.Heuristic.Specifics) 
        { 
            if(specificNode != subIdentifierNode)
            {
                float likenessScore = AdaptiveIdentifierFunctions.GetExistingNodesLikenessScore(specificNode, subIdentifierNode);
                if (likenessScore > Mathf.Max(existingNodesDistinctiveAbility, highestLikenessScore))
                {
                    highestLikenessScore = likenessScore;
                    highestLikenessSpecificNode=specificNode;
                }
            }

            if (highestLikenessSpecificNode != null) 
            { 
                AdaptiveIdentifierFunctions.NewCollectiveAnchor(subIdentifierNode, highestLikenessSpecificNode);
            }
        }

    }

    private void Transcend(SubIdentifierNode specificNode, float extrapolationLevel)
    {
        SubIdentifierNode heuristicNode = specificNode.Heuristic;
        float extrapolationPercent = extrapolationLevel / 100;
        if (heuristicNode != null)
        {
            foreach (AppearanceCharacteristicWithValue appearanceCharacteristicWithValue in specificNode.AppearanceCharacteristicsWithValue)
            {
                float appearanceValue = appearanceCharacteristicWithValue.Value * extrapolationPercent;
                heuristicNode.AddAppearanceCharacteristic(appearanceCharacteristicWithValue.CharacteristicType, appearanceValue);
            }

            foreach (ActionCharacteristicWithValue actionCharacteristicWithValue in specificNode.ActionCharacteristicsWithValue)
            {
                float actionValue = actionCharacteristicWithValue.Value * extrapolationPercent;
                heuristicNode.AddActionCharacteristic(actionCharacteristicWithValue.CharacteristicType, actionValue);
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
                specificNode.Parent.AddRelationshipNode(newRelationshipNode);
            }
        }
    }

    private void Trickle(SubIdentifierNode specificNode, float judgmentLevel)
    {
        SubIdentifierNode heuristicNode = specificNode.Heuristic;
        float judgmentPercent = judgmentLevel/100;
        if(heuristicNode != null)
        {
            //Trickle appearance
            foreach (AppearanceCharacteristicWithValue appearanceCharacteristicWithValue in heuristicNode.AppearanceCharacteristicsWithValue) 
            { 
                float appearanceValue = appearanceCharacteristicWithValue.Value * judgmentPercent;
                specificNode.AddAppearanceCharacteristic(appearanceCharacteristicWithValue.CharacteristicType, appearanceValue);
            }


            //Trickle action
            foreach(ActionCharacteristicWithValue actionCharacteristicWithValue in heuristicNode.ActionCharacteristicsWithValue)
            {
                float actionValue = actionCharacteristicWithValue.Value * judgmentPercent;
                specificNode.AddActionCharacteristic(actionCharacteristicWithValue.CharacteristicType, actionValue);
            }

            //Trickle relationship
            foreach (RelationshipNode relationshipNode in heuristicNode.RelationshipNodes) 
            { 
                RelationshipNode newRelationshipNode = new RelationshipNode(relationshipNode);

                //loop through pr and modr values and adjust them
                //Create new array with PR and ModR and loop through them
                foreach(RelationshipValues values in new[] {newRelationshipNode.PRValues, newRelationshipNode.ModRValues })
                {
                    values.LivelihoodValue *= judgmentPercent;
                    values.NurtureBelongingValue *= judgmentPercent;
                    values.DefensiveBelongingValue = judgmentPercent;
                }
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
                specificNode.AddRelationshipNode(newRelationshipNode);
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
                if (anchorLikenessScore < anchorDistinctiveAbility)
                {
                    AdaptiveIdentifierFunctions.DistancedPostZero(subIdentifierNode, anchorDistinctiveAbility);
                }
                else
                {
                    //When spread is similar 
                    //Heuristic is not anchor, then do New Close Post-Zero
                    if(heuristicSubIdentifier.isAnchor == false)
                    {
                        //Create a New Collective Anchor based on both stats
                        AdaptiveIdentifierFunctions.NewClosePostZero(subIdentifierNode, heuristicSubIdentifier);
                    }
                    //Else, then do Maintained Close Post-Zero (Aka, do nothing and keep there)
                }
            }
            
            
        }
    }

    
}
