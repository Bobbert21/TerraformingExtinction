using System.Collections.Generic;
using UnityEngine;

public class IdentifierTracker : MonoBehaviour
{
    public IdentifierTrackerSO InitialIdentifierTracker;
    public int ZeroPointCutoff = 5;
    private IdentifierTrackerSO IdentifierTrackerTree;

    void Start()
    {
        if (InitialIdentifierTracker != null)
        {
            // Create a copy of the InitialIdentifierTracker using utility class
            IdentifierTrackerTree = CreateCopy(InitialIdentifierTracker);
        }
    }

    public void AddValueToSubidentity(SubIdentifier subIdentifier, ICharacteristics characteristic, int value = 1)
    {
        /*
        IdentityUtils.AddValueAndTranscend(IdentifierTrackerTree.RootIdentifiers, subIdentifier, characteristic, value, ZeroPointCutoff);
        */
    }

    // Deep copy creation, can still be moved to utility
    private IdentifierTrackerSO CreateCopy(IdentifierTrackerSO original)
    {
        IdentifierTrackerSO copy = ScriptableObject.CreateInstance<IdentifierTrackerSO>();
        copy.TreeCreator = original.TreeCreator;
        copy.RootIdentifiers = new List<IdentifierNode>();

        foreach (var rootIdentifier in original.RootIdentifiers)
        {
            copy.RootIdentifiers.Add(IdentityUtils.CopyIdentifierNode(rootIdentifier));
        }

        return copy;
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