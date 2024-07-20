using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfferingButton : MonoBehaviour
{
   public void Click()
    {
        InventoryManager.Instance.OfferingItemToRoot();
    }
}
