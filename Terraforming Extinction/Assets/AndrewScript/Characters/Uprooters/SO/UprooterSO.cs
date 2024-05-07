using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Set the fertilizer limit before moving to the next uprooter
[System.Serializable]
public class FertilizerTransition
{
    public FertilizerTypes type;
    public int limit;
    public UprooterSO uprooterTransition;
}


[CreateAssetMenu(fileName = "UprootersSO", menuName = "ScriptableObject/Uprooters")]
public class UprooterSO : ScriptableObject
{
    public string Name;
    public int Health;
    public int MaxHealth;
    public FertilizerTransition[] FertilizerTransitions;
}
