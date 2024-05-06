using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class InventoryItem
    {
        public Items individualItems;
        public int quantity;

        public InventoryItem(Items individualItems, int quantity)
        {
            this.individualItems = individualItems;
            this.quantity = quantity;
        }
    }

    
    public List<InventoryItem> ListOfInventory = new();
    public int MaxInventorySpace; 

    //Add Item. Default Add 1 quantity
    public void AddItem(Items itemAdded, int quantityAdded = 1)
    {
        InventoryItem existingItem = null;
        // Check if the item already exists in the inventory. If the list is 0, then item added won't be in list (null)
        if (ListOfInventory.Count > 0)
        {
            existingItem = ListOfInventory.Find(invItem => invItem.individualItems.Type == itemAdded.Type);
        }


        //if the item added does not exist
        if (existingItem == null)
        {
            //if doesn't exceed maximum inventory space
            if(ListOfInventory.Count + 1 <= MaxInventorySpace)
            {
                InventoryItem newItem = new InventoryItem(itemAdded, quantityAdded);
                ListOfInventory.Add(newItem);
            }
            else
            {
                Debug.Log("Don't have inventory space");
            }
            
        }
        else
        {
            existingItem.quantity += quantityAdded;
        }

        }

    public void RemoveItem(Items itemRemoved, int quantityRemoved = 1)
    {
        InventoryItem existingItem = null;
        // Check if the item already exists in the inventory. If the list is 0, then item added won't be in list (null)
        if (ListOfInventory.Count > 0)
        {
            existingItem = ListOfInventory.Find(invItem => invItem.individualItems.Type == itemRemoved.Type);
        }

        //if the item added does not exist
        if (existingItem == null)
        {
            Debug.Log("No item exists");
        }
        else
        {
            if(existingItem.quantity - quantityRemoved > 0)
            {
                existingItem.quantity -= quantityRemoved;
            }
            else if(existingItem.quantity - quantityRemoved == 0)
            {
                    ListOfInventory.Remove(existingItem);
            }
            else
            {
                Debug.Log("Not enough item quantity to remove");
            }
        }
    }

    //Add Item with Max Quantity. Default add 1 quantity if not specified. Archived for now
    public void AddItemWithMaxQuantityForInventoryArchived(Items itemAdded, int quantityAdded = 1)
    {
        InventoryItem existingItem = null;
        // Check if the item already exists in the inventory. If the list is 0, then item added won't be in list (null)
        if(ListOfInventory.Count> 0)
        {
            existingItem = ListOfInventory.Find(invItem => invItem.individualItems.Type == itemAdded.Type);
        }
        

        //if the item added does not exist
        if(existingItem == null)
        {
            //check how much inventory space the quantityAdded will take up
            int inventorySpaceTakingUp = Mathf.CeilToInt(quantityAdded / itemAdded.MaxQuantityForInventorySpace);
            //How much extra space beyond max space the item added will take up
            int excessiveSpace = Mathf.Max(quantityAdded - (MaxInventorySpace - ListOfInventory.Count)*itemAdded.MaxQuantityForInventorySpace, 0);
            Debug.Log(excessiveSpace + " will be removed due to lack of inventory space");

            //if quantity added would have inventory space greater than max space
            if(excessiveSpace > 0)
            {
                for(int i = 0; i < (MaxInventorySpace - ListOfInventory.Count); i++)
                {
                    InventoryItem newItem = new InventoryItem(itemAdded, itemAdded.MaxQuantityForInventorySpace);
                    ListOfInventory.Add(newItem);
                }
            }
            //if doesn't exceed maximum space
            else
            {
                //for each item above the maximum quantity for inventory space
                for(int i = 0; i < quantityAdded; i += itemAdded.MaxQuantityForInventorySpace)
                {
                    //if the amount of quantity added is less than the maxquantityforinventoryspace
                    if(quantityAdded < itemAdded.MaxQuantityForInventorySpace + i)
                    {
                        InventoryItem newItem = new InventoryItem(itemAdded,quantityAdded - i);
                        ListOfInventory.Add(newItem);
                    }
                    //if amount of quantity added left is more than the max quantity for inventory space
                    else
                    {
                        InventoryItem newItem = new InventoryItem(itemAdded, itemAdded.MaxQuantityForInventorySpace);
                        ListOfInventory.Add(newItem);
                    }
                   
                }
                
            }
        }
        //If item already exist in this space
        else
        {

        }
        
    }

    public void RemoveItem(Items item)
    {

    }

    public void Update()
    {
        
    }
}
