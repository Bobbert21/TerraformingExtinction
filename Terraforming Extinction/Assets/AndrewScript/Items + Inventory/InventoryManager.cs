using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //Create Singleton
    public static InventoryManager Instance;

    private void Awake()
    {
        Instance = this; 
    }

    [System.Serializable]
    public class InventoryItem
    {
        public ItemSO individualItems;
        public int quantity;

        public InventoryItem(ItemSO individualItems, int quantity)
        {
            this.individualItems = individualItems;
            this.quantity = quantity;
        }
    }

    public Transform ItemContent;
    public GameObject InventoryItemObj;
    public List<InventoryItem> ListOfInventory = new();
    public int MaxInventorySpace;
    public GameObject Root;
    public Toggle EnableRemove;
    public ItemSO testItem1;
    public ItemSO testItem2;
    private ItemSO ItemSelected;

    public void SelectedItemFromPlayer(ItemSO itemSelected)
    {
        ItemSelected = itemSelected;
    }

    //Add Item. Default Add 1 quantity
    public void AddItem(ItemSO itemAdded, int quantityAdded = 1)
    {
        InventoryItem existingItem = null;
        // Check if the item already exists in the inventory. If the list is 0, then item added won't be in list (null)
        if (ListOfInventory.Count > 0)
        {
            existingItem = ListOfInventory.Find(invItem => invItem.individualItems.SpecificType == itemAdded.SpecificType);
        }


        //if the item added does not exist
        if (existingItem == null)
        {
            //if doesn't exceed maximum inventory space
            if(ListOfInventory.Count + 1 <= MaxInventorySpace)
            {
                InventoryItem newItem = new InventoryItem(itemAdded, quantityAdded);
                ListOfInventory.Add(newItem);
                DisplayItems();
            }
            else
            {
                Debug.Log("Don't have inventory space");
            }
            
        }
        else
        {
            existingItem.quantity += quantityAdded;
            DisplayItems();
        }

        }

    //If given an item script
    public void RemoveItem(ItemSO itemRemoved, int quantityRemoved = 1)
    {
        RemoveItem(itemRemoved.SpecificType, quantityRemoved);
    }

    public void RemoveItem(SpecificType removedType, int quantityRemoved = 1)
    {
        InventoryItem existingItem = null;
        // Check if the item already exists in the inventory. If the list is 0, then item added won't be in list (null)
        if (ListOfInventory.Count > 0)
        {
            existingItem = ListOfInventory.Find(invItem => invItem.individualItems.SpecificType == removedType);
        }

        //if the item added does not exist
        if (existingItem == null)
        {
            Debug.Log("No item exists");
        }
        else
        {
            if (existingItem.quantity - quantityRemoved > 0)
            {
                existingItem.quantity -= quantityRemoved;
                DisplayItems();
            }
            else if (existingItem.quantity - quantityRemoved == 0)
            {
                ListOfInventory.Remove(existingItem);
                DisplayItems();
            }
            else
            {
                Debug.Log("Not enough item quantity to remove");
            }
        }
    }

    public bool CanRemoveItem(SpecificType removedType, int quantityToRemove = 1)
    {
        InventoryItem existingItem = null;
        // Check if the item already exists in the inventory. If the list is 0, then the item won't be in the list (null)
        if (ListOfInventory.Count > 0)
        {
            existingItem = ListOfInventory.Find(invItem => invItem.individualItems.SpecificType == removedType);
        }

        // If the item doesn't exist or there's not enough quantity to remove, return false
        if (existingItem == null || existingItem.quantity < quantityToRemove)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void DisplayItems()
    {
        //Clear out the items in inventory
        foreach(Transform inventoryItem in ItemContent)
        {
            Destroy(inventoryItem.gameObject);
        }
        //List out items
        foreach (var inventoryItem in ListOfInventory)
        {
            GameObject newItemObj = Instantiate(InventoryItemObj, ItemContent);
            var newItemScript = newItemObj.GetComponent<InventoryItemController>();
            var itemName = newItemObj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemIcon = newItemObj.transform.Find("ItemIcon").GetComponent<Image>();
            var itemQuantity = newItemObj.transform.Find("ItemQuantity").GetComponent<TextMeshProUGUI>();
            var itemRemoveBtn = newItemObj.transform.Find("RemoveItemButton").gameObject;

            newItemScript.Item = inventoryItem.individualItems;
            itemName.text = inventoryItem.individualItems.Name.ToString();
            itemIcon.sprite = inventoryItem.individualItems.InventoryIcon;
            itemQuantity.text = inventoryItem.quantity.ToString();
            itemRemoveBtn.SetActive(EnableRemove.isOn);
        }
    }

    public void EnableItemRemove()
    {
        if (EnableRemove.isOn)
        {
            foreach(Transform inventoryItem in ItemContent)
            {
                inventoryItem.Find("RemoveItemButton").gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform inventoryItem in ItemContent)
            {
                inventoryItem.Find("RemoveItemButton").gameObject.SetActive(false);
            }
        }
    }

    //can only offer quantity = 1 and 1 item at a time
    public void OfferingItemToRoot()
    {
        if (ItemSelected != null)
        {
            var rootItemManagerScript = Root.GetComponent<RootItemManager>();
            if (CanRemoveItem(ItemSelected.SpecificType))
            {
                rootItemManagerScript.RootConversion(ItemSelected);
            }
            else { Debug.Log("Not enough quantity to offer"); }
        }
        else { Debug.Log("No item selected"); }
        ItemSelected= null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            
            Debug.Log(testItem1.GeneralType.ToString());
            AddItem(testItem1);
        }else if(Input.GetKeyDown(KeyCode.B))
        {   
            Debug.Log(testItem2.SpecificType.ToString());
            AddItem(testItem2);
        }
    }


}












/*
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
           //unfinished
       }

   }
   */
