using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;


public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI TextComponent;
    public string[] Lines;
    public float TextSpeed;
    public float WaitText = 2;
    private int index;
    private bool HasStarted = false;
    private bool IsWaitingAfterLine = false;
    private bool IsDone = false;
    private float waitText;

    private void Update()
    {
        if(HasStarted && !IsWaitingAfterLine) 
        {
            if (TextComponent.text.Length == Lines[index].Length)
            {
                NextLine();
                
            }
        }

        //wait before destroying so can read
        if(IsDone)
        {
            if(waitText < 0)
            {
                Destroy(gameObject);
            }
            waitText -= Time.deltaTime;
        }

        //wait after each line
        if(IsWaitingAfterLine)
        {
            if(waitText < 0)
            {
                waitText = WaitText;
                IsWaitingAfterLine = false;
                TextComponent.text = string.Empty;
                StartCoroutine(TypeLine());
            }
            waitText -= Time.deltaTime;
        }
        
    }
    public void StartDialogue()
    {
        TextComponent.text = string.Empty;
        index = 0;
        HasStarted = true;
        StartCoroutine(TypeLine());
        waitText = WaitText;
    }

    IEnumerator TypeLine()
    {
        foreach (char c in Lines[index].ToCharArray())
        {
            
            TextComponent.text += c;
            yield return new WaitForSeconds(TextSpeed);
        }
    }

    void NextLine()
    {
        if (index < Lines.Length - 1)
        {
            index++;
            IsWaitingAfterLine = true;
        }
        else
        {
            IsDone = true;
            
        }
    }
}
