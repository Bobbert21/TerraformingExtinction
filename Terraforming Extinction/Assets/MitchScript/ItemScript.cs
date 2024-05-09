using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider c)
    {
        Debug.Log("It failed");

        if (c.attachedRigidbody != null)
        {
            ItemCollector bc = c.attachedRigidbody.gameObject.GetComponent<ItemCollector>();
            if (bc != null)
            {
                Debug.Log("It reached");
                bc.collectItem();
                this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
        }
    }
}
