using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntityStatsContainer : MonoBehaviour
{
    [SerializeField] EntityCategory entityType;
    [SerializeField] int entityID;

    [SerializeField] Health health;
    [SerializeField] AttackTarget attackTarget;
    [SerializeField] MoveTowardsTarget moveTowardsTarget;
    [SerializeField] TargetLocator targetLocator;
    [SerializeField] EnemyUseCommand enemyMovement;

    [ReadOnly][SerializeField] private LivingEntityStats livingEntityStats;

    void OnEnable()
    {
        switch (entityType)
        {
            case EntityCategory.Alien:
                livingEntityStats = Globals.AlienStatsScriptableObject[entityID];
                break;
            case EntityCategory.TransformedHumanPlant:
                livingEntityStats = Globals.UprooterStatsScriptableObject[entityID];
                break;
        }

        health.SetMaxHealth(livingEntityStats.MaxHealth, true);
        attackTarget.ReloadTimer = livingEntityStats.ReloadSpeed;
        attackTarget.MinDistance = livingEntityStats.MinAttackDistance;
        attackTarget.ProjectileLayerMask = livingEntityStats.ProjectileLayerMask;
        attackTarget.ProjectileStatsID = livingEntityStats.ProjectileStatsID;
        targetLocator.GetComponent<CircleCollider2D>().radius = livingEntityStats.LineOfSight;
    }

    void Update()
    {
        
    }
}
