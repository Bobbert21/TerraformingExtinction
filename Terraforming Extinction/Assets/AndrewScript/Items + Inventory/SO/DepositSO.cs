using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositSO : ScriptableObject
{
   public enum DepositRarities
    {
        Common,
        Uncommon,
        Rare,
        Random
    }

    public DepositRarities DepositRarity;


}
