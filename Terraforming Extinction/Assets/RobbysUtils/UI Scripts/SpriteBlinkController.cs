using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBlinkController : MonoBehaviour
{
    [SerializeField] bool _canBlink = false;
    public bool CanBlink
    {
        get { return _canBlink; }
        set { _canBlink = value; }
    }

    SpriteRenderer sr;

    float spriteBlinkTimer;
    [SerializeField] float spriteBlinkTime = 0.2f;

    void OnEnable()
    {
        spriteBlinkTimer = spriteBlinkTime;
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        spriteBlinkTimer = spriteBlinkTime;
    }

    void Update()
    {
        if (CanBlink)
        {
            spriteBlinkTimer -= Time.deltaTime;
            if (spriteBlinkTimer <= 0)
            {
                sr.enabled = !sr.enabled;

                spriteBlinkTimer = spriteBlinkTime;
            }
        }
        else
        {
            if (!sr.enabled)
                sr.enabled = true;
        }
    }
}
