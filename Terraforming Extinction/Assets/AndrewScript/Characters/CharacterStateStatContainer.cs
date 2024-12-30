using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//accessed by things to find certain characters
public class CharacterStateStatContainer : MonoBehaviour
{
    public CharacterLevelStatSO Stats;
    public int[] FertilizerIntensity = new int[(int)FertilizerTypes.Count];
    public int[] FertilizerLevel = new int[(int)FertilizerTypes.Count];
    public int NutrientIntensity;
    public int NutrientLevel;
    public DialogueController DialogueController;
}
