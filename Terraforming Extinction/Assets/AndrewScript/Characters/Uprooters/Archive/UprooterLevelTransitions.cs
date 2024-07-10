using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NOT USED REPLACED BY STATEMACHINE
public class UprooterLevelTransitions : MonoBehaviour
{
    public bool UseStatContainer = false;
    [Header("Local variables to debug")]
    // Local variables
    public UprooterSO LocalStats;
    public UprooterDialogueController DialogueController;
    public UprooterStateManager LocalStateManager;
    public int[] LocalFertilizerIntensity;
    public int[] LocalFertilizerLevel;
    public int LocalNutrientIntensity;
    public int LocalNutrientLevel;
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

    private UprooterStateManager StateManager
    {
        get => UseStatContainer ? GetComponent<UprooterGeneralStatsContainer>().StateManager : LocalStateManager;
        set
        {
            if (UseStatContainer)
                GetComponent<UprooterGeneralStatsContainer>().StateManager = value;
            else
                LocalStateManager = value;
        }
    }

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


    // Start is called before the first frame update
    void Start()
    {
        if (!UseStatContainer)
        {
            LocalFertilizerIntensity = new int[(int)FertilizerTypes.Count];
            LocalFertilizerLevel = new int[(int)FertilizerTypes.Count];
        }
        else
        {
            DialogueController = GetComponent<UprooterGeneralStatsContainer>().DialogueController;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //dialogueController and state if others are using this script 
    public void FertilizerUsed(FertilizerSO fertilizerUsed)
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
            //these two below can be get rid of with StateManager
            StateManager.FertilizerUsedOn(fertilizerUsed);
            DialogueController.CreateDialogue(trackFertilizerTransition.dialogue, StateManager.GetState());
            Stats = uprooterTransition;
            FertilizerLevel[usedFertilizerIndex] += 1;
            FertilizerIntensity[usedFertilizerIndex] = 0;
            Debug.Log("Transition to " + Stats.Name);
            //Get rid of below
        }
    }

   //Maybe make a general fertilizer or nutrients level up public method
    
    public void NutrientUsed(NutrientSO nurtientUsed)
    {
        NutrientIntensity += 1;
        if (NutrientIntensity >= Stats.NutrientTransitions.limit && Stats.NutrientTransitions.limit != -1)
        {
            StateManager.NutrientUsedOn(nurtientUsed);
            DialogueController.CreateDialogue(Stats.NutrientTransitions.dialogue, StateManager.GetState());
            Stats = (UprooterSO)Stats.NutrientTransitions.characterTransition;
            NutrientLevel += 1;
            NutrientIntensity = 0;
            Debug.Log("Transition to " + Stats.Name);
        }
    }

    public void SetUprooterActiveState()
    {
        //StateManager = UprooterStates.Wakingup;
        DialogueController.CreateDialogue(null, StateManager.GetState());
        //StateManager = UprooterStates.Ready;
    }
}
