using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubIdentifierRelationshipNodeInfo
{
    public RelationshipNode RelationshipNode;
    public SubIdentifierNode SubIdentifierNode;

    public SubIdentifierRelationshipNodeInfo(SubIdentifierNode subIdentifierNode, RelationshipNode relationshipNode)
    {
        RelationshipNode = relationshipNode;
        SubIdentifierNode = subIdentifierNode;
    }
}

public class IdentityDetector : MonoBehaviour
{
    //will be passed by stat container in future
    public string testName;
    public List<EnumAppearanceCharacteristics> testAppearanceCharacteristics;
    public List<EnumActionCharacteristics> testActionCharacteristics;
    public EnumIdentifiers testEnumIdentifier;
    public RelationshipPersonalTreeSO testRelationshipPersonalTreeSO;
    public float testDistinctiveAbility;
    public float testExistingNodesDistinctiveAbility;
    public float testJudgementLevel;
    public float testExtrapolationLevel;
    public float testGeneralizationLevel;
    public bool IsDebugging = false;

    private CharacterMainCPort selfMainCPort;
    private DecisionMaking decisionMaking;
    private List<CharacterMainCPort> envMainCPorts;
    private double timePassed = 0;

    //Get all the surrounding env then process all the identifiers and pass it to DM
    private void Start()
    {
        if(IsDebugging == false)
        {
            selfMainCPort = GetComponent<CharacterMainCPort>();
            decisionMaking = GetComponent<DecisionMaking>();

            selfMainCPort.Initialize();

            if (selfMainCPort == null)
            {
                Debug.Log("selfMainCPort is null — is CharacterMainCPort on this GameObject?");
            }

            if (selfMainCPort.characterPsyche == null)
            {
                Debug.Log("characterPsyche is null — it may not have been initialized in CharacterMainCPort.Awake()");
            }


        }
        
    }


    private void Update()
    {
        if(IsDebugging == false)
        {
            timePassed += Time.deltaTime;
            selfMainCPort.Initialize();
            double processingSpeed = selfMainCPort.characterPsyche.ProcessingSpeed;
            float awarenessLevel = selfMainCPort.characterPsyche.AwarenessLevel;
            if (timePassed > processingSpeed)
            {
                envMainCPorts = new List<CharacterMainCPort>();
                //Number can be adjusted like 
                var foundColliders = Physics.OverlapSphere(Vector3.zero, awarenessLevel);

                foreach (Collider collider in foundColliders)
                {
                    if (collider.TryGetComponent(out CharacterMainCPort envMainCPort))
                    {
                        envMainCPorts.Add(envMainCPort);

                    }

                }

                if (envMainCPorts.Count > 0)
                {
                    Run();
                    envMainCPorts.Clear();
                }
            }

        }


    }

    //main function to run
    public void Run()
    {
        if (IsDebugging)
        {
            RelationshipPersonalTree testRelationshipPersonalTree = new RelationshipPersonalTree(testRelationshipPersonalTreeSO);
            RunSingle(
                testName,
                testAppearanceCharacteristics,
                testActionCharacteristics,
                testEnumIdentifier,
                testRelationshipPersonalTree,
                testDistinctiveAbility,
                testExistingNodesDistinctiveAbility,
                testJudgementLevel,
                testExtrapolationLevel,
                testGeneralizationLevel
            );
        }
        else
        {
            Dictionary<CharacterMainCPort, SubIdentifierRelationshipNodeInfo> envCPortToSubIdMap = new Dictionary<CharacterMainCPort, SubIdentifierRelationshipNodeInfo>();
            foreach (CharacterMainCPort envMainCPort in envMainCPorts)
            {
                //This is the subidentifier node of the env found in self's relationship personal tree
                //To-do: Convert this to a class to pass to
                SubIdentifierNode foundEnvSubIdentifierInRPT = RunSingle(
                    envMainCPort.Name,
                    envMainCPort.characterPhysical.appearanceCharacteristics,
                    envMainCPort.characterPhysical.actionCharacteristics,
                    envMainCPort.characterPhysical.Identifier,
                    selfMainCPort.characterPsyche.RelationshipPersonalTree,
                    selfMainCPort.characterPsyche.DistinctiveAbility,
                    selfMainCPort.characterPsyche.ExistingNodesDistinctiveAbility,
                    selfMainCPort.characterPsyche.JudgementLevel,
                    selfMainCPort.characterPsyche.ExtrapolationLevel,
                    selfMainCPort.characterPsyche.GeneralizationLevel
                );

                //This is the relationship node from the identifier node
                RelationshipNode foundRelationshipNodeFromEnv = findRelationshipNode(foundEnvSubIdentifierInRPT, envMainCPort.characterPhysical.ActionCommitting);

                envCPortToSubIdMap[envMainCPort] = new SubIdentifierRelationshipNodeInfo(foundEnvSubIdentifierInRPT, foundRelationshipNodeFromEnv);

                
            }

            decisionMaking.ActionSelection(envCPortToSubIdMap);
        }

    }

    private RelationshipNode findRelationshipNode(SubIdentifierNode foundSubIdentifierNode, EnumActionCharacteristics actionCommitting)
    {
        List<RelationshipNode> relationshipNodes = foundSubIdentifierNode.RelationshipNodes;

        foreach (RelationshipNode relationshipNode in relationshipNodes) 
        { 
            if(relationshipNode.ActionContext == actionCommitting)
            {
                return relationshipNode;
            }
        }

        //If hasn't found any nodes, then create a new one

        RelationshipNode newRelationshipNode = new RelationshipNode(actionCommitting.ToString(), new RelationshipValues(0, 0, 0), new RelationshipValues(0, 0, 0), actionCommitting, null);
        foundSubIdentifierNode.AddRelationshipNode(newRelationshipNode);

        return newRelationshipNode;
    }


    private SubIdentifierNode RunSingle(
    string envName,
    List<EnumAppearanceCharacteristics> envAppearanceCharacteristics,
    List<EnumActionCharacteristics> envActionCharacteristics,
    EnumIdentifiers envEnumIdentifier,
    RelationshipPersonalTree selfRelationshipPersonalTree,
    float selfDistinctiveAbility,
    float selfExistingNodesDistinctiveAbility,
    float selfJudgementLevel,
    float selfExtrapolationLevel,
    float selfGeneralizationLevel)
    {
        //1. Find with likeness
        //2. If perfect match or fit, then add underneath
        //3. If not, then Fit anchor right below the identifier node
        SubIdentifierNode foundSubIdentifierNode = null;
        float likenessScore = -1;
        (foundSubIdentifierNode, likenessScore) = AdaptiveIdentifierFunctions.FindSubidentifierNodeWithAppearanceAndActionWithLikenessScore(selfRelationshipPersonalTree, envEnumIdentifier, envAppearanceCharacteristics, envActionCharacteristics, envName);
        if (foundSubIdentifierNode != null) 
        {
            DebugManager.Instance?.SetIdentityDebugValue("FoundSubIdentifierNode", foundSubIdentifierNode.SubIdentifierName);
            DebugManager.Instance?.SetIdentityDebugValue("LikenessScore", likenessScore);
        }
        else
        {
            Debug.Log("No subidentifier found");
        }
        
        if (foundSubIdentifierNode != null) 
        {
            //perfect match or close enough to not distinct and have to be the same name. 
            if (likenessScore > selfDistinctiveAbility && foundSubIdentifierNode.SubIdentifierName == envName)
            {
                DebugManager.Instance?.SetIdentityDebugValue("PerfectMatchSubIdentifierNode", foundSubIdentifierNode.SubIdentifierName);
                //alters the relationship tree when perfect match (or similar enough) to be that specific thing
                selfRelationshipPersonalTree.AddValuesToSubIdentifier(foundSubIdentifierNode, selfExistingNodesDistinctiveAbility, selfJudgementLevel, selfExtrapolationLevel, selfGeneralizationLevel, 1);
                return foundSubIdentifierNode;
            }
            //FIT
            else if(likenessScore > selfDistinctiveAbility * 0.8)
            {
                DebugManager.Instance?.SetIdentityDebugValue("FitSubIdentifierNode", foundSubIdentifierNode.SubIdentifierName);
                float maxLikenessScore = AdaptiveIdentifierFunctions.GetMaxLikenessScorePossible(foundSubIdentifierNode);

                //ADOPT in FIT
                //1. Create new Subidentifier node and add underneath
                //2. Calculate adoptedValue
                //3. Populate the new LearningPeriodValues for characteristics and relationship nodes
                SubIdentifierNode newSubIdentifierNode = new SubIdentifierNode(foundSubIdentifierNode.Parent);
                newSubIdentifierNode.SubIdentifierName = envName;
                foundSubIdentifierNode.Specifics.Add(newSubIdentifierNode);
                newSubIdentifierNode.Heuristic = foundSubIdentifierNode;
                float adoptedValue = likenessScore / maxLikenessScore;


                //ADOPT
                //loop through foundSubIdentifierNode and get the new values for newSubIdentifierNode
                foreach(AppearanceCharacteristicWithValue appearanceCharacteristicWithValue in foundSubIdentifierNode.AppearanceCharacteristicsWithValue)
                {
                    //if value is large enough
                    int newValue = (int)System.MathF.Floor(appearanceCharacteristicWithValue.Value * adoptedValue);

                    //default to 1
                    newValue = Mathf.Max(newValue, 1);
                    if (newValue > 0)
                    {
                        AppearanceCharacteristicWithValue newAppearanceCharacteristicWithValue = new AppearanceCharacteristicWithValue(appearanceCharacteristicWithValue.CharacteristicType, newValue);
                        newSubIdentifierNode.LearningPeriodAppearanceCharacteristicsWithValue.Add(newAppearanceCharacteristicWithValue);
                        newSubIdentifierNode.AddAppearanceCharacteristic(appearanceCharacteristicWithValue.CharacteristicType, newValue);
                    }
                }

                //calculate new characteristics
                foreach (ActionCharacteristicWithValue actionCharacteristicWithValue in foundSubIdentifierNode.ActionCharacteristicsWithValue)
                {
                    int newValue = (int)System.MathF.Floor(actionCharacteristicWithValue.Value * adoptedValue);
                    //default to 1
                    newValue = Mathf.Max(newValue, 1);
                    if (newValue > 0)
                    {
                        ActionCharacteristicWithValue newActionCharacteristicWithValue = new ActionCharacteristicWithValue(actionCharacteristicWithValue.CharacteristicType, newValue);
                        newSubIdentifierNode.LearningPeriodActionCharacteristicsWithValue.Add(newActionCharacteristicWithValue);
                        newSubIdentifierNode.AddActionCharacteristic(actionCharacteristicWithValue.CharacteristicType, newValue);
                    }
                        
                }

                //adopt the relationship nodes
                foreach(RelationshipNode relationshipNode in foundSubIdentifierNode.RelationshipNodes)
                {
                    RelationshipValues newPRValues = new(relationshipNode.PRValues.LivelihoodValue * adoptedValue, relationshipNode.PRValues.DefensiveBelongingValue * adoptedValue, relationshipNode.PRValues.NurtureBelongingValue * adoptedValue);
                    RelationshipValues newModRValues = new(relationshipNode.ModRValues.LivelihoodValue * adoptedValue, relationshipNode.ModRValues.DefensiveBelongingValue * adoptedValue, relationshipNode.ModRValues.NurtureBelongingValue * adoptedValue);
                    RelationshipNode newRelationshipNode = new RelationshipNode(relationshipNode.Name, newPRValues, newModRValues, relationshipNode.ActionContext, relationshipNode.ResponseNodes);
                    newSubIdentifierNode.AddRelationshipNode(newRelationshipNode);  
                }

                return newSubIdentifierNode;
            }
        }
        //when there are no nodes with any similarity then just put it in the similar identifier
        else
        {

            bool foundIdentifierNode = false;
            foreach (IdentifierNode identifierNode in selfRelationshipPersonalTree.RootIdentifiers) { 
                if(identifierNode.Identifier == envEnumIdentifier)
                {
                    //add new subidentifier node
                    SubIdentifierNode newSubIdentifierNode = new SubIdentifierNode(identifierNode);
                    foreach(EnumAppearanceCharacteristics appearanceCharacteristic in envAppearanceCharacteristics)
                    {
                        newSubIdentifierNode.AddAppearanceCharacteristic(appearanceCharacteristic, 1);
                    }

                    foreach (EnumActionCharacteristics actionCharacteristic in envActionCharacteristics)
                    {
                        newSubIdentifierNode.AddActionCharacteristic(actionCharacteristic, 1);
                    }

                    //Adopt the Relationship node (identifier node has no appearance or action)
                    float adoptedValue = 1.0f;


                    foreach (RelationshipNode relationshipNode in identifierNode.RelationshipNodes)
                    {
                        RelationshipValues newPRValues = new(relationshipNode.PRValues.LivelihoodValue * adoptedValue, relationshipNode.PRValues.DefensiveBelongingValue * adoptedValue, relationshipNode.PRValues.NurtureBelongingValue * adoptedValue);
                        RelationshipValues newModRValues = new(relationshipNode.ModRValues.LivelihoodValue * adoptedValue, relationshipNode.ModRValues.DefensiveBelongingValue * adoptedValue, relationshipNode.ModRValues.NurtureBelongingValue * adoptedValue);
                        RelationshipNode newRelationshipNode = new RelationshipNode(relationshipNode.Name, newPRValues, newModRValues, relationshipNode.ActionContext, relationshipNode.ResponseNodes);
                        Debug.Log("Adding relationship node " + newRelationshipNode.Name + " to subidentifier node " + newSubIdentifierNode.SubIdentifierName);
                        newSubIdentifierNode.AddRelationshipNode(newRelationshipNode);
                    }

                    newSubIdentifierNode.SubIdentifierName = envName;
                    identifierNode.SubIdentifiers.Add(newSubIdentifierNode);
                    foundIdentifierNode = true;

                    DebugManager.Instance?.SetIdentityDebugValue("AddedSubIdentifierNode", newSubIdentifierNode.SubIdentifierName);
                    return newSubIdentifierNode;
                }
            }

            if (!foundIdentifierNode)
            {
                Debug.LogError("Could not find Identifier node for this subidentifier node");
                return null;
            }
        }

        return null;
    }
}
