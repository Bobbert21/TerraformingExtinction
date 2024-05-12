using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackTarget : MonoBehaviour
{
    [Header("Linked Components")]
    [SerializeField] TargetLocator targetLocator;
    [SerializeField] Transform target;

    [Header("Unit Settings")]
    [SerializeField] float reloadTimer;
    [SerializeField] float attackCooldownSpeed;
    [SerializeField] float minDistance = 0.1f; // Closest the unit will get to target
    [SerializeField] int projectileStatsID = 0;
    [SerializeField] LayerMask projectileLayerMask;

    [Header("Debugging")]
    [SerializeField] float distance;
    [SerializeField] float timer;

    public float ReloadTimer { get => reloadTimer; set => reloadTimer = value; }
    public float AttackCooldownSpeed { get => attackCooldownSpeed; set => attackCooldownSpeed = value; }
    public float MinDistance { get => minDistance; set => minDistance = value; }
    public int ProjectileStatsID { get => projectileStatsID; set => projectileStatsID = value; }
    public LayerMask ProjectileLayerMask { get => projectileLayerMask; set => projectileLayerMask = value; }

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
                    FireProjectile(target);

                    timer = reloadTimer;
                }
                else
                    timer -= Time.deltaTime * attackCooldownSpeed;
            }
            else
                timer = reloadTimer;
        }
    }

    private void FireProjectile(Transform target)
    {
        var projectile = ObjectPooler.SharedInstance.GetPooledObject("Projectile");

        projectile.transform.position = transform.position;
        var statsContainer = projectile.GetComponent<ProjectileStatsContainer>();
        statsContainer.ProjectileStatID = projectileStatsID;
        statsContainer.StartPos = transform.position;
        statsContainer.EndPos = target.position;
        statsContainer.ProjecileLayerMask = projectileLayerMask;

        projectile.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        
    }
}
