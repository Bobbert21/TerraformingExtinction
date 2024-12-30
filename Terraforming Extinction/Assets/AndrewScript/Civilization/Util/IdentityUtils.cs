using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IdentityUtils
{
    // Find IdentifierNode by its Identifiers enum value
    public static IdentifierNode FindIdentifierNodeByEnum(List<IdentifierNode> rootNodes, EnumIdentifiers identifier)
    {
        return FindIdentifierNodeRecursive(rootNodes, identifier);
    }

    // Recursive method to search through the tree
    private static IdentifierNode FindIdentifierNodeRecursive(List<IdentifierNode> nodes, EnumIdentifiers identifier)
    {
        foreach (IdentifierNode node in nodes)
        {
            if (node.Identifier == identifier)
            {
                return node;
            }

            // Recursively search in the children nodes
            IdentifierNode foundInChild = FindIdentifierNodeRecursive(node.Children, identifier);
            if (foundInChild != null)
            {
                return foundInChild;
            }
        }

        return null;
    }

    // Recursive method to copy an IdentifierNode and its children
    public static IdentifierNode CopyIdentifierNode(IdentifierNode originalNode)
    {
        IdentifierNode copyNode = new IdentifierNode
        {
            Identifier = originalNode.Identifier,
            Parent = originalNode.Parent,
            SubIdentifiers = new List<SubIdentifier>()
        };

        foreach (var subIdentifier in originalNode.SubIdentifiers)
        {
            SubIdentifier subCopy = new SubIdentifier(copyNode)
            {
                SubIdentifierName = subIdentifier.SubIdentifierName,
                Parent = copyNode,
                ActionCharacteristicsWithValue = new List<ActionCharacteristicWithValue>()
            };

            foreach (var characteristic in subIdentifier.ActionCharacteristicsWithValue)
            {
                subCopy.ActionCharacteristicsWithValue.Add(new ActionCharacteristicWithValue(characteristic.CharacteristicType, characteristic.Value));
            }

            copyNode.SubIdentifiers.Add(subCopy);
        }

        copyNode.Children = new List<IdentifierNode>();
        foreach (var child in originalNode.Children)
        {
            IdentifierNode childCopy = CopyIdentifierNode(child);
            childCopy.Parent = copyNode;
            copyNode.Children.Add(childCopy);
        }

        return copyNode;
    }

    /*
    // Find SubIdentifiers in a node that match an action
    //I'm not using this? Seems like i was supposed to use this for addvaluetosubidentifierwithnode but took it out
    public static List<SubIdentifier> GetSubIdentifiersByAction(IdentifierNode node, ICharacteristics action)
    {
        List<SubIdentifier> result = new List<SubIdentifier>();

        foreach (SubIdentifier subIdentifier in node.SubIdentifiers)
        {
            foreach (ActionCharacteristicWithValue characteristicWithValue in subIdentifier.ActionCharacteristicsWithValue)
            {
                if (characteristicWithValue.CharacteristicType.GeneralCharacteristic == action.GeneralCharacteristic)
                {
                    result.Add(subIdentifier);
                    break;
                }
            }
        }

        return result;
    }

    public static void AddValueAndTranscend(List<IdentifierNode> rootNodes, SubIdentifier identity, ICharacteristics characteristic, int value, int zeroPointCutoff)
    {
        
        SubIdentifier targetIdentity = null;

        //Find the subidentity in the identity tree
        foreach (var rootNode in rootNodes)
        {
            SubIdentifier foundSubIdentifier = FindSubIdentifierRecursive(rootNode, identity);
            if (foundSubIdentifier != null)
            {
                targetIdentity = foundSubIdentifier;
            }
        }

        //if you found the subidentity then add to subidentity and to the anchors of them
        while (targetIdentity != null) 
        {
            int characteristicsOccurrences = 0;
            //Get the right characteristics you are adding to in the identity
            foreach (ActionCharacteristicWithValue characteristicWithValue in targetIdentity.ActionCharacteristicsWithValue)
            {
                //find right action
                if (characteristicWithValue.CharacteristicType.GeneralCharacteristic == characteristic.GeneralCharacteristic)
                {
                    characteristicWithValue.Value += value;
                }

                // Handle zero point logic
                if (!identity.isZeroPoint)
                {
                    characteristicsOccurrences += characteristicWithValue.Value;
                    if (characteristicsOccurrences >= zeroPointCutoff)
                    {
                        targetIdentity.isZeroPoint = true;
                    }
                }
            }

            //transcend to anchor
            targetIdentity = targetIdentity.Anchor;
        }
    }
    */
    // Recursive helper method to search for the specific SubIdentifier in the tree
    private static SubIdentifier FindSubIdentifierRecursive(IdentifierNode currentNode, SubIdentifier targetSubIdentifier)
    {
        // Check if the current node has the subIdentifier we're looking for
        foreach (var subIdentifier in currentNode.SubIdentifiers)
        {
            if (subIdentifier == targetSubIdentifier)
            {
                return subIdentifier; // Return the matching subIdentifier
            }
        }

        // Recursively search in the children nodes
        foreach (var childNode in currentNode.Children)
        {
            SubIdentifier foundInChild = FindSubIdentifierRecursive(childNode, targetSubIdentifier);
            if (foundInChild != null)
            {
                return foundInChild; // Return the found subIdentifier from children if matched
            }
        }

        return null; // Return null if no match is found
    }
}
