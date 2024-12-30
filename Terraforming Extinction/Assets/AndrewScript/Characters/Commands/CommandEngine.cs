using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandEngine 
{
    //Commands: Move, Re-target + move (run-away, redirected strike (More preferable enemy target appears), signaled movement - when other enemies signal where they are), wander(will randomly traverse to find anybody nearby), attack, EXTRA: dodge, explode

    //Parameters to analyze
    //Avg Distance to enemies
    //Avg Distance to allies
    //Number of enemies in vicinity
    //Number of allies to vicinity
    //Current health
    //Signal location
    //Randomness: higher the more random it would pick its commands

    //Modes: Fearful, Aggressive, Dumb, Learner (NN)

    //Dumb

    //Attack: target enemy nearby

    //Runaway: 
    private AttackCommand attackCommand;
    private MoveCommand moveCommand;
    private WanderCommand wanderCommand = new WanderCommand();
    private Commands commandOutput;
    private List<Commands> commandList;

    public CommandEngine()
    {
        attackCommand = new AttackCommand();
        moveCommand = new MoveCommand();
        wanderCommand = new WanderCommand();
        commandList = new List<Commands>();
        commandList.Add(attackCommand);
        commandList.Add(moveCommand);
        commandList.Add(wanderCommand);
    }

    //Input: Preferred Target List, List of Targets around them and distance (enemies), range of attack, signal location, randomness
    //Target list should be some way to identify generic types of characters
    public Commands DumbEngine(Vector2 currentPosition, CharacterTypes[] preferredTargets, List<GameObject> objectsAround, int randomnessLevel)
    {
        //if below randomness criteria, then output random command. Else:
        //X1: Goes through to find preferred target
        //a: If too far, then move to target
        //b: If close to attack range, then attack
        //2: Nobody nearby and signal location
        //a: Move to signal location
        //3: Nobody nearby and no signal location
        //b: Wander

        int randomNumber = Random.Range(1,100);

        commandOutput = null;
        if (randomNumber > randomnessLevel)
        {
            if (objectsAround.Count > 0)
            {
                foreach (CharacterTypes preferredTarget in preferredTargets)
                {
                    foreach (GameObject objectAround in objectsAround)
                    {
                        //if all objects around are not characters, then should do wander
                        CharacterStateStatContainer statsContainer = objectAround.GetComponent<CharacterStateStatContainer>();
                        //make sure they have a script of interest
                        if (statsContainer != null && statsContainer.Stats != null)
                        {
                            CharacterTypes objectCharacterType = statsContainer.Stats.CharacterType;
                            if (objectCharacterType == preferredTarget)
                            {
                                //check distance
                                float distance = Vector2.Distance(currentPosition, objectAround.transform.position);
                                //go towards
                                if (distance > 5)
                                {
                                    moveCommand.SetTargetLocation(objectAround.transform.position);
                                    commandOutput = moveCommand;
                                }
                                //near enough. Go attack
                                else
                                {
                                    commandOutput = attackCommand;
                                }

                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                commandOutput = wanderCommand;
            }
        }
        //random
        else
        {
            int randomIndex = Random.Range(0, commandList.Count);
            Debug.Log("Random");
            commandOutput = commandList[randomIndex];
        }
        

        return commandOutput;
    }
}
