using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomUnityEvent : UnityEvent<int, int>
{

}

[System.Serializable]
public class CustomClickUnityEvent : UnityEvent<ClickEventArgs>
{

}

[System.Serializable]
public class ClickEventArgs : System.EventArgs
{
    public int ownerId;
    //public PlayerInput playerInput;
}