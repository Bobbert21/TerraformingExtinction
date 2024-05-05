using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsTarget : MonoBehaviour
{
    [SerializeField] TargetLocator targetLocator;
    [SerializeField] Transform target;
    [SerializeField] float distance;

    [Header("Unit Settings")]
    [SerializeField] float movementSpeed;
    [SerializeField] float minDistance = 0.1f; // Closest the unit will get to target

    void Start()
    {
        
    }

    void Update()
    {
        target = targetLocator.GetClosestTarget(transform.position);

        if (target != null )
        {
            distance = RobbysUtils.TransformUtils.GetDistance(transform.position, target.position);

            if (distance > minDistance)
                transform.position += (target.position - transform.position) * Time.deltaTime * movementSpeed;
        }
    }
}
