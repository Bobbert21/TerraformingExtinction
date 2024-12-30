using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class DialogueLinesWithRootStates
{
    public DialogueLines[] dialogueLines;
    public RootStates state;
}
[CreateAssetMenu(fileName = "RootLevelStatSO", menuName = "ScriptableObject/Characters/RootLevelStat")]
public class RootLevelStatSO : CharacterLevelStatSO
{
    public int NumOfUprooters;
    public DialogueLinesWithRootStates[] DialogueLinesWithStates;
    
}
