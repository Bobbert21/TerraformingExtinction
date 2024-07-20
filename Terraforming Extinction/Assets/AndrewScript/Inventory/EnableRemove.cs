using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRemove : MonoBehaviour
{
    public void Click()
    {
        InventoryManager.Instance.EnableItemRemove();
    }
}
