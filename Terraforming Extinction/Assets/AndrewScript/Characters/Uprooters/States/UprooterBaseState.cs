using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UprooterBaseState : ICharacterBaseState
{
    public abstract void Enter(UprooterStateManager stateManager);
    public abstract void Exit(UprooterStateManager stateManager);
    public abstract UprooterStates GetState();
    public abstract void SetState(UprooterStateManager statManager);
    public abstract void UpdateState(UprooterStateManager stateManager);

    public abstract bool PlayerSelects(ICharacterStateManager stateManager);
    public abstract void PlayerDeselects(ICharacterStateManager stateManager);

    protected UprooterStates state;

}
