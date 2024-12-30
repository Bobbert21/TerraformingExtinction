using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NOT USED REPLACED BY STATEMACHINE
public class UprooterLevelTransitions : MonoBehaviour
{
    public bool UseStatContainer = false;
    [Header("Local variables to debug")]
    // Local variables
    public UprooterLevelStatSO LocalStats;
    public UprooterDialogueController DialogueController;
    public UprooterStateManager LocalStateManager;
    public int[] LocalFertilizerIntensity;
    public int[] LocalFertilizerLevel;
    public int LocalNutrientIntensity;
    public int LocalNutrientLevel;
    private UprooterLevelStatSO Stats
    {
        get => UseStatContainer ? (UprooterLevelStatSO)GetComponent<UprooterStateStatContainer>().Stats : LocalStats;
        set
        {
            if (UseStatContainer)
                GetComponent<CharacterStateStatContainer>().Stats = value;
            else
                LocalStats = value;
        }
    }

    private UprooterStateManager StateManager
    {
        get => UseStatContainer ? GetComponent<UprooterStateStatContainer>().StateManager : LocalStateManager;
        set
        {
            if (UseStatContainer)
                GetComponent<UprooterStateStatContainer>().StateManager = value;
            else
                LocalStateManager = value;
        }
    }

    private int[] FertilizerIntensity
    {
        get => UseStatContainer ? GetComponent<UprooterStateStatContainer>().FertilizerIntensity : LocalFertilizerIntensity;
        set
        {
            if (UseStatContainer)
                GetComponent<UprooterStateStatContainer>().FertilizerIntensity = value;
            else
                LocalFertilizerIntensity = value;
        }
    }

    private int[] FertilizerLevel
    {
        get => UseStatContainer ? GetComponent<UprooterStateStatContainer>().FertilizerLevel : LocalFertilizerLevel;
        set
        {
            if (UseStatContainer)
                GetComponent<UprooterStateStatContainer>().FertilizerLevel = value;
            else
                LocalFertilizerLevel = value;
        }
    }

    private int NutrientIntensity
    {
        get => UseStatContainer ? GetComponent<UprooterStateStatContainer>().NutrientIntensity : LocalNutrientIntensity;
        set
        {
            if (UseStatContainer)
                GetComponent<UprooterStateStatContainer>().NutrientIntensity = value;
            else
                LocalNutrientIntensity = value;
        }
    }

    private int NutrientLevel
    {
        get => UseStatContainer ? GetComponent<UprooterStateStatContainer>().NutrientLevel : LocalNutrientLevel;
        set
        {
            if (UseStatContainer)
                GetComponent<UprooterStateStatContainer>().NutrientLevel = value;
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
            //DialogueController = GetComponent<UprooterGeneralStatsContainer>().DialogueController;
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
        UprooterLevelStatSO uprooterTransition = null;
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
                uprooterTransition = (UprooterLevelStatSO)fertilizerTransition.characterTransition;
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
            Stats = (UprooterLevelStatSO)Stats.NutrientTransitions.characterTransition;
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
