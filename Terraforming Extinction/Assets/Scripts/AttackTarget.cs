using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackTarget : MonoBehaviour
{
    [SerializeField] TargetLocator targetLocator;
    [SerializeField] Transform target;
    [SerializeField] float distance;
    [SerializeField] float attackTimer;
    [SerializeField] float timer;

    [Header("Unit Settings")]
    [SerializeField] float attackSpeed;
    [SerializeField] float minDistance = 0.1f; // Closest the unit will get to target

    void Start()
    {
    }

    void Update()
    {
        target = targetLocator.GetClosestTarget(transform.position);
        if (target != null)
        {
            distance = RobbysUtils.TransformUtils.GetDistance(transform.position, target.position);

            if (distance <= minDistance)
            {
                if (timer <= 0)
                {
                    // FireProjectile()

                    target.parent.GetComponent<Health>().TakeDamage(5, 0, out bool isImmune);
                    timer = attackTimer;
                }
                else
                    timer -= Time.deltaTime;
            }
            else
                timer = attackTimer;
        }
    }
}
