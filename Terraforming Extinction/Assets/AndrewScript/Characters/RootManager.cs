using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DepositSO;

public class RootManager : MonoBehaviour
{
    public RootSO RootStats;
    public int NutrientLevel;
    private int NutrientIntensity;

    public List<ItemSO> CommonItems = new();
    public List<ItemSO> UncommonItems = new();
    public List<ItemSO> RareItems = new();
    public List<ItemSO> AllowedInputItems = new();
    private int CommonCutoff = 100;
    private int UncommonCutoff;
    private int RareCutoff;
    public void RootConversion(ItemSO itemUsed)
    {
        //check if item is depositSO and then use it
        if (itemUsed is DepositSO depositUsed)
        {
            //check if the deposit is in the allowed items
            if (AllowedInputItems.Contains(itemUsed))
            {
                //Remove from inventory
                InventoryManager.Instance.RemoveItem(itemUsed);
                int randomNumber = UnityEngine.Random.Range(0, 100);
                if (depositUsed.DepositRarity == DepositRarities.Random)
                {
                    UncommonCutoff = 40;
                    RareCutoff = 10;
                }
                else if (depositUsed.DepositRarity == DepositRarities.Common)
                {
                    UncommonCutoff = 20;
                    RareCutoff = 5;
                }
                else if (depositUsed.DepositRarity == DepositRarities.Uncommon)
                {
                    UncommonCutoff = 55;
                    RareCutoff = 5;
                }
                else if (depositUsed.DepositRarity == DepositRarities.Rare)
                {
                    UncommonCutoff = 60;
                    RareCutoff = 40;
                }

                List<ItemSO> ChosenRarityItems = new();

                if (randomNumber <= RareCutoff)
                {
                    ChosenRarityItems = RareItems;
                    Debug.Log("Rare");
                }
                else if (randomNumber <= UncommonCutoff)
                {
                    ChosenRarityItems = UncommonItems;
                    Debug.Log("Uncommon");
                }
                else
                {
                    ChosenRarityItems = CommonItems;
                    Debug.Log("Common");
                }

                // Generate a random index within the range of valid indices for the list
                int randomItemIndex = UnityEngine.Random.Range(0, ChosenRarityItems.Count);
                //Add to inventory of player
                InventoryManager.Instance.AddItem(ChosenRarityItems[randomItemIndex]);
                InventoryManager.Instance.DisplayItems();
            }

        }
        else if (itemUsed is NutrientSO nutrientUsed)
        {
            Debug.Log("Nutrient used on root");
            NutrientIntensity += 1;
            //remove the fertilizer from inventory
            InventoryManager.Instance.RemoveItem(itemUsed);
            if (NutrientIntensity >= RootStats.NutrientTransitions.limit && RootStats.NutrientTransitions.limit != -1)
            {
                RootStats = (RootSO)RootStats.NutrientTransitions.characterTransition;
                NutrientLevel += 1;
                NutrientIntensity = 0;
                GameManager.Instance.MaxNumOfUprooters = RootStats.NumOfUprooters;
                Debug.Log("Transition to " + RootStats.Name);
            }
        }
    }

    private void Start()
    {
        GameManager.Instance.MaxNumOfUprooters = RootStats.NumOfUprooters;
    }
}
