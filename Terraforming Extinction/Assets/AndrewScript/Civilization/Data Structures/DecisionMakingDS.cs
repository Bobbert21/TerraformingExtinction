using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Include friend stats (F)
public enum EnumPersonalityStats
{
    L,
    NB,
    DB,
    FL,
    FNB,
    FDB,
    NL,
    NNB,
    NDB
}

[System.Serializable]
public enum DMTypes
{
    Instinct,
    Simple,
    Complex
}

[System.Serializable]
public enum RewardTypes
{
    SystemConsume,
    SystemResist,
    SystemFeed,
    SystemProtect,
    SystemEliminate,
    SystemGain,
    Depletion,
    Acquire,
    Stats
}

[System.Serializable]
public class Perspective
{
    public string Name;
    public string Description;
    public RewardTypes RewardType;
    public string Empathy;
    public string Target;
    public string Predictor;
    public int HabitCounter;
}

