using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DepositSO", menuName = "ScriptableObject/Deposits")]
public class DepositSO : ItemSO
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
