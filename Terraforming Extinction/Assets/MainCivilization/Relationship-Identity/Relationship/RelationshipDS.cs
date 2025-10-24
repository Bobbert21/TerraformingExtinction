  using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum EnumGoals
{
    None,
    Rob_Home,
    Count
}

[System.Serializable]
public enum SystemTheory
{
    Consume,
    Resist,
    Feed,
    Protect,
    Eliminate,
    Gain
}

[System.Serializable]
public class RelationshipValues
{
    public float DefensiveBelongingValue;
    public float NurtureBelongingValue;
    public float LivelihoodValue;
    public RelationshipValues() { }

    public RelationshipValues(RelationshipValues other)
    {
        DefensiveBelongingValue = other.DefensiveBelongingValue;
        NurtureBelongingValue = other.NurtureBelongingValue;
        LivelihoodValue = other.LivelihoodValue;
    }
    public RelationshipValues(float livelihoodValue, float defensiveBelongingValue, float nurtureBelongingValue)
    {
        DefensiveBelongingValue = defensiveBelongingValue;
        NurtureBelongingValue = nurtureBelongingValue;
        LivelihoodValue = livelihoodValue;
    }

    public void AddValues(RelationshipValues relationshipValues)
    {
        DefensiveBelongingValue += relationshipValues.DefensiveBelongingValue;
        NurtureBelongingValue += relationshipValues.NurtureBelongingValue;
        LivelihoodValue += relationshipValues.LivelihoodValue;
    }

    public override string ToString()
    {
        return "L: " + LivelihoodValue.ToString() + " NB: " + NurtureBelongingValue.ToString() + " DB: " + DefensiveBelongingValue.ToString();
    }
}



[System.Serializable]
public class NeScenarioNode
{
    public string Name;
    public SubIdentifierNode ParentSubIdentifierNode;
    public IdentifierNode ParentIdentifierNode;
    public RelationshipValues PRValues;
    public RelationshipValues ModRValues;
    public EnumActionCharacteristics ActionContext;
    public List<NiDecisionNode> ResponseNodes;
    public List<NiDecisionNode> ActionPlanNodes;
    public List<(NeScenarioNode neNode, int habitCounter)> ProceedingScenarios = new();
    public List<(NeScenarioNode neNode, int habitCounter)> PrecedingScenarios = new();

    public int HabitCounter;

    public NeScenarioNode() { }

    //Copy constructor
    public NeScenarioNode(NeScenarioNode other, SubIdentifierNode parentSubIdentifierNode = null, IdentifierNode parentIdentifierNode = null)
    {
        Name = other.Name;
        ParentSubIdentifierNode = parentSubIdentifierNode;
        ParentIdentifierNode = parentIdentifierNode;
        PRValues = new RelationshipValues(other.PRValues);  
        ModRValues = new RelationshipValues(other.ModRValues);
        ActionContext = other.ActionContext; // enums are value types, so direct copy is fine
        ResponseNodes = new List<NiDecisionNode>();
        ActionPlanNodes = new List<NiDecisionNode>();
        //Deep copy the ProceedingScenarios list
        ProceedingScenarios = other.ProceedingScenarios
            .Select(ps => (ps.neNode != null ? new NeScenarioNode(ps.neNode, parentSubIdentifierNode, parentIdentifierNode) : null, ps.habitCounter))
            .Where(t => t.Item1 != null) // skip nulls
            .ToList();

        //Deep copy the PrecedingScenarios list
        PrecedingScenarios = other.PrecedingScenarios
            .Select(ps => (ps.neNode != null ? new NeScenarioNode(ps.neNode, parentSubIdentifierNode, parentIdentifierNode) : null, ps.habitCounter))
            .Where(t => t.Item1 != null)
            .ToList();

        if (other.ResponseNodes != null)
        {
            foreach (var node in other.ResponseNodes)
            {
                ResponseNodes.Add(new NiDecisionNode(node, other));
            }
        }
        
    }

    public NeScenarioNode DeepCopy(SubIdentifierNode copyParentSubIdentifierNode = null, IdentifierNode copyParentIdentifierNode = null)
    {
        // Create the new RelationshipNode shell first
        var newNode = new NeScenarioNode(
            Name,
            new RelationshipValues(PRValues),
            new RelationshipValues(ModRValues),
            ActionContext,
            new List<NiDecisionNode>(), // empty for now
            new List<NiDecisionNode>(),
            HabitCounter,
            copyParentSubIdentifierNode,
            copyParentIdentifierNode
        );

        // Now copy ResponseNodes, linking them to the *new* RelationshipNode
        if (ResponseNodes != null)
        {
            foreach (var response in ResponseNodes)
            {
                newNode.ResponseNodes.Add(new NiDecisionNode(response, newNode));
            }
        }

        if(ActionPlanNodes != null)
        {
            foreach (var actionPlan in ActionPlanNodes)
            {
                newNode.ActionPlanNodes.Add(new NiDecisionNode(actionPlan, newNode));
            }
        }

        return newNode;
    }

    public NeScenarioNode(string name, RelationshipValues pRValues, RelationshipValues modRValues, EnumActionCharacteristics actionContext, 
        List<NiDecisionNode> responseNodes, List<NiDecisionNode> actionPlanNodes, int habitCounter = 0, SubIdentifierNode parentSubIdentifierNode = null, 
        IdentifierNode parentIdentifierNode = null, List<(NeScenarioNode neNode, int habitCounter)> proceedingScenarios = null,
        List<(NeScenarioNode neNode, int habitCounter)> precedingScenarios = null)
    {
        Name = name;
        ParentSubIdentifierNode = parentSubIdentifierNode;
        ParentIdentifierNode = parentIdentifierNode;
        PRValues = pRValues;
        ModRValues = modRValues;
        ActionContext = actionContext;
        ResponseNodes = responseNodes;
        ActionPlanNodes = actionPlanNodes;
        HabitCounter = habitCounter;
        ProceedingScenarios = proceedingScenarios;
        PrecedingScenarios = precedingScenarios;
    }
}

[System.Serializable]
public class NiDecisionNode
{
    public DecisionSO Decision;
    public NeScenarioNode ParentNeScenarioNode; 
    public RelationshipValues ModRValues;
    public int HabitCounter;

    public NiDecisionNode(NiDecisionNode other, NeScenarioNode parentNeScenarioNode) 
    { 
        Decision = other.Decision;
        ParentNeScenarioNode = parentNeScenarioNode;
        ModRValues = other.ModRValues;
        HabitCounter = other.HabitCounter;
    }
}

//Could delete
[System.Serializable]
public class ActionNode
{
    public string Name;
    public EnumActionCharacteristics Action;
    public RelationshipValues PRValues;
    public RelationshipValues ModRValues;
}
