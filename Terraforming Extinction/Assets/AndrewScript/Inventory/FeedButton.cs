using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedButton : MonoBehaviour
{
   public void Click()
    {
        InventoryManager.Instance.ItemOnUprooter();
    }
}
