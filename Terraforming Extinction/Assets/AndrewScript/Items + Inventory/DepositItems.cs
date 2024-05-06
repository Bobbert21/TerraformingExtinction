using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DepositSO;

public class DepositItems : Items
{
    public DepositSO Deposit;
    public DepositRarities DepositRarity;

    public DepositItems()
    {
        //Type from base class
        Type = ItemType.Deposits;
        DepositRarity = Deposit.DepositRarity;
    }
}
