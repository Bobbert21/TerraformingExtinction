using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour
{
    public int PlayerId = -1;
    public int OwnerId = -1;

    public CustomClickUnityEvent ClickEvent;
    public CustomClickUnityEvent MouseDownEvent;
    public CustomClickUnityEvent MouseUpEvent;
    public CustomClickUnityEvent UnFocusedEvent;
    public CustomClickUnityEvent HoverEvent;
    public CustomClickUnityEvent UnHoverEvent;

    public GameObject ObjectToReturn;

    public GameObject GetReturnObject()
    {
        return ObjectToReturn;
    }
}

[System.Serializable]
public class GenericUnityEvent<T> : UnityEvent<T> { }

[System.Serializable] 
public class GenericStringUnityEvent : GenericUnityEvent<string> { }

[System.Serializable] 
public class GenericActionUnityEvent : GenericUnityEvent<System.Action> { }
