using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool UseStatContainer = false;
    public float Speed = 5f; // Speed at which the character moves
    public Rigidbody2D RigidBody;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();

    }

    private void PlayerMove()
    {
        // Get input for horizontal and vertical movement using WASD keys
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // A and D keys
        float verticalInput = Input.GetAxisRaw("Vertical");     // W and S keys

        // Combine the input into a movement vector
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        // Apply the movement as a force to the Rigidbody
        RigidBody.velocity = movement * Speed;

        if (movement != Vector2.zero)
        {
            if(UseStatContainer)
            {
                GetComponent<PlayerGeneralStatsContainer>().playerSelects.CheckObjSelectedAvailability();
            }
        }

        // Stop applying force when there's no input
        if (Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f))
        {
            RigidBody.velocity = Vector2.zero;
        }


    }
}
