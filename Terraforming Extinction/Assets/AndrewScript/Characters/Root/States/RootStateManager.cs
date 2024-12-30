using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


[System.Serializable]
public enum RootStates
{
    None,
    Ready,
    Levelingup,
    Damaged
}

public class RootStateManager : MonoBehaviour, ICharacterStateManager
{
    public RootReadyState ReadyState = new RootReadyState();
    public RootLevelingupState LevelingUpState= new RootLevelingupState();
    public RootDamagedState DamagedState = new RootDamagedState();
    public bool UseStatContainer = false;
    public RootBaseState CurrentState;
    public RootBaseState PreviousState;
    [Header("Local variables to debug")]
    // Local variables
    public RootLevelStatSO LocalLevelStats;
    public RootStates LocalState;
    public DialogueController DialogueController;
    public int[] LocalFertilizerIntensity;
    public int[] LocalFertilizerLevel;
    public int LocalNutrientIntensity;
    public int LocalNutrientLevel;

    private int TimeToStartDialogue;
    private float Timer;
    private int[] FertilizerIntensity
    {
        get => UseStatContainer ? GetComponent<RootStateStatContainer>().FertilizerIntensity : LocalFertilizerIntensity;
        set
        {
            if (UseStatContainer)
                GetComponent<RootStateStatContainer>().FertilizerIntensity = value;
            else
                LocalFertilizerIntensity = value;
        }
    }

    private int[] FertilizerLevel
    {
        get => UseStatContainer ? GetComponent<RootStateStatContainer>().FertilizerLevel : LocalFertilizerLevel;
        set
        {
            if (UseStatContainer)
                GetComponent<RootStateStatContainer>().FertilizerLevel = value;
            else
                LocalFertilizerLevel = value;
        }
    }

    private int NutrientIntensity
    {
        get => UseStatContainer ? GetComponent<RootStateStatContainer>().NutrientIntensity : LocalNutrientIntensity;
        set
        {
            if (UseStatContainer)
                GetComponent<RootStateStatContainer>().NutrientIntensity = value;
            else
                LocalNutrientIntensity = value;
        }
    }

    private int NutrientLevel
    {
        get => UseStatContainer ? GetComponent<RootStateStatContainer>().NutrientLevel : LocalNutrientLevel;
        set
        {
            if (UseStatContainer)
                GetComponent<RootStateStatContainer>().NutrientLevel = value;
            else
                LocalNutrientLevel = value;
        }
    }

    private RootLevelStatSO LevelStats
    {
        get => UseStatContainer ? (RootLevelStatSO)GetComponent<RootStateStatContainer>().Stats : LocalLevelStats;
        set
        {
            if (UseStatContainer)
                //in uprooterstatemanager, i used character general stats below instead for some reason?
                GetComponent<RootStateStatContainer>().Stats = value;
            else
                LocalLevelStats = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        CurrentState = ReadyState;
        CurrentState.Enter(this);
        if (UseStatContainer)
        {
            DialogueController = GetComponent<RootStateStatContainer>().DialogueController;
        }

        TimeToStartDialogue = Random.Range(LevelStats.MinTimeForDialogue, LevelStats.MaxTimeForDialogue);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState.UpdateState(this);
        Debug.Log(GetState());
        //Need to add localstates to choose from so can debug
        if (!UseStatContainer)
        {
            if (LocalState != GetState())
            {
                if (LocalState == RootStates.Ready)
                {
                    PreviousState = CurrentState;
                    CurrentState = ReadyState;
                }
            }
        }

        Timer += Time.deltaTime;
        if (Timer > TimeToStartDialogue)
        {
            CreateDialogue();
            Timer = 0;
        }
    }

    public RootStates GetState()
    {
        return CurrentState.GetState();
    }

    public void SwitchState(RootBaseState state)
    {
        CurrentState = state;
        CurrentState.Enter(this);
    }

    //have to change so nutrients or other items used on it will affect differently based on states
    public void NutrientUsedOn(NutrientSO nurtientUsed)
    {

        NutrientIntensity += 1;
        if (NutrientIntensity >= LevelStats.NutrientTransitions.limit && LevelStats.NutrientTransitions.limit != -1)
        {
            DialogueLines[] dialogueLines = LevelStats.NutrientTransitions.dialogue;
            int randomIndex = Random.Range(0, dialogueLines.Length);
            DialogueLines dialogueLine = dialogueLines[randomIndex];
            DialogueController.CreateDialogue(dialogueLine);
            PreviousState = CurrentState;
            CurrentState = LevelingUpState;
            CurrentState.Enter(this);
            //pass through these calculations into the states (so each state can act accordingly with the items used on it)
            NutrientLevel += 1;
            NutrientIntensity = 0;
            LevelStats = (RootLevelStatSO)LevelStats.NutrientTransitions.characterTransition;
            Debug.Log("Transition to " + LevelStats.Name);
        }
    }

    public void CreateDialogue()
    { 

        DialogueLinesWithRootStates[] dialogueLinesWithStates = LevelStats.DialogueLinesWithStates;
        DialogueLines dialogueLine = null;

        foreach(DialogueLinesWithRootStates dialogueLineWithState in dialogueLinesWithStates)
        {
            if(dialogueLineWithState.state == GetState())
            {
                DialogueLines[] dialogueLines = null;
                dialogueLines = dialogueLineWithState.dialogueLines;
                int randomIndex = Random.Range(0, dialogueLines.Length);
                dialogueLine = dialogueLines[randomIndex];
                break;
            }
        }

        if (dialogueLine != null) { DialogueController.CreateDialogue(dialogueLine); }
    }

    public void ShowInventory()
    {
        InventoryManager.Instance.Root = gameObject;
    }

    public GameObject PlayerSelected()
    {
        //current states does actions and this returns a gameobject if select is successful
        //will return this gameObject to the PlayerSelects script so it can save and keep track of what is currently selected
        if (CurrentState.PlayerSelects(this))
        {

            return gameObject;
        }
        else
        {
            return null;
        }

    }

    public void PlayerDeselected()
    {
        CurrentState.PlayerDeselects(this);
    }

    public void HideInventory()
    {
        InventoryManager.Instance.ShowInventoryOptionsWithRoot(false);
    }
}
