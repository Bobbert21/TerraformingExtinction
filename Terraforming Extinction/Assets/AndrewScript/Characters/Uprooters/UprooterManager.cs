using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public enum UprooterState
{
    Inactive,
    Wakingup,
    Ready,
    Levelingup,
    Attack
}

public class UprooterManager : MonoBehaviour
{
    public int Health;
    public int MaxHealth;
    public UprooterSO UprooterStats;
    public UprooterState State = UprooterState.Inactive;
    
    //use enum to figure out which fertilizer it is keeping track of. like red is 0 so index 0 in the array
    private int[] FertilizerIntensity;
    public int[] FertilizerLevel;

    private int NutrientIntensity;
    public int NutrientLevel;

    public int MinTimeForDialogue = 8;
    public int MaxTimeForDialogue = 15;
    private float Timer;
    private int TimeToStartDialogue;

    private void Start()
    {
        FertilizerIntensity= new int[(int)FertilizerTypes.Count];
        FertilizerLevel = new int[(int)FertilizerTypes.Count];
        Timer = 0f;
        TimeToStartDialogue = Random.Range(MinTimeForDialogue, MaxTimeForDialogue);
    }

    //when item is used on the uprooter
    public void ItemUsed(ItemSO itemUsed)
    {
        //using fertilizer as item
        if(itemUsed is FertilizerSO fertilizerUsed)
        {
            UprooterSO uprooterTransition = null;
            int usedFertilizerIndex = (int)fertilizerUsed.FertilizerType;
            //add intensity based on what fertilizer is given
            FertilizerIntensity[usedFertilizerIndex] += 1;
            //remove the fertilizer from inventory
            InventoryManager.Instance.RemoveItem(itemUsed);

            //get current intensity
            int usedFertilizerIntensity = FertilizerIntensity[usedFertilizerIndex];
            Debug.Log(fertilizerUsed.FertilizerType + " used with now intensity " + usedFertilizerIntensity);
            //var to keep track of limit of the fertilizer
            int usedFertilizerLimit = -1;
            FertilizerTransition trackFertilizerTransition= null;
            //in the uprooterSO, find the limit of this fertilizer 
            foreach (FertilizerTransition fertilizerTransition in UprooterStats.FertilizerTransitions)
            {
                //if the index (fertilizer enum) = enum of fertilizerlimit in uprooters
                if(usedFertilizerIndex == (int)fertilizerTransition.type)
                {
                    usedFertilizerLimit = fertilizerTransition.limit;
                    uprooterTransition = (UprooterSO)fertilizerTransition.characterTransition;
                    //for dialogue when changing to another color
                    trackFertilizerTransition = fertilizerTransition;
                    break;
                }
            }
            //If the fertilizer increases beyond the limit. -1 means there are no more limits
            if(usedFertilizerIntensity >= usedFertilizerLimit && usedFertilizerLimit != -1)
            {
                State = UprooterState.Levelingup;
                CreateDialogue(trackFertilizerTransition.dialogue);
                UprooterStats = uprooterTransition;
                FertilizerLevel[usedFertilizerIndex] += 1;
                //restart intensity for next limit of the uprooterSO
                FertilizerIntensity[usedFertilizerIndex] = 0;
                Debug.Log("Transition to " + UprooterStats.Name);
            }
        }else if(itemUsed is NutrientSO nutrientUsed)
        {
            NutrientIntensity += 1;
            //remove the fertilizer from inventory
            InventoryManager.Instance.RemoveItem(itemUsed);
            if (NutrientIntensity >= UprooterStats.NutrientTransitions.limit && UprooterStats.NutrientTransitions.limit != -1)
            {
                State = UprooterState.Levelingup;
                CreateDialogue(UprooterStats.NutrientTransitions.dialogue);
                UprooterStats = (UprooterSO)UprooterStats.NutrientTransitions.characterTransition;
                NutrientLevel += 1;
                NutrientIntensity = 0;
                Debug.Log("Transition to " + UprooterStats.Name);
            }
        }
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        if(Timer > TimeToStartDialogue)
        {
            CreateDialogue();
        }
    }

    public void CreateDialogue(DialogueCreation[] ReceivedDialogue = null)
    {
        Timer = 0;
        TimeToStartDialogue = Random.Range(MinTimeForDialogue, MaxTimeForDialogue);
        // Instantiate the dialogue box prefab
        GameObject dialogueBoxInstance = Instantiate(GameManager.Instance.DialogueBox);
        // Set the dialogue box instance as a child of the canvas
        dialogueBoxInstance.transform.SetParent(GameManager.Instance.Canvas.transform, false); // Set 'false' to preserve world position

        //position of box is above
        dialogueBoxInstance.transform.position = (Vector2)transform.position + (Vector2.up * (gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 2)) 
            + Vector2.up* dialogueBoxInstance.GetComponent<RectTransform>().rect.height/2 +
            Vector2.right * dialogueBoxInstance.GetComponent<RectTransform>().rect.width/3; // Adjust the offset as needed


        DialogueCreation[] UprooterDialogue = null;
        //pull the righth dialogue
        if (State == UprooterState.Wakingup)
        {
            UprooterDialogue = UprooterStats.WakingUpDialogue;
            int randomIndex = Random.Range(0,UprooterDialogue.Length);
            dialogueBoxInstance.GetComponent<Dialogue>().Lines = UprooterDialogue[randomIndex].dialogue;
            dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
            State = UprooterState.Ready;
        }else if(State == UprooterState.Levelingup)
        {
            UprooterDialogue = ReceivedDialogue;
            int randomIndex = Random.Range(0, UprooterDialogue.Length);
            dialogueBoxInstance.GetComponent<Dialogue>().Lines = UprooterDialogue[randomIndex].dialogue;
            dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
            if (GameManager.Instance.CurrentState == GameStates.WaveInProgress)
            {
                State = UprooterState.Attack;
            }
            else
            {
                State = UprooterState.Ready;
            }
        }
    }

}
