using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DepositRarities
{
    Common,
    Uncommon,
    Rare,
    Random
}

[CreateAssetMenu(fileName = "DepositSO", menuName = "ScriptableObject/Deposits")]
public class DepositSO : ItemSO
{
  

    
    public DepositRarities DepositRarity;
    
}
