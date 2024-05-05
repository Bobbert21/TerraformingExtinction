//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using RobbysUtils;

//public class CursorController : MonoBehaviour
//{
//    [SerializeField] bool useMouseInput = true;

//    //[SerializeField] PlayerInput playerInput;
//    [SerializeField] Transform cursor;
//    [SerializeField] float movementSpeed = 3f;

//    [SerializeField] ClickableObject prevHoveredObject;
//    [SerializeField] ClickableObject prevSelectedObject;
    
//    [SerializeField] ClickableObject hoveredObject;
//    [SerializeField] ClickableObject selectedObject;

//    void Start()
//    {
        
//    }

//    void Update()
//    {
//        if (useMouseInput)
//        {
//            cursor.position = RobbysUtils.BasicUtils.GetMouseWorldPosition();
//            Cursor.visible = false;
//        }
//        else
//        {
//            //Vector2 movement = playerInput.LeftStickRaw;
//            //cursor.position += (Vector3)movement * Time.deltaTime * movementSpeed;
//        }

//        PreventLeavingBounds();

//        //bool isClickEvent = playerInput.BottomButtonDown;
//        //bool isButtonDownEvent = playerInput.BottomButton;
//        //bool isButtonUpEvent = playerInput.BottomButtonUp;
//        var tempHover = hoveredObject;
//        var tempSelected = selectedObject;

//        RaycastHit2D hit = Physics2D.Raycast(cursor.position, Vector2.zero);
//        var newCO = hit.collider?.GetComponent<ClickableObject>();
//        if (newCO != null && (newCO.OwnerId == playerInput.GetPlayerID() || newCO.OwnerId == -1)) // For now -1 means global control
//            hoveredObject = newCO;
//        else
//            hoveredObject = null;

//        if (isClickEvent && hoveredObject?.ClickEvent != null)
//        {
//            tempSelected?.UnFocusedEvent.Invoke(null);  // Not Implemented Yet
//            selectedObject = hoveredObject;

//            selectedObject?.ClickEvent.Invoke(null);  // Not Implemented Yet
//        }
//        else if (isButtonDownEvent && selectedObject?.MouseDownEvent != null)
//        {
//            tempSelected?.UnFocusedEvent.Invoke(null);  // Not Implemented Yet
//            selectedObject = hoveredObject;

//            selectedObject?.MouseDownEvent.Invoke(null);  // Not Implemented Yet
//        }
//        else if (isButtonUpEvent && selectedObject?.MouseUpEvent != null)
//        {
//            selectedObject?.MouseUpEvent.Invoke(null);  // Not Implemented Yet
//        }
//        else
//        {
//            if (tempHover != selectedObject)
//                tempHover?.UnHoverEvent.Invoke(null);  // Not Implemented Yet

//            if (hoveredObject != selectedObject)
//                hoveredObject?.HoverEvent.Invoke(null);  // Not Implemented Yet
//        }
//    }

//    void PreventLeavingBounds()
//    {
//        CameraUtils.CameraBounds cameraBounds = CameraUtils.GetCameraViewBounds();

//        if (cursor.position.x > cameraBounds.RightBound.x)
//            cursor.position = new Vector2(cameraBounds.RightBound.x, cursor.position.y);
//        else if (cursor.position.x < cameraBounds.LeftBound.x)
//            cursor.position = new Vector2(cameraBounds.LeftBound.x, cursor.position.y);

//        if (cursor.position.y > cameraBounds.UpBound.y)
//            cursor.position = new Vector2(cursor.position.x, cameraBounds.UpBound.y);
//        else if (cursor.position.y < cameraBounds.DownBound.y)
//            cursor.position = new Vector2(cursor.position.x, cameraBounds.DownBound.y);
//    }
//}
