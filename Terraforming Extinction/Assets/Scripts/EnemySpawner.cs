using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobbysUtils;

[ExecuteInEditMode]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Vector2[] spawnLocations;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] bool canClickForLocations;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canClickForLocations = !canClickForLocations;
        }

        if (canClickForLocations)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                spawnLocations = spawnLocations.AddItem(pos);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var spawnLocation in spawnLocations)
            {
                Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
            }
        }
    }
}
