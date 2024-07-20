using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UprooterBaseState 
{
    public abstract void Enter(UprooterStateManager stateManager);
    public abstract void Exit(UprooterStateManager stateManager);
    public abstract UprooterStates GetState();
    public abstract void SetState(UprooterStateManager statManager);
    public abstract void UpdateState(UprooterStateManager stateManager);

    protected UprooterStates state;

}
