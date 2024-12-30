using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootReadyState : RootBaseState
{
    public override void Enter(RootStateManager stateManager) 
    { 
    
    }
    public override void Exit(RootStateManager stateManager)
    {

    }
    public override RootStates GetState()
    {
        return RootStates.Ready;
    }
    public override void SetState(RootStateManager statManager)
    {

    }
    public override void UpdateState(RootStateManager stateManager) 
    { 
    
    }

    public override bool PlayerSelects(ICharacterStateManager stateManager)
    {
        InventoryManager.Instance.ShowInventoryOptionsWithRoot(true);
        RootStateManager rootStateManager = (RootStateManager)stateManager;
        rootStateManager.ShowInventory();
        return true;
    }

    public override void PlayerDeselects(ICharacterStateManager stateManager)
    {
        InventoryManager.Instance.ShowInventoryOptionsWithRoot(false);
    }

}
