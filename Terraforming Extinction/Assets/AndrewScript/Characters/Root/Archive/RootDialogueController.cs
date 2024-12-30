using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootDialogueController : MonoBehaviour
{
    private float Timer = 0;
    private int TimeToStartDialogue;
    public int MinTimeForDialogue = 8;
    public int MaxTimeForDialogue = 15;
    public GameObject DialogueBox;
    public Canvas DialogueCanvas;
    public bool UseStatContainer = false;


    [Header("Local variables to debug")] //GameManager and CharacterGeneralStatsContainer
    public RootLevelStatSO LocalStats;
    public RootStates LocalState = RootStates.Ready;
    public Material InactiveShade;
    private Material originalMaterial;


    private RootStates State
    {
        get => UseStatContainer ? GetComponent<RootStateStatContainer>().StateManager.GetState() : LocalState;
        set
        {
            //you can't change the state (or more specifically for the stat container)
            LocalState = value;
        }
    }

    private RootLevelStatSO Stats
    {
        get => UseStatContainer ? (RootLevelStatSO)GetComponent<RootStateStatContainer>().Stats : LocalStats;
        set
        {
            if (UseStatContainer)
                GetComponent<CharacterStateStatContainer>().Stats = value;
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

    public void CreateDialogue(DialogueLines[] ReceivedDialogue = null, RootStates state = RootStates.None)
    {
        //if given value
        if (state != RootStates.None)
        {
            State = state;
        }
        Timer = 0;
        TimeToStartDialogue = Random.Range(MinTimeForDialogue, MaxTimeForDialogue);

       
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




        /*
        DialogueLines[] UprooterDialogue = null;
        //pull the right dialogue and state
        if (State == RootStates.Ready) //&& (GameManager.Instance.CurrentState == GameStates.WaveTransition ||
                                           //GameManager.Instance.CurrentState == GameStates.Start))
        {
            UprooterDialogue = Stats.WaveTransitionDialogue;
            int randomIndex = Random.Range(0, UprooterDialogue.Length);
            dialogueBoxInstance.GetComponent<Dialogue>().Lines = UprooterDialogue[randomIndex].dialogue;
            dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
        }
    */
    }
}
