using RobbysUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour
{
    int index = 0;
    [SerializeField] bool allowConsoleTimeout = false;
    [SerializeField] float consoleTimeout = 3f;
    [SerializeField] Color consoleBackgroundColor = Color.black;
    [SerializeField] Color consoleFontColor = Color.white;
    [Space]
    [SerializeField] KeyCode consolePreviousLineCode = KeyCode.UpArrow;
    [SerializeField] KeyCode consoleNextLineCode = KeyCode.DownArrow;
    [Space]
    [SerializeField] int currentLine = 0;
    [Space]
    [SerializeField] TextMesh[] textMeshes;
    [SerializeField] string[] messages = new string[0];
    [Space] 
    [SerializeField] SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] GameObject textParent;

    [SerializeField] bool isVisible = false;
    [SerializeField] float timer;

    Color baseConsoleBGColor;
    Color baseConsoleFontColor;

    void Start()
    {
        if (messages.Length == 0)
        {
            HideConsole();
        }

        timer = consoleTimeout;

        UpdateConsoleColors();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WriteLine("TEST " +  index);
        }

        if (Input.GetKeyDown(consoleNextLineCode))
        {
            if (currentLine > 0)
                currentLine -= 1;

            OnMessagesChanged();
        }

        if (Input.GetKeyDown(consolePreviousLineCode))
        {
            if (currentLine < messages.Length - 1)
                currentLine += 1;

            OnMessagesChanged();
        }

        if (allowConsoleTimeout)
        {
            if (isVisible)
            {
                if (timer <= 0)
                {
                    HideConsole();
                    timer = consoleTimeout;
                }
                else
                    timer -= Time.deltaTime;
            }
        }

        if (baseConsoleBGColor != consoleBackgroundColor)
        {
            RefreshConsoleBGColor();
        }

        if (baseConsoleFontColor != consoleFontColor)
        {
            RefreshConsoleFontColors();
        }
    }

    [ContextMenu("WriteLine")]
    public void WriteLine(string text)
    {
        string time = $"[{((DateTime.Now.Hour > 12) ? DateTime.Now.Hour - 12 : DateTime.Now.Hour)}:{DateTime.Now.Minute}:{DateTime.Now.Second}] ";
        messages = messages.AddItemAtBeginning(time + text);
        currentLine = 0;

        //for (int i = textMeshes.Length - 1; i > 0; i--)
        //{
        //    var currentMesh = textMeshes[i]; 
        //    var prevMesh = textMeshes[i - 1];

        //    currentMesh.text = prevMesh.text;
        //}

        OnMessagesChanged();

        index++;
    }

    void HideConsole()
    {
        backgroundSpriteRenderer.enabled = false;
        textParent.SetActive(false);
        
        //foreach (var mesh in textMeshes)
        //{
        //    mesh.gameObject.SetActive(false);
        //}

        isVisible = false;
    }

    void ShowConsole()
    {
        backgroundSpriteRenderer.enabled = true;
        textParent.SetActive(true);

        //foreach (var mesh in textMeshes)
        //{
        //    mesh.gameObject.SetActive(true);
        //}

        isVisible = true;
    }

    void UpdateConsoleColors()
    {
        RefreshConsoleBGColor();
        RefreshConsoleFontColors();
    }

    void RefreshConsoleFontColors()
    {
        foreach (var mesh in textMeshes)
        {
            mesh.color = consoleFontColor;
        }
        baseConsoleFontColor = consoleFontColor;
    }

    void RefreshConsoleBGColor()
    {
        backgroundSpriteRenderer.color = consoleBackgroundColor;
        baseConsoleBGColor = consoleBackgroundColor;
    }

    void OnMessagesChanged()
    {
        if (messages.Length == 0)
            return;

        if (messages.Length > 0 && !isVisible)
        {
            ShowConsole();
        }

        if (allowConsoleTimeout)
            timer = consoleTimeout;

        int index = currentLine;
        int msgCount = messages.Length;
        foreach (var mesh in textMeshes)
        {
            if (index >= msgCount)
            {
                mesh.text = "";
            }
            else
            {
                mesh.text = messages[index];
                index++;
            }
        }
    }
}
