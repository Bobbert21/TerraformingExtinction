using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum GeneralType
{
    Fertilizers,
    Deposits
}
public enum SpecificType{
    RedFertilizers,
    BlueFertilizers,
    GreenFertilizers,
    YellowFertilizers,
    Nutrients,
    CommonDeposits,
    UncommonDeposits,
    RareDeposits,
    RandomDeposits
}

public class Items : MonoBehaviour
{
    [HideInInspector]
    public ItemSO ItemScriptableObject;
    //[HideInInspector]
    public SpecificType SpecificType;
    [HideInInspector]
    //may take out. Not needed 
    public GeneralType GeneralType;
    [HideInInspector]
    public Sprite InventoryIcon;
    [HideInInspector]
    public Sprite GameImage;
    //public int MaxQuantityForInventorySpace;

    public Items SOToSpecificItemObj(ItemSO inputSO)
    {
        // Check if inputSO is actually a FertilizerSO
        if (inputSO is FertilizerSO fertilizerSO)
        {
            return new FertilizerItems(fertilizerSO);
        }
        else if(inputSO is DepositSO depositSO)
        {
            return new DepositItems(depositSO);
        }
        else
        {
            // Handle the case where inputSO is not a FertilizerSO
            Debug.LogError("Error with conversion");
            return null;
        }
    }
}
