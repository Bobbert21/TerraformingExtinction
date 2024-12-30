using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UprooterLevelingUpState : UprooterBaseState
{
    public override void Enter(UprooterStateManager stateManager)
    {

    }
    public override void Exit(UprooterStateManager stateManager)
    {
        stateManager.SwitchState(stateManager.PreviousState);
    }
    public override UprooterStates GetState()
    {
        return UprooterStates.Levelingup;
    }
    public override void SetState(UprooterStateManager statManager)
    {

    }

    public override void UpdateState(UprooterStateManager stateManager)
    {

        Exit(stateManager);
    }

    public override bool PlayerSelects(ICharacterStateManager stateManager)
    {
        throw new System.NotImplementedException();
    }

    public override void PlayerDeselects(ICharacterStateManager stateManager)
    {

    }
}
