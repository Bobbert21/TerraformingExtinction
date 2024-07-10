using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WanderCommand : Commands
{
    public override void Execute(GameObject actor)
    {
        Vector2 targetLocation = WanderLocation(actor.transform.position);
        NavMeshAgent agent = actor.GetComponent<NavMeshAgent>();
        
        agent.SetDestination(targetLocation);
    }

    //continuously go to a random point in the opposite side of the screen
    private Vector2 WanderLocation(Vector2 currentLocation)
    {
        //Figure out what quandrant it's in
        Vector2 worldBoundaries = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        float centerX = worldBoundaries.x / 2;
        float centerY = worldBoundaries.y / 2;

        float screenX = currentLocation.x < centerX ? Random.Range(centerX, Screen.width) : Random.Range(0, centerX);
        float screenY = currentLocation.y < centerY ? Random.Range(centerY, Screen.height) : Random.Range(0, centerY);

        Vector2 screenPoint = new Vector2(screenX, screenY);
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);

        return worldPoint;
    }

    public override void Terminate(GameObject actor)
    {
        NavMeshAgent agent = actor.GetComponent<NavMeshAgent>();
        agent.ResetPath(); // Resetting the path stops the agent from moving
    }
}
