using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBlinkController : MonoBehaviour
{
    Image image;
    Sprite sprite;

    float buttonBlinkTimer;
    public float buttonBlink = 0.6f;

    void OnEnable()
    {
        buttonBlinkTimer = buttonBlink;
    }

    void Start()
    {
        image = GetComponent<Image>();
        sprite = image.sprite;

        buttonBlinkTimer = buttonBlink;
    }

    void Update()
    {
        if (!image.enabled)
        {
            buttonBlinkTimer = buttonBlink;
            return;
        }

        buttonBlinkTimer -= Time.deltaTime;
        if (buttonBlinkTimer <= 0)
        {
            if (image.sprite == null)
            {
                image.color = new Color(1, 1, 1, 1);
                image.sprite = sprite;
            }
            else
            {
                image.color = new Color(1, 1, 1, 0);
                image.sprite = null;
            }
            buttonBlinkTimer = buttonBlink;
        }
    }
}
