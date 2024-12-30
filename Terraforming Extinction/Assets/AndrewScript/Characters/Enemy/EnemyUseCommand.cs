using RobbysUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUseCommand : MonoBehaviour
{
    //public Transform target;
    NavMeshAgent agent;
    private CommandEngine commandEngine = new CommandEngine();
    private Commands command;
    //preferred charactertypes to go after
    public CharacterTypes[] characterTypes;
    public List<GameObject> objectsAround = new List<GameObject>();
    private float time = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
 
        if (other.gameObject != gameObject) // Ensure it’s not the same object
        {
            GameObject objectAround = other.gameObject;

            //check if it is a character
            if (objectAround.TryGetComponent<CharacterStateStatContainer>(out var objectAroundCharacterScript))
            {
                Debug.Log("object around script: " + objectAroundCharacterScript);
                objectsAround.Add(other.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.gameObject != gameObject) // Ensure it’s not the same object
        {
            objectsAround.Remove(other.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis= false;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 1)
        {
            
            if (command != null)
            {
                command.Terminate(gameObject);
            }

            command = commandEngine.DumbEngine(transform.position, characterTypes, objectsAround, 0);

            if (command != null)
            {
                command.Execute(gameObject);
            }
            time = 0;
        }
    }
}
