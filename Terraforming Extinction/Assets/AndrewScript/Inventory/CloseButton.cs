using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    public bool UseInventoryManager = false;
    [Header("Local Variables")]
    public GameObject LocalInventory;
    public GameObject LocalFeedBtn;
    public GameObject LocalOfferingBtn;

    private GameObject Inventory
    {
        get => UseInventoryManager ? InventoryManager.Instance.Inventory : LocalInventory;
    }
    private GameObject FeedBtn
    {
        get => UseInventoryManager ? InventoryManager.Instance.FeedBtn : LocalFeedBtn;
    }

    private GameObject OfferingBtn
    {
        get => UseInventoryManager ? InventoryManager.Instance.OfferBtn : LocalOfferingBtn;
    }

    public void Click()
    {
        Inventory.SetActive(false);
        FeedBtn.SetActive(false);
        OfferingBtn.SetActive(false);
    }
}
