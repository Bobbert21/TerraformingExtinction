using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FertilizerSO", menuName = "ScriptableObject/Fertilizers")]
public class FertilizerSO : ItemSO
{
    public enum FertilizerTypes
    {
        Red,
        Blue,
        White,
        Yellow,
        Green
    }

    public FertilizerTypes FertilizerType;
}
