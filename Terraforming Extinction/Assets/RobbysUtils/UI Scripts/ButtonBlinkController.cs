using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBlinkController : MonoBehaviour
{
    SpriteRenderer sr;
    Sprite sprite;

    float buttonBlinkTimer;
    public float buttonBlink = 0.6f;

    void OnEnable()
    {
        buttonBlinkTimer = buttonBlink;
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sprite = sr.sprite;

        buttonBlinkTimer = buttonBlink;
    }

    void Update()
    {
        buttonBlinkTimer -= Time.deltaTime;
        if (buttonBlinkTimer <= 0)
        {
            if (sr.sprite == null)
            {
                sr.sprite = sprite;
            }
            else
            {
                sr.sprite = null;
            }
            buttonBlinkTimer = buttonBlink;
        }
    }
}
