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

    //Create Singleton
    public static PlayerSelects Instance;

    private void Awake()
    {
        Instance = this;
    }

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
                    //make functions in Inventory Manager
                    var rootGeneralStatScript = ObjSelected.GetComponent<RootStateStatContainer>();
                    RootStateManager rootStateManager = rootGeneralStatScript.StateManager;
                    rootStateManager.PlayerDeselected();
                }
                else if(ObjSelected.tag == "Plant")
                {
                    //make functions in Inventory Manager
                    var uprooterGeneralStatScript = ObjSelected.GetComponent<UprooterStateStatContainer>();
                    UprooterStateManager uprooterStateManager = uprooterGeneralStatScript.StateManager;
                    uprooterStateManager.PlayerDeselected();
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
                    Debug.Log("Clicked on Root");
                    var ClickOnScript = collidedEnvObj.GetComponent<RootStateStatContainer>().ClickOn;
                    //run the playerselected action and receive the object of the gameobject selected 
                    GameObject rootSelected = ClickOnScript.ClickOn();
                    if (rootSelected != null)
                    {
                        ObjSelected = rootSelected;
                    }
                    break;
                }
                else if (collidedEnvObj.tag == "Plant")
                {
                    Debug.Log("Into plant");

                    var ClickOnScript = collidedEnvObj.GetComponent<UprooterStateStatContainer>().ClickOn;
                    //run the playerselected action and receive the object of the gameobject selected 
                    GameObject uprooterSelected = ClickOnScript.ClickOn();
                    if (uprooterSelected != null)
                    {
                        ObjSelected = uprooterSelected;
                    }
                }
                else if (collidedEnvObj.tag == "Item")
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


