using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UprooterReadyState : UprooterBaseState
{
    public override void Enter(UprooterStateManager stateManager)
    {

    }
    public override void Exit(UprooterStateManager stateManager)
    {

    }
    public override UprooterStates GetState()
    {
        return UprooterStates.Ready;
    }
    public override void SetState(UprooterStateManager statManager)
    {

    }

    public override void UpdateState(UprooterStateManager stateManager)
    {

    }
    public override bool PlayerSelects(ICharacterStateManager stateManager)
    {
        InventoryManager.Instance.ShowInventoryOptionsWithUprooters(true);
        UprooterStateManager uprooterStateManager = (UprooterStateManager)stateManager;
        uprooterStateManager.ShowInventory();
        return true;
    }

    public override void PlayerDeselects(ICharacterStateManager stateManager)
    {
        InventoryManager.Instance.ShowInventoryOptionsWithUprooters(false);
    }


}
