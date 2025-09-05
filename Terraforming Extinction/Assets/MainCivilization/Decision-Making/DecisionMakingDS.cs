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
    NDB,
    None,
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


public class DMReturnPredictorCalculations
{
    public double largestPositivePredictorValue;
    public double largestNegativePredictorValue;
    public double largestPositivePredictorAdjustedChange;
    public double largestNegativePredictorAdjustedChange;
    public EnumPersonalityStats largestPositivePredictorStat;
    public EnumPersonalityStats largestNegativePredictorStat;
    public Perspective positivePerspective;
    public Perspective negativePerspective;

    public DMReturnPredictorCalculations() { }

    public DMReturnPredictorCalculations(EnumPersonalityStats statOfInterest) {
        this.largestPositivePredictorValue = double.MinValue;
        this.largestNegativePredictorValue = double.MaxValue;
        this.largestPositivePredictorAdjustedChange = double.MinValue;
        this.largestNegativePredictorAdjustedChange = double.MaxValue;
        this.largestPositivePredictorStat = statOfInterest;
        this.largestNegativePredictorStat = statOfInterest;
        this.positivePerspective = null;
        this.negativePerspective = null;

    }

    public void SetPredictorValues(double largestPositivePredictorValue, double largestNegativePredictorValue)
    {
        this.largestPositivePredictorValue = largestPositivePredictorValue;
        this.largestNegativePredictorValue = largestNegativePredictorValue;
    }

    public void SetChangeValues(double largestPositivePredictorChange, double largestNegativePredictorChange)
    {
        this.largestPositivePredictorAdjustedChange = largestPositivePredictorChange;
        this.largestNegativePredictorAdjustedChange = largestNegativePredictorChange;
    }

    public void SetPersonalityStats(EnumPersonalityStats largestPositivePredictorStat, EnumPersonalityStats largestNegativePredictorStat)
    {
        this.largestPositivePredictorStat = largestPositivePredictorStat;
        this.largestNegativePredictorStat = largestNegativePredictorStat;
    }

    public void SetPerspectives(Perspective positivePerspective, Perspective negativePerspective)
    {
        this.positivePerspective = positivePerspective;
        this.negativePerspective = negativePerspective;
    }

}
