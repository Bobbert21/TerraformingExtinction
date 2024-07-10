using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UprooterDialogueController : MonoBehaviour
{
    private float Timer = 0;
    private int TimeToStartDialogue;
    public int MinTimeForDialogue = 8;
    public int MaxTimeForDialogue = 15;
    public GameObject DialogueBox;
    public Canvas DialogueCanvas;
    public bool UseStatContainer = false;


    [Header("Local variables to debug")] //GameManager and CharacterGeneralStatsContainer
    public UprooterSO LocalStats;
    public UprooterStates LocalState = UprooterStates.Inactive;
    public Material InactiveShade;
    private Material originalMaterial;


    private UprooterStates State
    {
        get => UseStatContainer ? GetComponent<UprooterGeneralStatsContainer>().StateManager.GetState() : LocalState;
        set
        {
            //you can't change the state (or more specifically for the stat container)
            LocalState = value;
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


    // Start is called before the first frame update
    void Start()
    {
        if (UseStatContainer)
        {
            Timer = 0f;
            TimeToStartDialogue = Random.Range(MinTimeForDialogue, MaxTimeForDialogue);
            originalMaterial = GetComponent<SpriteRenderer>().material;
            GetComponent<SpriteRenderer>().material = InactiveShade;
            DialogueBox = GameManager.Instance.DialogueBox;
            DialogueCanvas = GameManager.Instance.Canvas;
        }
        else
        {
            originalMaterial = GetComponent<SpriteRenderer>().material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > TimeToStartDialogue)
        {
            CreateDialogue();
        }
    }

    public void CreateDialogue(DialogueCreation[] ReceivedDialogue = null, UprooterStates state = UprooterStates.None)
    {
        //if given value
        if(state != UprooterStates.None)
        {
            State = state;
        }
        Timer = 0;
        TimeToStartDialogue = Random.Range(MinTimeForDialogue, MaxTimeForDialogue);

        if (State != UprooterStates.Inactive)
        {
            // Instantiate the dialogue box prefab
            GameObject dialogueBoxInstance = Instantiate(DialogueBox);
            // Set the dialogue box instance as a child of the canvas
            dialogueBoxInstance.transform.SetParent(DialogueCanvas.transform, false); // Set 'false' to preserve world position


            // Get the RectTransform component of the dialogue box
            RectTransform dialogueRectTransform = dialogueBoxInstance.GetComponent<RectTransform>();

            // Calculate the position above the character
            Vector3 characterPosition = transform.position;
            
            // Set the position above the character
            Vector3 dialoguePosition = new Vector3(characterPosition.x + 0.5f, characterPosition.y + 1.7f, characterPosition.z);
   
            // Convert the world position to canvas space
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(dialoguePosition);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(DialogueCanvas.GetComponent<RectTransform>(), screenPoint, null, out Vector2 canvasPos);
            dialogueRectTransform.localPosition = canvasPos;



            DialogueCreation[] UprooterDialogue = null;

            //pull the right dialogue and state
            if (State == UprooterStates.Ready) //&& (GameManager.Instance.CurrentState == GameStates.WaveTransition ||
                //GameManager.Instance.CurrentState == GameStates.Start))
            {
                UprooterDialogue = Stats.WaveTransitionDialogue;
                int randomIndex = Random.Range(0, UprooterDialogue.Length);
                dialogueBoxInstance.GetComponent<Dialogue>().Lines = UprooterDialogue[randomIndex].dialogue;
                dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
            }
            else if (State == UprooterStates.Wakingup)
            {
                GetComponent<SpriteRenderer>().material = originalMaterial;
                UprooterDialogue = Stats.WakingUpDialogue;
                int randomIndex = Random.Range(0, UprooterDialogue.Length);
                dialogueBoxInstance.GetComponent<Dialogue>().Lines = UprooterDialogue[randomIndex].dialogue;
                dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
            }
            else if (State == UprooterStates.Levelingup)
            {
                UprooterDialogue = ReceivedDialogue;
                int randomIndex = Random.Range(0, UprooterDialogue.Length);
                dialogueBoxInstance.GetComponent<Dialogue>().Lines = UprooterDialogue[randomIndex].dialogue;
                dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
                
            }
            else if (State == UprooterStates.Damaged)
            {
                UprooterDialogue = Stats.DamagedDialogue;
                int randomIndex = Random.Range(0, UprooterDialogue.Length);
                dialogueBoxInstance.GetComponent<Dialogue>().Lines = UprooterDialogue[randomIndex].dialogue;
                dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
            }
            else if (State == UprooterStates.Battle)// &&
                //GameManager.Instance.CurrentState == GameStates.WaveInProgress)
            {
                UprooterDialogue = Stats.WaveInProgressDialogue;
                int randomIndex = Random.Range(0, UprooterDialogue.Length);
                dialogueBoxInstance.GetComponent<Dialogue>().Lines = UprooterDialogue[randomIndex].dialogue;
                dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
            }
        }
    }
}
