using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobbysUtils;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] float animationSpeed;
    [SerializeField] float minScale;
    [SerializeField] float maxScale;

    [SerializeField] bool animate = true;
    [SerializeField] bool enlarge = true;

    void Start()
    {
        
    }

    void Update()
    {
        if (animate)
        {
            if (enlarge)
            {
                bool isFinished = TransformUtils.LerpLocalScaleTo(transform, maxScale, animationSpeed);
                if (isFinished)
                {
                    transform.localScale = Vector3.one * maxScale;
                    animate = false;
                }
            }
            else
            {
                bool isFinished = TransformUtils.LerpLocalScaleTo(transform, minScale, animationSpeed);
                if (isFinished)
                {
                    transform.localScale = Vector3.one * minScale;
                    animate = false;
                }
            }
        }
    }

    public void Animate(bool enlarge)
    {
        animate = true;
        this.enlarge = enlarge;
    }
}
