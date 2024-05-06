using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DepositSO;

public class DepositItems : Items
{

    public DepositSO Deposit;
    [HideInInspector]
    public string Name;
    [HideInInspector]
    public DepositRarities DepositRarity;

    public DepositItems(DepositSO deposit)
    {
        Deposit = deposit;
    }

    private void Awake()
    {
        Type = Deposit.Type;
        Name = Deposit.Name;
        DepositRarity = Deposit.DepositRarity;
        InventoryIcon = Deposit.InventoryIcon;
        GameImage = Deposit.GameImage;
       
    }

    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = GameImage;
    }


}
