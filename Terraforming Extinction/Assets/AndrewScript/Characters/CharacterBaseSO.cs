using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Set the fertilizer limit before moving to the next uprooter
[System.Serializable]
public class FertilizerTransition
{
    public FertilizerTypes type;
    public int limit;
    public CharacterBaseSO characterTransition;
    public DialogueCreation[] dialogue;
}
[System.Serializable]
public class NutrientTransition
{
    public int limit;
    public CharacterBaseSO characterTransition;
    public DialogueCreation[] dialogue;
}

[System.Serializable]
public class DialogueCreation
{
    public string[] dialogue;
}

public enum CharacterTypes
{
    Uprooters,
    Aliens
}

public abstract class CharacterBaseSO : ScriptableObject
{
    public string Name;
    public int Health;
    public CharacterTypes CharacterType;
    public FertilizerTransition[] FertilizerTransitions;
    public NutrientTransition NutrientTransitions;
    
    public DialogueCreation[] InactiveDialogue;
    public DialogueCreation[] WakingUpDialogue;
    public DialogueCreation[] WaveTransitionDialogue;
    public DialogueCreation[] WaveInProgressDialogue;
    public DialogueCreation[] DamagedDialogue;

}

