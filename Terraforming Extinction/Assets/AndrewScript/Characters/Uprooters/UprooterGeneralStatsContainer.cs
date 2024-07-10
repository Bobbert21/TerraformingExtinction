using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum UprooterStates
{
    None,
    Inactive,
    Wakingup,
    Ready,
    Levelingup,
    Battle,
    Damaged,
    Dead
}
public class UprooterGeneralStatsContainer : CharacterGeneralStatsContainer
{
    public UprooterStateManager StateManager;
    public int[] FertilizerIntensity;
    public int[] FertilizerLevel;
    public int NutrientIntensity;
    public int NutrientLevel;
    public UprooterDialogueController DialogueController;
    public UprooterItemUsedOn UseItemOn;

    private void Start()
    {
        FertilizerIntensity = new int[(int)FertilizerTypes.Count];
        FertilizerLevel = new int[(int)FertilizerTypes.Count];
    }
}
