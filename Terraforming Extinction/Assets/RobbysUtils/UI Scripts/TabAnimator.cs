using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabAnimator : MonoBehaviour
{
    [SerializeField] bool instantMove = false;

    [SerializeField] Vector2 basePos;
    [SerializeField] Vector2 moveToPos;
    [SerializeField] float animationSpeed = 4f;
    [SerializeField] float minimumDistance = 0.1f;
    [SerializeField] bool moveUp = false;

    bool canClick = true;

    void Update()
    {
        if (!instantMove)
        {
            bool isFinished;
            if (moveUp)
                isFinished = RobbysUtils.TransformUtils.LerpPostionToPoint(transform, basePos, animationSpeed, minimumDistance);
            else
                isFinished = RobbysUtils.TransformUtils.LerpPostionToPoint(transform, moveToPos, animationSpeed, minimumDistance);

            canClick = isFinished;
        }
        else
        {
            if (moveUp)
                transform.position = basePos;
            else
                transform.position = moveToPos;

            canClick = true;
        }
    }

    public void ClickEvent()
    {
        if (canClick)
            moveUp = !moveUp;
    }
}
