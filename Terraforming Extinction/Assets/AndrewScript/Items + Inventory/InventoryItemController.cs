using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemController : MonoBehaviour
{
    public ItemType Type;
    public void RemoveItem()
    {

        InventoryManager.Instance.RemoveItem(Type);
    }
}
