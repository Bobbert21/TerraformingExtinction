using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    [SerializeField] private ProjectileStatsScriptableObject projectileStatsScriptableObject;
    [SerializeField] private GeneralStatsScriptableObject plantStatsScriptableObject;
    [SerializeField] private GeneralStatsScriptableObject alienStatsScriptableObject;

    public static Globals Instance;

    public ProjectileStatsScriptableObject ProjectileStatsScriptableObjectInstance { get => projectileStatsScriptableObject; }

    public static ProjectileStatsScriptableObject ProjectileStatsScriptableObject { get; set; }
    public static GeneralStatsScriptableObject PlantStatsScriptableObject { get; set; }
    public static GeneralStatsScriptableObject AlienStatsScriptableObject { get; set; }

    void Awake()
    {
        Instance = this;

        ProjectileStatsScriptableObject = projectileStatsScriptableObject;
        PlantStatsScriptableObject = plantStatsScriptableObject;
        AlienStatsScriptableObject = alienStatsScriptableObject;
    }

    void Update()
    {
        
    }
}
