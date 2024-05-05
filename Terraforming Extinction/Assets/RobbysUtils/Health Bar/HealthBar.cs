using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Transform fillRect;
    Vector3 targetLocalScale;
    [SerializeField] float animationSpeed = 10f;

    //void Update()
    //{
    //    fillRect.localScale = Vector3.Lerp(fillRect.localScale, targetLocalScale, Time.deltaTime * animationSpeed);
    //    if (RobbysUtils.TransformUtils.GetDistance(fillRect.localScale, targetLocalScale) < 0.001f)
    //    {
    //        fillRect.localScale = targetLocalScale;
    //    }
    //}

    public void SetFillColor(Color color)
    {
        fillRect.GetComponent<SpriteRenderer>().color = color;
    }

    public void SetHealth(float health)
    {
        fillRect.localScale = new Vector3(health / 100, 1, 1);
    }
}
