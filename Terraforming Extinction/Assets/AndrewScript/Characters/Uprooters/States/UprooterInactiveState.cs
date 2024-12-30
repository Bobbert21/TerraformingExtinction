using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UprooterInactiveState : UprooterBaseState
{
    public override void Enter(UprooterStateManager stateManager)
    {

    }
    public override void Exit(UprooterStateManager stateManager)
    {

    }
    public override UprooterStates GetState()
    {
        return UprooterStates.Inactive;
    }
    public override void SetState(UprooterStateManager statManager)
    {

    }

    public override void UpdateState(UprooterStateManager stateManager)
    {

    }

    public override bool PlayerSelects(ICharacterStateManager stateManager) { 
        UprooterStateManager uprooterStateManager = (UprooterStateManager)stateManager;
        uprooterStateManager.CreateRejoiceButton();
        return true;
    }

    public override void PlayerDeselects(ICharacterStateManager stateManager)
    {

    }

}
