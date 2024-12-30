using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class DialogueLinesWithUprooterStates
{
    public DialogueLines[] dialogueLines;
    public UprooterStates state;
}
[CreateAssetMenu(fileName = "UprooterLevelStatSO", menuName = "ScriptableObject/Characters/UprooterLevelStat")]
public class UprooterLevelStatSO : CharacterLevelStatSO
{
    public DialogueLinesWithUprooterStates[] DialogueLinesWithStates;
}
