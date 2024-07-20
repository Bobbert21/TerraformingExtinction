using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UprooterStateManager : MonoBehaviour
{
    
    public UprooterInactiveState InactiveState = new UprooterInactiveState();
    public UprooterReadyState ReadyState = new UprooterReadyState();
    public UprooterLevelingUpState LevelingUpState = new UprooterLevelingUpState();
    public UprooterBaseState CurrentState;
    public UprooterBaseState PreviousState;
    public bool UseStatContainer = false;
    [Header("Local variables to debug")]
    // Local variables
    public UprooterSO LocalStats;
    public UprooterStates LocalState;
    public UprooterDialogueController DialogueController;
    public int[] LocalFertilizerIntensity;
    public int[] LocalFertilizerLevel;
    public int LocalNutrientIntensity;
    public int LocalNutrientLevel;



    private int[] FertilizerIntensity
    {
        get => UseStatContainer ? GetComponent<UprooterGeneralStatsContainer>().FertilizerIntensity : LocalFertilizerIntensity;
        set
        {
            if (UseStatContainer)
                GetComponent<UprooterGeneralStatsContainer>().FertilizerIntensity = value;
            else
                LocalFertilizerIntensity = value;
        }
    }

    private int[] FertilizerLevel
    {
        get => UseStatContainer ? GetComponent<UprooterGeneralStatsContainer>().FertilizerLevel : LocalFertilizerLevel;
        set
        {
            if (UseStatContainer)
                GetComponent<UprooterGeneralStatsContainer>().FertilizerLevel = value;
            else
                LocalFertilizerLevel = value;
        }
    }

    private int NutrientIntensity
    {
        get => UseStatContainer ? GetComponent<UprooterGeneralStatsContainer>().NutrientIntensity : LocalNutrientIntensity;
        set
        {
            if (UseStatContainer)
                GetComponent<UprooterGeneralStatsContainer>().NutrientIntensity = value;
            else
                LocalNutrientIntensity = value;
        }
    }

    private int NutrientLevel
    {
        get => UseStatContainer ? GetComponent<UprooterGeneralStatsContainer>().NutrientLevel : LocalNutrientLevel;
        set
        {
            if (UseStatContainer)
                GetComponent<UprooterGeneralStatsContainer>().NutrientLevel = value;
            else
                LocalNutrientLevel = value;
        }
    }

    private UprooterSO Stats
    {
        get => UseStatContainer ? (UprooterSO)GetComponent<UprooterGeneralStatsContainer>().Stats : LocalStats;
        set
        {
            if (UseStatContainer)
                GetComponent<CharacterGeneralStatsContainer>().Stats = value;
            else
                LocalStats = value;
        }
    }
    void Start()
    {
        CurrentState = InactiveState;
        CurrentState.Enter(this);
        if(UseStatContainer)
        {
            DialogueController = GetComponent<UprooterGeneralStatsContainer>().DialogueController;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState.UpdateState(this);
        Debug.Log(GetState());
        //Need to add localstates to choose from so can debug
        if (!UseStatContainer)
        {
            if(LocalState != GetState())
            {
                if(LocalState == UprooterStates.Ready)
                {
                    PreviousState = CurrentState;
                    CurrentState = ReadyState;
                }
            }
        }
    }

    public UprooterStates GetState()
    {
        return CurrentState.GetState();
    }

    public void SwitchState(UprooterBaseState state)
    {
        CurrentState = state;
        CurrentState.Enter(this);
    }

    //have to change so nutrients or other items used on it will affect differently based on states
    public void NutrientUsedOn(NutrientSO nurtientUsed)
    {
        
        NutrientIntensity += 1;
        if (NutrientIntensity >= Stats.NutrientTransitions.limit && Stats.NutrientTransitions.limit != -1)
        {
            //could move dialoguecontroller inside state but would have to pass through the dialoguecontroller stats and intensity stats
            //actually should try to move it in state so i can control what kind of dialogue if any goes to it
            DialogueController.CreateDialogue(Stats.NutrientTransitions.dialogue, UprooterStates.Levelingup);
            PreviousState = CurrentState;
            CurrentState = LevelingUpState;
            CurrentState.Enter(this);
            //pass through these calculations into the states (so each state can act accordingly with the items used on it)
            NutrientLevel += 1;
            NutrientIntensity = 0;
            Stats = (UprooterSO)Stats.NutrientTransitions.characterTransition;
            Debug.Log("Transition to " + Stats.Name);
        }
    }

    public void FertilizerUsedOn(FertilizerSO fertilizerUsed)
    {
        Debug.Log("fertilizer used");
        UprooterSO uprooterTransition = null;
        int usedFertilizerIndex = (int)fertilizerUsed.FertilizerType;
        // add intensity based on what fertilizer is given
        FertilizerIntensity[usedFertilizerIndex] += 1;

        // get current intensity
        int usedFertilizerIntensity = FertilizerIntensity[usedFertilizerIndex];
        Debug.Log(fertilizerUsed.FertilizerType + " used with now intensity " + usedFertilizerIntensity);

        int usedFertilizerLimit = -1;
        FertilizerTransition trackFertilizerTransition = null;

        foreach (FertilizerTransition fertilizerTransition in Stats.FertilizerTransitions)
        {
            if (usedFertilizerIndex == (int)fertilizerTransition.type)
            {
                usedFertilizerLimit = fertilizerTransition.limit;
                uprooterTransition = (UprooterSO)fertilizerTransition.characterTransition;
                trackFertilizerTransition = fertilizerTransition;
                break;
            }
        }

        if (usedFertilizerIntensity >= usedFertilizerLimit && usedFertilizerLimit != -1)
        {
            DialogueController.CreateDialogue(trackFertilizerTransition.dialogue, UprooterStates.Levelingup);
            Stats = uprooterTransition;
            PreviousState = CurrentState;
            CurrentState = LevelingUpState;
            CurrentState.Enter(this);
            FertilizerLevel[usedFertilizerIndex] += 1;
            FertilizerIntensity[usedFertilizerIndex] = 0;
            Debug.Log("Transition to " + Stats.Name);
        }
    }

    public void ActivateUprooter()
    {
        if(GetState() == UprooterStates.Inactive)
        {
            PreviousState= CurrentState;
            CurrentState = ReadyState;
            CurrentState.Enter(this);
        }
        else
        {
            Debug.Log("Cannot activate. Already activated");
        }
    }
}
