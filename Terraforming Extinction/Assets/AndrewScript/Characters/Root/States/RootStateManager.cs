using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum RootStates
{
    Ready,
    Levelingup,
    Damaged
}

public class RootStateManager : MonoBehaviour
{
    public RootReadyState ReadyState = new RootReadyState();
    public bool UseStatContainer = false;
    public RootBaseState CurrentState;
    public RootBaseState PreviousState;
    [Header("Local variables to debug")]
    // Local variables
    public RootSO LocalStats;
    public RootStates LocalState;
    public RootDialogueController DialogueController;
    public int[] LocalFertilizerIntensity;
    public int[] LocalFertilizerLevel;
    public int LocalNutrientIntensity;
    public int LocalNutrientLevel;

    private int[] FertilizerIntensity
    {
        get => UseStatContainer ? GetComponent<RootStatsContainer>().FertilizerIntensity : LocalFertilizerIntensity;
        set
        {
            if (UseStatContainer)
                GetComponent<RootStatsContainer>().FertilizerIntensity = value;
            else
                LocalFertilizerIntensity = value;
        }
    }

    private int[] FertilizerLevel
    {
        get => UseStatContainer ? GetComponent<RootStatsContainer>().FertilizerLevel : LocalFertilizerLevel;
        set
        {
            if (UseStatContainer)
                GetComponent<RootStatsContainer>().FertilizerLevel = value;
            else
                LocalFertilizerLevel = value;
        }
    }

    private int NutrientIntensity
    {
        get => UseStatContainer ? GetComponent<RootStatsContainer>().NutrientIntensity : LocalNutrientIntensity;
        set
        {
            if (UseStatContainer)
                GetComponent<RootStatsContainer>().NutrientIntensity = value;
            else
                LocalNutrientIntensity = value;
        }
    }

    private int NutrientLevel
    {
        get => UseStatContainer ? GetComponent<RootStatsContainer>().NutrientLevel : LocalNutrientLevel;
        set
        {
            if (UseStatContainer)
                GetComponent<RootStatsContainer>().NutrientLevel = value;
            else
                LocalNutrientLevel = value;
        }
    }

    private RootSO Stats
    {
        get => UseStatContainer ? (RootSO)GetComponent<RootStatsContainer>().Stats : LocalStats;
        set
        {
            if (UseStatContainer)
                //in uprooterstatemanager, i used character general stats below instead for some reason?
                GetComponent<RootStatsContainer>().Stats = value;
            else
                LocalStats = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        CurrentState = ReadyState;
        CurrentState.Enter(this);
        if (UseStatContainer)
        {
            DialogueController = GetComponent<RootStatsContainer>().DialogueController;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
