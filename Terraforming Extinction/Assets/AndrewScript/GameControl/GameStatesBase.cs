using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStatesBase 
{
    public abstract void EnterState(GameManager game);
    public abstract void UpdateState(GameManager game);
}
