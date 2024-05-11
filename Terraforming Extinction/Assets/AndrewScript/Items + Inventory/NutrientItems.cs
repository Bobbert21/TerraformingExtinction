using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutrientItems : Items
{
    public NutrientSO NutrientStats;
    public int Amount;

    public NutrientItems(NutrientSO nutrientStats)
    {

        ItemType = ItemType.Nutrients;
        NutrientStats = nutrientStats;
    }

}
