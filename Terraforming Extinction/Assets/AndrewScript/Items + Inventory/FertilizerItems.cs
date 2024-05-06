using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FertilizerSO;

public class FertilizerItems : Items
{
    public FertilizerSO Fertilizer;
    public FertilizerTypes FertilizerType;

    public FertilizerItems()
    {
        //Type from base class
        Type = ItemType.Fertilizers;
        FertilizerType = Fertilizer.FertilizerType;
    }
}
