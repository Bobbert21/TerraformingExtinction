using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UprooterStates
{
    None,
    Inactive,
    Wakingup,
    Ready,
    Levelingup,
    Battle,
    Damaged,
    Dead
}
public class UprooterStateManager : MonoBehaviour, ICharacterStateManager
{
    
    public UprooterInactiveState InactiveState = new UprooterInactiveState();
    public UprooterReadyState ReadyState = new UprooterReadyState();
    public UprooterLevelingUpState LevelingUpState = new UprooterLevelingUpState();
    public UprooterBaseState CurrentState;
    public UprooterBaseState PreviousState;
    public Canvas canvas;
    public Button RejoiceBtn;
    public bool UseStatContainer = false;
    [Header("Local variables to debug")]
    // Local variables
    //serialized field and private
    public UprooterLevelStatSO LocalLevelStat;
    public UprooterStates LocalState;
    //Make this into get set function
    public DialogueController DialogueController;
    public int[] LocalFertilizerIntensity;
    public int[] LocalFertilizerLevel;
    public int LocalNutrientIntensity;
    public int LocalNutrientLevel;
    private int TimeToStartDialogue;
    private float Timer;

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

    private UprooterLevelStatSO LevelStat
    {
        get => UseStatContainer ? (UprooterLevelStatSO)GetComponent<UprooterStateStatContainer>().Stats : LocalLevelStat;
        set
        {
            if (UseStatContainer)
                GetComponent<CharacterStateStatContainer>().Stats = value;
            else
                LocalLevelStat = value;
        }
    }
    void Start()
    {
        CurrentState = InactiveState;
        CurrentState.Enter(this);
        if(UseStatContainer)
        {
            DialogueController = GetComponent<UprooterStateStatContainer>().DialogueController;
        }
        else
        {
            LocalFertilizerIntensity = new int[(int)FertilizerTypes.Count];
            LocalFertilizerLevel = new int[(int)FertilizerTypes.Count];
            for (int i = 0; i < LocalFertilizerIntensity.Length; i++)
            {
                LocalFertilizerIntensity[i] = 0;
                LocalFertilizerLevel[i] = 0;
            }
        }

        TimeToStartDialogue = Random.Range(LevelStat.MinTimeForDialogue, LevelStat.MaxTimeForDialogue);


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

        Timer += Time.deltaTime;
        if (Timer > TimeToStartDialogue)
        {
            CreateDialogue();
            Timer = 0;
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
        if (NutrientIntensity >= LevelStat.NutrientTransitions.limit && LevelStat.NutrientTransitions.limit != -1)
        {
            DialogueLines[] dialogueLines = LevelStat.NutrientTransitions.dialogue;
            int randomIndex = Random.Range(0, dialogueLines.Length);
            DialogueLines dialogueLine = dialogueLines[randomIndex];
            DialogueController.CreateDialogue(dialogueLine);
            PreviousState = CurrentState;
            CurrentState = LevelingUpState;
            CurrentState.Enter(this);
            //pass through these calculations into the states (so each state can act accordingly with the items used on it)
            NutrientLevel += 1;
            NutrientIntensity = 0;
            LevelStat = (UprooterLevelStatSO)LevelStat.NutrientTransitions.characterTransition;
            Debug.Log("Transition to " + LevelStat.Name);
        }
    }

    public void FertilizerUsedOn(FertilizerSO fertilizerUsed)
    {
        Debug.Log("fertilizer used");
        UprooterLevelStatSO uprooterTransition = null;
        int usedFertilizerIndex = (int)fertilizerUsed.FertilizerType;
        Debug.Log("Which index of fertilizer: " + usedFertilizerIndex);
        // add intensity based on what fertilizer is given
        FertilizerIntensity[usedFertilizerIndex] += 1;

        // get current intensity
        int usedFertilizerIntensity = FertilizerIntensity[usedFertilizerIndex];
        Debug.Log(fertilizerUsed.FertilizerType + " used with now intensity " + usedFertilizerIntensity);

        int usedFertilizerLimit = -1;
        FertilizerTransition trackFertilizerTransition = null;

        foreach (FertilizerTransition fertilizerTransition in LevelStat.FertilizerTransitions)
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
            DialogueLines[] dialogueLines = trackFertilizerTransition.dialogue;
            int randomIndex = Random.Range(0, dialogueLines.Length);
            DialogueLines dialogueLine = dialogueLines[randomIndex];
            DialogueController.CreateDialogue(dialogueLine);
            LevelStat = uprooterTransition;
            PreviousState = CurrentState;
            CurrentState = LevelingUpState;
            CurrentState.Enter(this);
            FertilizerLevel[usedFertilizerIndex] += 1;
            FertilizerIntensity[usedFertilizerIndex] = 0;
            Debug.Log("Transition to " + LevelStat.Name);
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

    public void CreateDialogue()
    {

        DialogueLinesWithUprooterStates[] dialogueLinesWithStates = LevelStat.DialogueLinesWithStates;
        DialogueLines dialogueLine = null;

        foreach (DialogueLinesWithUprooterStates dialogueLineWithState in dialogueLinesWithStates)
        {
            if (dialogueLineWithState.state == GetState())
            {
                DialogueLines[] dialogueLines = null;
                dialogueLines = dialogueLineWithState.dialogueLines;
                int randomIndex = Random.Range(0, dialogueLines.Length);
                dialogueLine = dialogueLines[randomIndex];
                break;
            }
        }
        if(dialogueLine != null ) { DialogueController.CreateDialogue(dialogueLine); }
        
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

    public void ShowInventory()
    {
        InventoryManager.Instance.Uprooter = gameObject;
    }

    public void CreateRejoiceButton()
    {
        //make sure you don't have too much
        if(UprooterManager.Instance.MaxNumOfUprooters > UprooterManager.Instance.CurrentNumOfUprooters)
        {
            Button newRejoice = Instantiate(RejoiceBtn);

            // Set the button as a child of the canvas
            newRejoice.transform.SetParent(canvas.transform, false); // Set 'false' to preserve world position

            // Get the RectTransform component of the button
            RectTransform rejoiceRectTransform = newRejoice.GetComponent<RectTransform>();

            // Calculate the position above the target object in world space
            Vector3 targetPosition = gameObject.transform.position;
            Vector3 rejoicePosition = new Vector3(targetPosition.x, targetPosition.y + gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 2 + 0.5f, targetPosition.z);

            // Convert the world position to screen space
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(rejoicePosition);

            // Convert the screen space position to local position within the canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, null, out Vector2 canvasPos);
            rejoiceRectTransform.localPosition = canvasPos;

            newRejoice.gameObject.SetActive(true);
            newRejoice.GetComponent<Rejoice>().setObject(gameObject);
        }
        
    }

    public void PlayerDeselected()
    {
        CurrentState.PlayerDeselects(this);
    }

    public void HideInventory()
    {
        InventoryManager.Instance.ShowInventoryOptionsWithUprooters(false);
    }
}
