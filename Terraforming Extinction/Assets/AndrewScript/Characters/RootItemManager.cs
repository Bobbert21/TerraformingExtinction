using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DepositSO;

public class RootItemManager : MonoBehaviour
{
    public List<ItemSO> CommonItems = new();
    public List<ItemSO> UncommonItems = new();
    public List<ItemSO> RareItems= new();
    public List<ItemSO> AllowedInputItems = new();
    private int CommonCutoff = 100;
    private int UncommonCutoff;
    private int RareCutoff;
    public void RootConversion(ItemSO item)
    {
        //check if the item is in the allowed items
        if (AllowedInputItems.Contains(item))
        {
            //Remove from inventory
            InventoryManager.Instance.RemoveItem(item);
            int randomNumber = UnityEngine.Random.Range(0, 100);
            //check if item is depositSO and then use it
            if (item is DepositSO depositItem)
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


}
