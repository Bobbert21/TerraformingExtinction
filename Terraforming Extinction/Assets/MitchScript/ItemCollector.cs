using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    public int itemsCollected = 0;

    public void collectItem()
    {
        itemsCollected++;
    }

}
