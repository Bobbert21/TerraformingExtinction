using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum GameStates
{
    Start,
    MainMenu,
    WaveTransition,
    WaveInProgress,
    WaveEnding,
    GameOver
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameStates CurrentState;
    public Button StartWaveBtn;
    public GameObject DialogueBox;
    public Canvas Canvas;
    public int MaxNumOfUprooters;
    public int CurrentNumOfUprooters = 0;
    //Singleton
    private void Awake()
    {
        // Ensure there is only one instance of the GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        CurrentState = GameStates.Start;
    }

    public void StartWaveTransition()
    {
        if(CurrentState == GameStates.Start ||
            CurrentState == GameStates.WaveTransition)
        {
            CurrentState = GameStates.WaveInProgress;
            StartWaveBtn.interactable = false;
        }
        

    }
}
