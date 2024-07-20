using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    public string Name;
    public ItemType Type;
    public Sprite InventoryIcon;
    public Sprite GameImage;
}
