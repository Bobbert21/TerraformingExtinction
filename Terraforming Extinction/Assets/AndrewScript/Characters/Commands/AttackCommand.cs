using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Commands
{
    public override void Execute(GameObject actor) 
    {
        Debug.Log("Attack!");
    }

    public override void Terminate(GameObject actor)
    {
        Debug.Log("Attack Terminated");
    }
}
