using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSelects : MonoBehaviour
{
    public float SelectionDistance = 5f;
    public GameObject ObjSelected;
    public Canvas canvas;
    public Button RejoiceBtn;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            EnvironmentSelection();
        }
    }

    

    public void CheckObjSelectedAvailability()
    {
        if (ObjSelected != null)
        {
            Collider2D playerCollider = gameObject.GetComponent<Collider2D>();
            Collider2D itemCollider = ObjSelected.GetComponent<Collider2D>();
            float distance = DistanceOfCollidersClosestPoints(playerCollider, itemCollider);
            //stop showing inventory options when too far away
            if(distance > SelectionDistance)
            {
                if(ObjSelected.tag == "Root")
                {
                    InventoryManager.Instance.ShowInventoryOptionsWithRoot(false);
                }
                else if(ObjSelected.tag == "Plant")
                {
                    //make functions in Inventory Manager
                    InventoryManager.Instance.ShowInventoryOptionsWithUprooters(false);
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
            //ObjSelected = null;
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
                    InventoryManager.Instance.ShowInventoryOptionsWithRoot(true);
                    InventoryManager.Instance.Root = ObjSelected;
                    break;
                }else if(collidedEnvObj.tag == "Plant")
                {
                    Debug.Log("Into plant");
                    
                    var uprooterGeneralStatScript = collidedEnvObj.GetComponent<UprooterGeneralStatsContainer>();
                    UprooterStates uprooterState = uprooterGeneralStatScript.StateManager.GetState();
                    Debug.Log(uprooterState);
                    //If ready or battling
                    //move these into the statemanager code
                    if(uprooterState == UprooterStates.Ready||
                        uprooterState == UprooterStates.Battle )
                    {
                        ObjSelected = collidedEnvObj;
                        InventoryManager.Instance.ShowInventoryOptionsWithUprooters(true);
                        InventoryManager.Instance.Uprooter = ObjSelected;
                        break;
                    }
                    //if inactive, then can choose to rejoice
                    else if (uprooterState == UprooterStates.Inactive &&
                            UprooterManager.Instance.CurrentNumOfUprooters < UprooterManager.Instance.MaxNumOfUprooters)
                    {
                        ObjSelected = collidedEnvObj;
                        Button newRejoice = Instantiate(RejoiceBtn);

                        // Set the button as a child of the canvas
                        newRejoice.transform.SetParent(canvas.transform, false); // Set 'false' to preserve world position

                        // Get the RectTransform component of the button
                        RectTransform rejoiceRectTransform = newRejoice.GetComponent<RectTransform>();

                        // Calculate the position above the target object in world space
                        Vector3 targetPosition = ObjSelected.transform.position;
                        Vector3 rejoicePosition = new Vector3(targetPosition.x, targetPosition.y + ObjSelected.GetComponent<SpriteRenderer>().bounds.size.y / 2 + 0.5f, targetPosition.z);

                        // Convert the world position to screen space
                        Vector2 screenPoint = Camera.main.WorldToScreenPoint(rejoicePosition);

                        // Convert the screen space position to local position within the canvas
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, null, out Vector2 canvasPos);
                        rejoiceRectTransform.localPosition = canvasPos;

                        newRejoice.gameObject.SetActive(true);
                        newRejoice.GetComponent<Rejoice>().setPlayer(this);
                        newRejoice.GetComponent<Rejoice>().setObject(ObjSelected);
                        Debug.Log("Rejoice set");
                        break;
                    }

                }
                else if(collidedEnvObj.tag == "Item")
                {
                    Debug.Log("Get Item");
                    InventoryManager.Instance.AddItem(collidedEnvObj.GetComponent<Items>().ItemScriptableObject, collidedEnvObj.GetComponent<Items>().Quantity);
                    Destroy(collidedEnvObj);
                    break;
                }
            }
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
        //HighlightPoint(closestPoint1, Color.red);
        //HighlightPoint(closestPoint2, Color.blue);
        return distance;
    }

    // Custom method to highlight a point with a GameObject
    private void HighlightPoint(Vector3 point, Color color)
    {
        // Instantiate a small sphere GameObject at the specified point
        GameObject highlightObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        highlightObject.transform.position = point;
        highlightObject.transform.localScale = Vector3.one * 5f; // Adjust the scale of the GameObject to make it smaller
        highlightObject.GetComponent<Renderer>().material.color = color;
        Destroy(highlightObject, 1f); // Destroy the GameObject after a short delay to clean up
    }
}


