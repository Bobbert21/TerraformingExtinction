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
}
[System.Serializable]
public class NutrientTransition
{
    public int limit;
    public CharacterBaseSO characterTransition;
}

    public abstract class CharacterBaseSO : ScriptableObject
    {
        public string Name;
        public int Health;
        public FertilizerTransition[] FertilizerTransitions;
        public NutrientTransition NutrientTransitions;
    }
