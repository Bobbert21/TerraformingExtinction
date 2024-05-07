using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Speed at which the character moves

    // Update is called once per frame
    void Update()
    {
        var movement = new UnityEngine.Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed * Time.deltaTime;

        transform.Translate(movement);
    }
}
