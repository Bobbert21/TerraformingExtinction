using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FertilizerTypes
{
    Red,
    Blue,
    White,
    Yellow,
    Green,
    Count
}

[CreateAssetMenu(fileName = "FertilizerSO", menuName = "ScriptableObject/Fertilizers")]
public class FertilizerSO : ItemSO
{
   

    public FertilizerTypes FertilizerType;
}
