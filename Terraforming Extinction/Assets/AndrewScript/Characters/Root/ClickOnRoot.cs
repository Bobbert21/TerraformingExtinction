using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOnRoot : ClickOnObj
{
    public bool UseStatContainer = false;
    public RootStateManager LocalRootStateManager;

    private RootStateManager rootStateManager
    {
        get => UseStatContainer ? GetComponent<RootStateStatContainer>().StateManager : LocalRootStateManager;
    }

    public GameObject ClickOn()
    {
        return rootStateManager.PlayerSelected();
    }
}
