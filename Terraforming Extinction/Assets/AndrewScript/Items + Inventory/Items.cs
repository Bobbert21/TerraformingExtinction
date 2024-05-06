using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType{
    Fertilizers,
    Nutrients,
    Deposits
}

public class Items : MonoBehaviour
{

    public ItemType Type;
    public int MaxQuantityForInventorySpace;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
