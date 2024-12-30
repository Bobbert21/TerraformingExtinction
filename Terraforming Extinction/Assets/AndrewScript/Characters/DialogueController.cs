using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public GameObject DialogueBox;
    public Canvas DialogueCanvas;
    


    //[Header("Local variables to debug")] //GameManager and CharacterGeneralStatsContainer
    //public CharacterBaseSO LocalStats;
    //public UprooterStates LocalState = UprooterStates.Inactive;
    //public Material InactiveShade;
    //private Material originalMaterial;



    /*
    private CharacterBaseSO Stats
    {
        get => UseStatContainer ? GetComponent<CharacterGeneralStatsContainer>().Stats : LocalStats;
        set
        {
            if (UseStatContainer)
                GetComponent<CharacterGeneralStatsContainer>().Stats = value;
            else
                LocalStats = value;
        }
    }
    */

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDialogue(DialogueLines ReceivedDialogue)
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

        dialogueBoxInstance.GetComponent<Dialogue>().Lines = ReceivedDialogue.dialogue;
        dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
    }
}
