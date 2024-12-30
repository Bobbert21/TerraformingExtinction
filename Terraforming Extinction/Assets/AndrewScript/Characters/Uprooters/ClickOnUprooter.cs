using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOnUprooter : ClickOnObj
{
    
    public bool UseStatContainer = false;
    public UprooterStateManager LocalUprooterStateManager;

    private UprooterStateManager uprooterStateManager
    {
        get => UseStatContainer ? GetComponent<UprooterStateStatContainer>().StateManager : LocalUprooterStateManager;
    }

    public GameObject ClickOn()
    {
        return uprooterStateManager.PlayerSelected();
    }
}

