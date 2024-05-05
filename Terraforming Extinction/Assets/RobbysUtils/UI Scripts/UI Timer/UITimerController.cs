using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UITimerController : MonoBehaviour
{
    [Header("Timer UI Components")]
    [SerializeField] Transform timerBackground;
    [SerializeField] Transform timerForeground;

    [Header("Timer Debugging")]
    [SerializeField] bool isInitialized = false;

    [SerializeField] float baseXScale;
    [SerializeField] float baseYScale;

    [SerializeField] bool scaleX;
    [SerializeField] bool scaleY;
    [SerializeField] float maxTime;
    [SerializeField] float currentTime;
    [SerializeField] bool countDown = false;
    [SerializeField] UnityEvent OnFinish;

    public float MaxTime { get => maxTime; set => maxTime = value; }
    public float CurrentTime { get => currentTime; set => currentTime = value; }

    public bool IsCountingDown { get => countDown; }

    public Sprite BackgroundSprite { get => timerBackground.GetComponent<SpriteRenderer>().sprite; set => timerBackground.GetComponent<SpriteRenderer>().sprite = value; }
    public Sprite ForegroundSprite { get => timerForeground.GetComponent<SpriteRenderer>().sprite; set => timerForeground.GetComponent<SpriteRenderer>().sprite = value; }

    public void InitializeTimer()
    {
        if (!isInitialized)
        {
            baseXScale = timerForeground.localScale.x;
            baseYScale = timerForeground.localScale.y;

            isInitialized = true;
        }
    }

    void Update()
    {
        if (countDown)
        {
            if (currentTime <= 0)
            {
                OnFinish?.Invoke();
                countDown = false;
            }
            else
                currentTime -= Time.deltaTime;

            SetTime(currentTime);
        }
    }

    public void SetTimerStats(float maxTime, bool scaleX, bool scaleY)
    {
        this.maxTime = maxTime;
        this.currentTime = maxTime;

        this.scaleX = scaleX;
        this.scaleY = scaleY;

        SetTime(currentTime);
    }

    public void StartTimer(UnityAction finishAction = null)
    {
        countDown = true;

        if (finishAction != null)
            OnFinish.AddListener(finishAction);
    }

    public void StopTimer()
    {
        countDown = false;
    }

    public void HideTimer()
    {
        timerBackground.GetComponent<SpriteRenderer>().enabled = false;
        timerForeground.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void ShowTimer()
    {
        timerBackground.GetComponent<SpriteRenderer>().enabled = true;
        timerForeground.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public void SetTime(float newCurrentTime)
    {
        currentTime = newCurrentTime;

        float x = scaleX ? (currentTime / maxTime) * baseXScale : baseXScale;
        float y = scaleY ? (currentTime / maxTime) * baseYScale : baseYScale;

        timerForeground.localScale = new Vector3(x, y, 0);
    }
}
