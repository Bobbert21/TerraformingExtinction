using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class UprooterStateStatContainer : CharacterStateStatContainer
{
    public UprooterStateManager StateManager;
    public UprooterItemUsedOn UseItemOn;
    public ClickOnUprooter ClickOn;

    private void Start()
    {
        FertilizerIntensity = new int[(int)FertilizerTypes.Count];
        FertilizerLevel = new int[(int)FertilizerTypes.Count];
    }
}
