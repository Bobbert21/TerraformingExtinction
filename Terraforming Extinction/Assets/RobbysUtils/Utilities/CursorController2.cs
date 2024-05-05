//using RobbysUtils;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//public class CursorClickController2 : MonoBehaviour
//{
//    [Header("Cursor Settings")]
//    [SerializeField] bool useMouseInput = true;
//    [SerializeField] LayerMask blockingMask;
//    [SerializeField] float movementSpeed = 3f;

//    [Header("Local References")]
//    //[SerializeField] PlayerInput playerInput;
//    [SerializeField] Transform cursor;
//    [SerializeField] SpriteRenderer cursorInsideRenderer;
//    [SerializeField] SpriteRenderer cursorOutsideRenderer;

//    [Header("Unity Events")]
//    [Tooltip("Events that occur when the action occurs, regardless of if there was a clickable object found")]
//    [SerializeField] CustomClickUnityEvent OnMouseDownEvent;
//    [SerializeField] CustomClickUnityEvent OnMouseUpEvent;
//    [SerializeField] CustomClickUnityEvent OnClickEvent;

//    [Header("Debugging")]
//    [SerializeField] bool allowInteraction;
//    [SerializeField] bool allowMovingCursor;
//    [SerializeField] ClickableObject hoveredCO;
//    [SerializeField] ClickableObject mouseDownCO;
//    [SerializeField] ClickableObject prevClicked;
//    [SerializeField] bool wasMouseDown;

//    public bool AllowInteraction
//    {
//        get => allowInteraction;
//        set
//        {
//            allowInteraction = value;
//            if (allowInteraction)
//            {
//                cursorInsideRenderer.color = Color.white;
//                cursorOutsideRenderer.color = Color.white;
//            }
//            else
//            {
//                cursorInsideRenderer.color = Color.grey;
//                cursorOutsideRenderer.color = Color.grey;
//            }
//        }
//    }
//    public bool AllowMovingCursor { get => allowMovingCursor; set => allowMovingCursor = value; }

//    void Start()
//    {

//    }

//    void OnEnable()
//    {
//        if (useMouseInput)
//        {
//            cursor.position = BasicUtils.GetMouseWorldPosition();
//            Cursor.visible = false;
//        }
//        else
//        {
//            cursor.position = Vector2.zero;
//        }

//        cursorInsideRenderer.gameObject.SetActive(true);
//        cursorOutsideRenderer.gameObject.SetActive(true);

//        CameraUtils.PreventLeavingBounds(cursor);
//    }

//    void OnDisable()
//    {
//        cursor.gameObject.SetActive(false);
//    }

//    void Update()
//    {
//        if (allowMovingCursor)
//        {
//            if (useMouseInput)
//            {
//                cursor.position = BasicUtils.GetMouseWorldPosition();
//                Cursor.visible = false;
//            }
//            else
//            {
//                //Vector2 movement = playerInput.LeftStickRaw;
//                //cursor.position += (Vector3)movement * Time.deltaTime * movementSpeed;
//            }

//            CameraUtils.PreventLeavingBounds(cursor);
//        }

//        if (allowInteraction)
//        {
//            RaycastHit2D hit = Physics2D.Raycast(cursor.position, Vector2.zero, 5, blockingMask);
//            var detectedCO = hit.collider;

//            if (detectedCO != null)
//            {
//                if (detectedCO.TryGetComponent(out ClickableObject co))
//                {
//                    if (co.PlayerId == playerInput.GetPlayerID() || co.PlayerId == -1)
//                    {
//                        if (co != hoveredCO)
//                        {
//                            ClickEventArgs hoverEventArgs = new ClickEventArgs()
//                            {
//                                ownerId = co == null ? -1 : co.OwnerId,
//                                playerInput = this.playerInput
//                            };
//                            co.HoverEvent?.Invoke(hoverEventArgs);

//                            ClickEventArgs unhoverEventArgs = new ClickEventArgs()
//                            {
//                                ownerId = hoveredCO == null ? -1 : hoveredCO.OwnerId,
//                                playerInput = this.playerInput
//                            };
//                            hoveredCO?.UnHoverEvent?.Invoke(unhoverEventArgs);

//                            hoveredCO = co;
//                        }
//                    }
//                }
//            }
//        }
//    }
//}