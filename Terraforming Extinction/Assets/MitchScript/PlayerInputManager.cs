using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputManager : MonoBehaviour
{
    public float Speed = 5f; // Speed at which the character moves
    public float SelectionDistance = 30f;
    private GameObject ObjSelected;
    public Rigidbody2D RigidBody;

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();

        if (Input.GetMouseButtonDown(0))
        {
            EnvironmentSelection();
        }
    }

    private void PlayerMovement()
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
            CheckObjSelectedAvailability();
        }

        // Stop applying force when there's no input
        if (Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f))
        {
            RigidBody.velocity = Vector2.zero;
        }


    }

    private void CheckObjSelectedAvailability()
    {
        if (ObjSelected != null)
        {
            Collider2D playerCollider = gameObject.GetComponent<Collider2D>();
            Collider2D itemCollider = ObjSelected.GetComponent<Collider2D>();
            float distance = DistanceOfCollidersClosestPoints(playerCollider, itemCollider);
            if(distance > SelectionDistance)
            {
                if(ObjSelected.tag == "Root")
                {
                    InventoryManager.Instance.OfferBtn.gameObject.SetActive(false);
                    InventoryManager.Instance.CloseBtn.onClick.Invoke();
                }
                else if(ObjSelected.tag == "Plant")
                {
                    InventoryManager.Instance.FeedBtn.gameObject.SetActive(false);
                    InventoryManager.Instance.RejoiceBtn.gameObject.SetActive(false);
                    InventoryManager.Instance.CloseBtn.onClick.Invoke();
                }

                ObjSelected = null;
            }
        }
    }

    private void EnvironmentSelection()
    {
        //get all the objects in the area on the mouse click
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //draw ray from mouse with direction vector0 to see what it hits on the screen
        RaycastHit2D[] mouseCollidedEnvs = Physics2D.RaycastAll(mousePosition, Vector2.zero);

        //check for collision from mouse
        foreach (RaycastHit2D mouseCollidedEnv in mouseCollidedEnvs)
        {
            GameObject collidedEnvObj = mouseCollidedEnv.collider.gameObject;
            
            Collider2D objClickedCollider = mouseCollidedEnv.collider;
            Collider2D playerCollider = gameObject.GetComponent<Collider2D>();

            float distance = DistanceOfCollidersClosestPoints(objClickedCollider, playerCollider);

            Debug.Log("Distance from " + collidedEnvObj.name + " is " + distance);
                
            if (distance < SelectionDistance)
            {
                //Root clicked on
                if (collidedEnvObj.tag == "Root")
                {
                    ObjSelected = collidedEnvObj;
                    //Click on inventory button
                    InventoryManager.Instance.InventoryBtn.onClick.Invoke();
                    InventoryManager.Instance.Root = ObjSelected;
                    //set offer button
                    InventoryManager.Instance.OfferBtn.gameObject.SetActive(true);
                    break;
                }else if(collidedEnvObj.tag == "Plant")
                {
                    //Doing stuff with plants between waves, during waves or at start
                    if(GameManager.Instance.CurrentState == GameStates.WaveTransition || 
                        GameManager.Instance.CurrentState == GameStates.Start || 
                        GameManager.Instance.CurrentState == GameStates.WaveInProgress)
                    {
                        ObjSelected = collidedEnvObj;
                        //Click on inventory button
                        InventoryManager.Instance.InventoryBtn.onClick.Invoke();
                        //set offer button
                        InventoryManager.Instance.FeedBtn.gameObject.SetActive(true);
                        InventoryManager.Instance.Uprooter = ObjSelected;

                        //Start or transition and uprooter's inactive. Rejoice!
                        if (GameManager.Instance.CurrentState != GameStates.WaveInProgress &&
                            ObjSelected.GetComponent<UprooterManager>().State == UprooterState.Inactive &&
                            GameManager.Instance.CurrentNumOfUprooters < GameManager.Instance.MaxNumOfUprooters) 
                        {
                            Debug.Log("Current Uprooters:" + GameManager.Instance.CurrentNumOfUprooters + " Max num: " + GameManager.Instance.MaxNumOfUprooters);

                            InventoryManager.Instance.RejoiceBtn.gameObject.SetActive(true);

                            // Position the button above the target object
                            Vector2 rejoicePosition = (Vector2)ObjSelected.transform.position + (Vector2.up * (ObjSelected.GetComponent<SpriteRenderer>().bounds.size.y / 2)) + Vector2.up * 20f; // Adjust the height as needed
                            InventoryManager.Instance.RejoiceBtn.transform.position = Camera.main.WorldToScreenPoint(rejoicePosition);
                        }

                        break;
                    }
                    
                }
                else if(collidedEnvObj.tag == "Item")
                {
                    Debug.Log("Get Item");
                    InventoryManager.Instance.AddItem(collidedEnvObj.GetComponent<Items>().ItemScriptableObject, collidedEnvObj.GetComponent<Items>().Quantity);
                    Destroy(collidedEnvObj);
                }
            }
        }

    }

    public void SetUprooterActiveState()
    {
        if(ObjSelected.GetComponent<UprooterManager>() != null)
        {
            ObjSelected.GetComponent<UprooterManager>().State = UprooterState.Ready;
            InventoryManager.Instance.RejoiceBtn.gameObject.SetActive(false);
            GameManager.Instance.CurrentNumOfUprooters += 1;
        }
        
    }

    private float DistanceOfCollidersClosestPoints(Collider2D collider1, Collider2D collider2)
    {
        //closet distance between the colliders based on center of player and clicked object
        Vector2 closestPoint1 = collider1.ClosestPoint(collider2.bounds.center);
        Vector2 closestPoint2 = collider2.ClosestPoint(collider1.bounds.center);

        //calculate distance
        float distance = Vector2.Distance(closestPoint1, closestPoint2);
        // Highlight closest points with GameObjects
        HighlightPoint(closestPoint1, Color.red);
        HighlightPoint(closestPoint2, Color.blue);
        return distance;
    }

    // Custom method to highlight a point with a GameObject
    private void HighlightPoint(Vector3 point, Color color)
    {
        // Instantiate a small sphere GameObject at the specified point
        GameObject highlightObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        highlightObject.transform.position = point;
        highlightObject.transform.localScale = Vector3.one * 20f; // Adjust the scale of the GameObject to make it smaller
        highlightObject.GetComponent<Renderer>().material.color = color;
        Destroy(highlightObject, 1f); // Destroy the GameObject after a short delay to clean up
    }
}


