using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UprooterManager : MonoBehaviour
{
    public static UprooterManager Instance { get; private set; }
    public int CurrentNumOfUprooters = 0;
    public int MaxNumOfUprooters = 10;

    private void Awake()
    {
        // Ensure there is only one instance of the GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
        // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
