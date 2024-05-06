using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public enum ItemType{
    RedFertilizers,
    BlueFertilizers,
    GreenFertilizers,
    YellowFertilizers,
    Nutrients,
    CommonDeposits,
    UncommonDeposits,
    RareDeposits,
    RandomDeposits
}

public class Items : MonoBehaviour
{
    //[HideInInspector]
    public ItemType Type;
    [HideInInspector]
    public Sprite InventoryIcon;
    [HideInInspector]
    public Sprite GameImage;
    //public int MaxQuantityForInventorySpace;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
