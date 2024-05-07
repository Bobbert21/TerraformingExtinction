using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    public ItemSO Item;
    public void RemoveItem()
    {

        InventoryManager.Instance.RemoveItem(Item.SpecificType);
    }

    public void OnButtonClick()
    {
        InventoryManager.Instance.SelectedItemFromPlayer(Item);
        Debug.Log("Button clicked");
    }
}
