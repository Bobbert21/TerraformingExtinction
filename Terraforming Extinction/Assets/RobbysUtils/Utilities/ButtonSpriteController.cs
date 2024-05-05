using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSpriteController : MonoBehaviour
{
    [SerializeField] SpriteColorGroup spriteColorGroup;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void MouseDownEvent()
    {
        spriteColorGroup.SetColor(Color.grey);
    }

    public void MouseUpEvent()
    {
        spriteColorGroup.SetColor(Color.white);
    }

    public void UnHoverEvent()
    {
        spriteColorGroup.SetColor(Color.white);
    }
}
