using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FertilizerSO", menuName = "ScriptableObject/Fertilizers")]
public class FertilizerSO : ScriptableObject
{
    public enum FertilizerTypes
    {
        Red,
        Blue,
        White,
        Yellow,
        Green
    }

    public string Name;
    public ItemType Type;
    public FertilizerTypes FertilizerType;
    public Sprite InventoryIcon;
    public Sprite GameImage;
}
