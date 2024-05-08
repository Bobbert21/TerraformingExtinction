using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FertilizerSO;

public class FertilizerItems : Items
{
    public FertilizerSO Fertilizer;
    [HideInInspector]
    public string Name;
    [HideInInspector]
    public FertilizerTypes FertilizerType;


    public FertilizerItems(FertilizerSO fertilizer)
    {
        Fertilizer = fertilizer;
        ItemScriptableObject = Fertilizer;
    }

    private void Awake()
    {
        ItemScriptableObject = Fertilizer;
        //Type from base class
        ItemType = Fertilizer.Type;
        Name = Fertilizer.Name;
        FertilizerType = Fertilizer.FertilizerType;
        InventoryIcon = Fertilizer.InventoryIcon;
        GameImage = Fertilizer.GameImage;
    }

   
    }
