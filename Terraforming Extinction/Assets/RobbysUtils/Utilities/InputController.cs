//using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [Header("Action IDs")]
    [SerializeField] protected int leftStickXID;
    [SerializeField] protected int leftStickYID;

    [SerializeField] protected int rightStickXID;
    [SerializeField] protected int rightStickYID;

    [SerializeField] protected int leftTriggerButtonID;
    [SerializeField] protected int rightTriggerButtonID;

    [SerializeField] protected int leftShoulderButtonID;
    [SerializeField] protected int rightShoulderButtonID;

    [SerializeField] protected int bottomButtonID;
    [SerializeField] protected int leftButtonID;
    [SerializeField] protected int rightButtonID;
    [SerializeField] protected int topButtonID;

    [SerializeField] protected int startButtonID;
    [SerializeField] protected int selectButtonID;

    [SerializeField] protected int dpadXID;
    [SerializeField] protected int dpadYID;

    [Header("Debugging")]
    [SerializeField] protected Vector2 mousePosition;
    [SerializeField] protected Vector2 mousePositionDelta;

    [SerializeField] protected Vector2 leftStick;
    [SerializeField] protected Vector2 leftStickRaw;

    [SerializeField] protected bool leftTriggerButton;
    [SerializeField] protected bool rightTriggerButton;

    [SerializeField] protected bool leftShoulderButton;
    [SerializeField] protected bool leftShoulderButtonDown;
    [SerializeField] protected bool leftShoulderButtonUp;

    [SerializeField] protected bool rightShoulderButton;
    [SerializeField] protected bool rightShoulderButtonDown;
    [SerializeField] protected bool rightShoulderButtonUp;

    [SerializeField] protected bool bottomButton;
    [SerializeField] protected bool leftButton;
    [SerializeField] protected bool rightButton;
    [SerializeField] protected bool topButton;

    [SerializeField] protected bool startButton;
    [SerializeField] protected bool selectButton;

    [Header("Last Controller")]
    [SerializeField] protected bool hasMouseMoved;
    [SerializeField] protected bool isKeyboardOrMouse;
    [SerializeField] protected bool isController;
    //[SerializeField] protected ControllerType lastControllerType = ControllerType.Mouse;

    public virtual bool HasMouseMoved { get; }
    public virtual bool IsKeyboardAndMouse { get; }
    public virtual bool IsController { get; }

    //public virtual ControllerType LastController { get; }
    public virtual Vector2 MousePosition { get; }

    public virtual Vector2 LeftStick { get; }
    public virtual Vector2 LeftStickRaw { get; }
    
    public virtual Vector2 RightStick { get; }
    public virtual Vector2 RightStickRaw { get; }

    public virtual bool SelectButton { get; }
    public virtual bool SelectButtonDown { get; }
    public virtual bool SelectButtonUp { get; }
    public virtual bool StartButton { get; }
    public virtual bool StartButtonDown { get; }
    public virtual bool StartButtonUp { get; }

    public virtual bool LeftShoulderButton { get; }
    public virtual bool LeftShoulderButtonDown { get; }
    public virtual bool LeftShoulderButtonUp { get; }

    public virtual bool LeftTriggerButton { get; }
    public virtual bool LeftTriggerButtonDown { get; }
    public virtual bool LeftTriggerButtonUp{ get; }

    public virtual bool RightShoulderButton { get; }
    public virtual bool RightShoulderButtonDown { get; }
    public virtual bool RightShoulderButtonUp { get; }

    public virtual bool RightTriggerButton { get; }
    public virtual bool RightTriggerButtonDown { get; }
    public virtual bool RightTriggerButtonUp { get; }

    public virtual bool BottomButton { get; }
    public virtual bool BottomButtonDown { get; }
    public virtual bool BottomButtonUp { get; }
    public virtual bool BottomButtonShortPress { get; }

    public virtual bool LeftButton { get; }
    public virtual bool LeftButtonDown { get; }
    public virtual bool LeftButtonUp { get; }

    public virtual bool TopButton { get; }
    public virtual bool TopButtonDown { get; }
    public virtual bool TopButtonUp { get; }

    public virtual bool RightButton { get; }
    public virtual bool RightButtonDown { get; }
    public virtual bool RightButtonUp { get; }

    public virtual Vector2 DPadX 
    {
        get;
    }

    public virtual Vector2 DPadY 
    { 
        get; 
    }

    //Player player;

    [Header("Player Settings")]
    [SerializeField] protected int playerId;
    [SerializeField] protected Vector2 deadzone = new Vector2(0.1f, 0.1f);

    //public Player Player
    //{
    //    get
    //    {
    //        if (player == null)
    //            player = ReInput.players.GetPlayer(playerId);

    //        return player;
    //    }

    //    set => player = value;
    //}

    public virtual void Awake() { }

    public virtual void Update()
    {
        var hasMouseMoved = HasMouseMoved;

        var leftStick = LeftStick;
        var leftStickRaw = LeftStickRaw;

        var rightStick = RightStick;
        var rightStickRaw = RightStickRaw;

        var leftShoulderButton = LeftShoulderButton;
        var rightShoulderButton = RightShoulderButton;

        var bottomButton = BottomButton;
        var leftButton = LeftButton;
        var rightButton = RightButton;
        var topButton = TopButton;
        var topButtonDown = TopButtonDown;

        var startButton = StartButton;
        var selectButton = SelectButton;
    }

    //public virtual void SetPlayerID(int ID) 
    //{
    //    playerId = ID;
    //    Player = ReInput.players.GetPlayer(ID);
    //}

    //public virtual int GetPlayerID() 
    //{ 
    //    return playerId; 
    //}

    //public virtual bool GetAnyButtonDown() 
    //{ 
    //    return Player.GetAnyButtonDown(); 
    //}
}
