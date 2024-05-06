using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DepositSO", menuName = "ScriptableObject/Deposits")]
public class DepositSO : ScriptableObject
{
   public enum DepositRarities
    {
        Common,
        Uncommon,
        Rare,
        Random
    }

    public string Name;
    public ItemType Type;
    public DepositRarities DepositRarity;
    public Sprite InventoryIcon;
    public Sprite GameImage;
}
