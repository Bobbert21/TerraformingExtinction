using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]

public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand;
}

public class ObjectPooler : MonoBehaviour
{

    public static ObjectPooler SharedInstance;
    public List<ObjectPoolItem> itemsToPool;
    public List<GameObject> pooledObjects;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public int AddItemToPool(GameObject itemToPool, int amount, bool canExpand)
    {
        if (itemToPool != null)
        {
            if (!itemsToPool.Exists(i => i.objectToPool == itemToPool))
            {
                ObjectPoolItem objectPoolItem = new ObjectPoolItem()
                {
                    objectToPool = itemToPool,
                    amountToPool = amount,
                    shouldExpand = canExpand
                };

                itemsToPool.Add(objectPoolItem);

                for (int i = 0; i < objectPoolItem.amountToPool; i++)
                {
                    GameObject obj = (GameObject)Instantiate(objectPoolItem.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                }

                return 1;
            }
        }

        return 0;
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && !pooledObjects[i].activeSelf && pooledObjects[i].tag == tag)
            {
                return pooledObjects[i];
            }
        }

        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                if (item.shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }

        return null;
    }
}
