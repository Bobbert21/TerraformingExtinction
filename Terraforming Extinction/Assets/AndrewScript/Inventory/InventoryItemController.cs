using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    public ItemSO Item;
    public void RemoveItem()
    {

        InventoryManager.Instance.RemoveItem(Item.Type);
    }

    public void OnButtonClick()
    {
        InventoryManager.Instance.PlayerSelectingItem(Item);
        Debug.Log("Button clicked");
    }
}
