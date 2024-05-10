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
    RandomDeposits,
    CorpseResidues
}

public class Items : MonoBehaviour
{
    public SpriteRenderer ItemSpriteRenderer;
    public BoxCollider2D ItemCollider2D;
    public int Quantity;
    [HideInInspector]
    public ItemSO ItemScriptableObject;
    public ItemType ItemType { get; set; }
    [HideInInspector]
    public Sprite InventoryIcon;
    [HideInInspector]
    public Sprite GameImage;
    //public int MaxQuantityForInventorySpace;

    public Items SOToSpecificItemObj(ItemSO inputSO)
    {
        // Check if inputSO is actually a FertilizerSO
        if (inputSO is FertilizerSO fertilizerSO)
        {
            return new FertilizerItems(fertilizerSO);
        }
        else if(inputSO is DepositSO depositSO)
        {
            return new DepositItems(depositSO);
        }
        else
        {
            // Handle the case where inputSO is not a FertilizerSO
            Debug.LogError("Error with conversion");
            return null;
        }
    }

    protected void ResizeCollider()
    {
        if (ItemSpriteRenderer.sprite != null && ItemCollider2D != null)
        {
            // Get the size of the sprite
            Vector2 spriteSize = ItemSpriteRenderer.sprite.bounds.size;

            // Resize the collider to match the size of the sprite
            ItemCollider2D.size = spriteSize;

            ItemCollider2D.offset = new Vector2(spriteSize.x / 2, 0);
        }
    }
}
