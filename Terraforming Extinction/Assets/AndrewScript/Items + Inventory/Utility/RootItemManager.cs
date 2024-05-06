using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DepositSO;

public class RootItemManager : MonoBehaviour
{
    public List<Items> CommonItems = new();
    public List<Items> UncommonItems = new();
    public List<Items> RareItems= new();
    public InventoryManager PlayerInventory;
    public List<Items> AllowedInputItems = new();
    private int CommonCutoff = 100;
    private int UncommonCutoff;
    private int RareCutoff;
    public void RootConversion(Items item)
    {
        //check if the item is in the allowed items
        if (AllowedInputItems.Contains(item))
        {
            //Remove from inventory
            PlayerInventory.RemoveItem(item);
            int randomNumber = UnityEngine.Random.Range(0, 100);
            if (item is DepositItems depositItem)
            {
                if (depositItem.DepositRarity == DepositRarities.Random)
                {
                    UncommonCutoff = 40;
                    RareCutoff = 10;
                }
                else if (depositItem.DepositRarity == DepositRarities.Common)
                {
                    UncommonCutoff = 20;
                    RareCutoff = 5;
                }
                else if (depositItem.DepositRarity == DepositRarities.Uncommon)
                {
                    UncommonCutoff = 55;
                    RareCutoff = 5;
                }
                else if (depositItem.DepositRarity == DepositRarities.Rare)
                {
                    UncommonCutoff = 60;
                    RareCutoff = 40;
                }
            }

            List<Items> ChosenRarityItems = new();

            if (randomNumber <= RareCutoff)
            {
                ChosenRarityItems = RareItems;
            }
            else if (randomNumber <= UncommonCutoff)
            {
                ChosenRarityItems = UncommonItems;
            }
            else
            {
                ChosenRarityItems = CommonItems;
            }

            // Generate a random index within the range of valid indices for the list
            int randomItemIndex = UnityEngine.Random.Range(0, ChosenRarityItems.Count);
            //Add to inventory of player
            PlayerInventory.AddItem(ChosenRarityItems[randomItemIndex]);
        }
        
    }


}
