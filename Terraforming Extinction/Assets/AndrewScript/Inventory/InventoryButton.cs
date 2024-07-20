using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    public bool UseInventoryManager = false;
    [Header("Local Variables")]
    public GameObject LocalInventory;

    private void Start()
    {
       
    }
    private GameObject Inventory
    {
        get => UseInventoryManager ? InventoryManager.Instance.Inventory : LocalInventory;
    }

    public void Click()
    {
        Inventory.SetActive(!Inventory.activeInHierarchy);
        InventoryManager.Instance.DisplayItems();
    }
}
