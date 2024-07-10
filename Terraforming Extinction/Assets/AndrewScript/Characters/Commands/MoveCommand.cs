using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommand : Commands
{
    private Vector2 targetLocation;

    public void SetTargetLocation(Vector2 targetLocation)
    { this.targetLocation = targetLocation; }

    public override void Execute(GameObject actor)
    {
        
        NavMeshAgent agent = actor.GetComponent<NavMeshAgent>();
        if(targetLocation == Vector2.zero)
        {
            targetLocation = actor.transform.position;
        }
        agent.SetDestination(targetLocation);
    }

    public override void Terminate(GameObject actor)
    {
        NavMeshAgent agent = actor.GetComponent<NavMeshAgent>();
        agent.ResetPath(); // Resetting the path stops the agent from moving
    }
}
