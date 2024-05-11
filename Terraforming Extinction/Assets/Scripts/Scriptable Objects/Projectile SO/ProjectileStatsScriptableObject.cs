using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileStatsScriptableObject", menuName = "ScriptableObject/Projectile Stats")]
public class ProjectileStatsScriptableObject : ScriptableObject
{
    [SerializeField] private EntityCategory ownerEntityCategory;
    [SerializeField] private List<ProjectileStats> projectileStats;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    /// <remarks>
    /// This is an Indexer, allows you to define functionality for accessing the containing class like using a key on a Dictionary 
    /// <para></para>
    /// The same as using GetProjectileStats(projectileID)
    /// <para></para>
    /// Example:
    /// <code>
    ///  var localStatsItem = ProjectileStatsScriptableObject[projectileID]
    /// </code>
    /// </remarks>
    public ProjectileStats this[int ID]
    {
        get { return projectileStats.SingleOrDefault(ps => ps.ProjectileID == ID); }
    }

    public List<ProjectileStats> ProjectileStats { get => projectileStats; set => projectileStats = value; }

    public ProjectileStats GetProjectileStats(int ID)
    {
        return projectileStats.SingleOrDefault(ps => ps.ProjectileID == ID);
    }
}

[System.Serializable]
public class ProjectileStats
{
    [SerializeField] private int projectileID;

    [SerializeField] private int damage;
    [SerializeField] private float movementSpeed;
    [SerializeField] private AnimationCurve movementPath;
    [SerializeField] private float lifeSpan;
    [SerializeField] private int target;
    [SerializeField] private float affectedRadius;
    [SerializeField] private int pierce;
    [SerializeField] private float attackDelay;
    [SerializeField] private StatusEffects statusEffectsToApply;
    [SerializeField] private ColliderSettings colliderSettings;
    [SerializeField] private Sprite projectileSprite;
    [SerializeField] private bool canHitFriendlies;

    public int ProjectileID { get => projectileID; set => projectileID = value; }
    
    public int Damage { get => damage; set => damage = value; }
    public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
    /// <summary>
    /// Should this be the movement speed of the projectile at T time or is this supposed to be the path it moves in
    /// <para></para>
    /// I think this actually needs to somehow be an equation??
    /// </summary>
    public AnimationCurve MovementPath { get => movementPath; set => movementPath = value; }
    public float LifeSpan { get => lifeSpan; set => lifeSpan = value; }
    public int Target { get => target; set => target = value; }
    public float AffectedRadius { get => affectedRadius; set => affectedRadius = value; }
    public int Pierce { get => pierce; set => pierce = value; }
    public float AttackDelay { get => attackDelay; set => attackDelay = value; }
    public StatusEffects StatusEffectsToApply { get => statusEffectsToApply; set => statusEffectsToApply = value; }
    public ColliderSettings ColliderSettings { get => colliderSettings; set => colliderSettings = value; }
    public Sprite ProjectileSprite { get => projectileSprite; set => projectileSprite = value; }
    public bool CanHitFriendlies { get => canHitFriendlies; set => canHitFriendlies = value; }
}

[System.Serializable]
public class ColliderSettings
{
    [SerializeField] private Collider2DType collider2DType;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 dimensions;
    [SerializeField] private float radius;
    [SerializeField] private CapsuleDirection2D capsuleDirection2D;

    public Collider2DType Collider2DType { get => collider2DType; set => collider2DType = value; }
    public Vector2 Offset { get => offset; set => offset = value; }
    public Vector2 Dimensions { get => dimensions; set => dimensions = value; }
    public float Radius { get => radius; set => radius = value; }
    public CapsuleDirection2D CapsuleDirection2D { get => capsuleDirection2D; set => capsuleDirection2D = value; }
}

public enum Collider2DType
{
    Box,
    Circle,
    Capsule,
}