using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Set the fertilizer limit before moving to the next uprooter
[System.Serializable]
public class FertilizerTransition
{
    public FertilizerTypes type;
    public int limit;
    public CharacterLevelStatSO characterTransition;
    public DialogueLines[] dialogue;
}
[System.Serializable]
public class NutrientTransition
{
    public int limit;
    public CharacterLevelStatSO characterTransition;
    public DialogueLines[] dialogue;
}

[System.Serializable]
public class DialogueLines
{
    public string[] dialogue;
}

public enum CharacterTypes
{
    Root,
    Uprooters,
    Aliens
}

public abstract class CharacterLevelStatSO : ScriptableObject
{
    public string Name;
    public CharacterTypes CharacterType;
    public FertilizerTransition[] FertilizerTransitions;
    public NutrientTransition NutrientTransitions;
    public int MinTimeForDialogue = 8;
    public int MaxTimeForDialogue = 15;

    /*
    public DialogueLines[] InactiveDialogue;
    public DialogueLines[] WakingUpDialogue;
    public DialogueLines[] WaveTransitionDialogue;
    public DialogueLines[] WaveInProgressDialogue;
    public DialogueLines[] DamagedDialogue;
    */

}

