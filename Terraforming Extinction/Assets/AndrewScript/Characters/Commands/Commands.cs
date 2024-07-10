using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Commands 
{
    public abstract void Execute(GameObject actor);

    public abstract void Terminate(GameObject actor);
}
