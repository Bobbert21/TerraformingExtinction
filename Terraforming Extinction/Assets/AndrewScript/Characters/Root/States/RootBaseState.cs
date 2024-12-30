using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RootBaseState: ICharacterBaseState
{
    public abstract void Enter(RootStateManager stateManager);
    public abstract void Exit(RootStateManager stateManager);
    public abstract RootStates GetState();
    //I think it sets a new state to transition to
    public abstract void SetState(RootStateManager statManager);
    public abstract void UpdateState(RootStateManager stateManager);

    public abstract bool PlayerSelects(ICharacterStateManager stateManager);
    public abstract void PlayerDeselects(ICharacterStateManager stateManager);

    protected RootStates state;
}
