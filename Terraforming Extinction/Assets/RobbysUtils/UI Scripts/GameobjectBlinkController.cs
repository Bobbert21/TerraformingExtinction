using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameobjectBlinkController : MonoBehaviour
{
    [SerializeField] GameObject gameObjectToBlink;

    [SerializeField] float blinkToggleTime;

    float timer;

    void OnEnable()
    {
        timer = blinkToggleTime;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (timer <= 0)
        {
            gameObjectToBlink.SetActive(!gameObjectToBlink.activeSelf);

            timer = blinkToggleTime;
        }
        else
            timer -= Time.deltaTime;
    }
}
