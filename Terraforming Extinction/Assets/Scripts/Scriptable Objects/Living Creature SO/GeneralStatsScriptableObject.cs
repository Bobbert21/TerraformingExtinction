using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneralStatsScriptableObject", menuName = "ScriptableObject/General Stats")]
public class GeneralStatsScriptableObject : ScriptableObject
{
    [SerializeField] private EntityCategory entityCategory;
    [SerializeField] private List<LivingEntityStats> livingEntityStats;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    /// <remarks>
    /// This is an Indexer, allows you to define functionality for accessing the containing class like using a key on a Dictionary 
    /// <para></para>
    /// The same as using GetLivingEntityStats(projectileStatsID)
    /// <para></para>
    /// Example:
    /// <code>
    ///  var localStatsItem = GeneralStatsScriptableObject[livingEntityID]
    /// </code>
    /// </remarks>
    public LivingEntityStats this[int ID]
    {
        get { return livingEntityStats.SingleOrDefault(ps => ps.LivingEntityID == ID); }
    }

    public List<LivingEntityStats> LivingEntityStats { get => livingEntityStats; set => livingEntityStats = value; }

    public LivingEntityStats GetLivingEntityStats(int ID)
    {
        return livingEntityStats.SingleOrDefault(ps => ps.LivingEntityID == ID);
    }
}

[System.Serializable]
public class LivingEntityStats
{
    [SerializeField] private int livingEntityID;

    [SerializeField] private int maxHealth;
    [SerializeField] private float reloadSpeed;
    [SerializeField] AttackRangeCategory attackRangeType;
    [SerializeField] private float lineOfSight;
    [SerializeField] private StatusEffects statusWeaknesses;
    [SerializeField] private StatusEffects statusStrengths;
    [SerializeField] private int projectileStatsID;
    [SerializeField] private LayerMask projectileLayerMask;
    [SerializeField] private float minAttackDistance;

    public int LivingEntityID { get => livingEntityID; set => livingEntityID = value; }

    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float ReloadSpeed { get => reloadSpeed; set => reloadSpeed = value; }
    public AttackRangeCategory AttacRangeType { get => attackRangeType; set => attackRangeType = value; }
    public float LineOfSight { get => lineOfSight; set => lineOfSight = value; }
    public StatusEffects StatusWeaknesses { get => statusWeaknesses; set => statusWeaknesses = value; }
    public StatusEffects StatusStrengths { get => statusStrengths; set => statusStrengths = value; }
    public int ProjectileStatsID { get => projectileStatsID; set => projectileStatsID = value; }
    public LayerMask ProjectileLayerMask { get => projectileLayerMask; set => projectileLayerMask = value; }
    public float MinAttackDistance { get => minAttackDistance; set => minAttackDistance = value; }


    #region Stats that we will add after game jam/if we have enough time

    [Header("These will only be implemented after game jam/if we have time")]
    [SerializeField] private float armor;
    /// <summary>
    /// Effect for if we have enough time
    /// </summary>
    public float Armor { get => armor; set => armor = value; }

    
    [SerializeField] private float regenRate;
    /// <summary>
    /// Effect for if we have enough time
    /// </summary>
    public float RegenRate { get => regenRate; set => regenRate = value; }


    [SerializeField] private StatusEffects buffEffect;
    /// <summary>
    /// Effect for if we have enough time
    /// </summary>
    public StatusEffects BuffEffect { get => buffEffect; set => buffEffect = value; }


    [SerializeField] private PriorityType priorityType;
    /// <summary>
    /// Effect for if we have enough time
    /// </summary>
    public PriorityType PriorityType { get => priorityType; set => priorityType = value; }

    #endregion
}

#region Enums For General Stats

public enum EntityCategory
{
    TransformedHumanPlant,
    Alien,
    Human
}

public enum AttackRangeCategory
{
    Short,
    Mid,
    Long,
    Spawning,
}

/// <summary>
/// The different types of statuses
/// </summary>
/// <remarks>
/// INFO: Enums that use System.Flags NEED to equal of power of 2!
/// </remarks>
[System.Flags]
public enum StatusEffects
{
    Red = 0,
    Blue = 1,
    Green = 2,
    Yellow = 4,
    Black = 8,
    Gray = 8,
}

public enum PriorityType
{
    NonApplicable,
    Plants,
    Players,
    Roots
}
#endregion