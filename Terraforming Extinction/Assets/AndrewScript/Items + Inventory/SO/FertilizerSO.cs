using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilizerSO : ScriptableObject
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
    //need to add sprites
}
