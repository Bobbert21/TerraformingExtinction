using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DepositSO;



public class RootMiscController : MonoBehaviour
{
    public RootLevelStatSO RootStats;
    public int NutrientLevel;
    private int NutrientIntensity;
    public RootStates State;
    public List<ItemSO> CommonItems = new();
    public List<ItemSO> UncommonItems = new();
    public List<ItemSO> RareItems = new();
    public List<ItemSO> AllowedInputItems = new();
    private int CommonCutoff = 100;
    private int UncommonCutoff;
    private int RareCutoff;

    public int MinTimeForDialogue = 8;
    public int MaxTimeForDialogue = 15;
    private float Timer = 0;
    private int TimeToStartDialogue;
    public void RootConversion(ItemSO itemUsed)
    {
        //check if item is depositSO and then use it
        if (itemUsed is DepositSO depositUsed)
        {
            //check if the deposit is in the allowed items
            if (AllowedInputItems.Contains(itemUsed))
            {
                //Remove from inventory
                InventoryManager.Instance.RemoveItem(itemUsed);
                int randomNumber = UnityEngine.Random.Range(0, 100);
                if (depositUsed.DepositRarity == DepositRarities.Random)
                {
                    UncommonCutoff = 40;
                    RareCutoff = 10;
                }
                else if (depositUsed.DepositRarity == DepositRarities.Common)
                {
                    UncommonCutoff = 20;
                    RareCutoff = 5;
                }
                else if (depositUsed.DepositRarity == DepositRarities.Uncommon)
                {
                    UncommonCutoff = 55;
                    RareCutoff = 5;
                }
                else if (depositUsed.DepositRarity == DepositRarities.Rare)
                {
                    UncommonCutoff = 60;
                    RareCutoff = 40;
                }

                List<ItemSO> ChosenRarityItems = new();

                if (randomNumber <= RareCutoff)
                {
                    ChosenRarityItems = RareItems;
                    Debug.Log("Rare");
                }
                else if (randomNumber <= UncommonCutoff)
                {
                    ChosenRarityItems = UncommonItems;
                    Debug.Log("Uncommon");
                }
                else
                {
                    ChosenRarityItems = CommonItems;
                    Debug.Log("Common");
                }

                // Generate a random index within the range of valid indices for the list
                int randomItemIndex = UnityEngine.Random.Range(0, ChosenRarityItems.Count);
                //Add to inventory of player
                InventoryManager.Instance.AddItem(ChosenRarityItems[randomItemIndex]);
                InventoryManager.Instance.DisplayItems();
            }

        }
        else if (itemUsed is NutrientSO nutrientUsed)
        {
            Debug.Log("Nutrient used on root");
            NutrientIntensity += 1;
            //remove the fertilizer from inventory
            InventoryManager.Instance.RemoveItem(itemUsed);
            if (NutrientIntensity >= RootStats.NutrientTransitions.limit && RootStats.NutrientTransitions.limit != -1)
            {
                State = RootStates.Levelingup;
                CreateDialogue(RootStats.NutrientTransitions.dialogue);
                RootStats = (RootLevelStatSO)RootStats.NutrientTransitions.characterTransition;
                NutrientLevel += 1;
                NutrientIntensity = 0;
                GameManager.Instance.MaxNumOfUprooters = RootStats.NumOfUprooters;
                Debug.Log("Transition to " + RootStats.Name);
            }
        }
    }

    public void CreateDialogue(DialogueLines[] ReceivedDialogue = null)
    {
        Timer = 0;
        TimeToStartDialogue = Random.Range(MinTimeForDialogue, MaxTimeForDialogue);
        // Instantiate the dialogue box prefab
        GameObject dialogueBoxInstance = Instantiate(GameManager.Instance.DialogueBox);
        // Set the dialogue box instance as a child of the canvas
        dialogueBoxInstance.transform.SetParent(GameManager.Instance.Canvas.transform, false); // Set 'false' to preserve world position

        //position of box is above
        dialogueBoxInstance.transform.position = (Vector2)transform.position + (Vector2.up * (gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 2))
            + Vector2.up * dialogueBoxInstance.GetComponent<RectTransform>().rect.height / 2 +
            Vector2.right * dialogueBoxInstance.GetComponent<RectTransform>().rect.width / 3; // Adjust the offset as needed

        /*
        DialogueLines[] RootDialogue = null;
        if(State == RootStates.Ready && (GameManager.Instance.CurrentState == GameStates.WaveTransition ||
            GameManager.Instance.CurrentState == GameStates.Start))
        {
            RootDialogue = RootStats.WaveTransitionDialogue;
            int randomIndex = Random.Range(0, RootDialogue.Length);
            dialogueBoxInstance.GetComponent<Dialogue>().Lines = RootDialogue[randomIndex].dialogue;
            dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
        }
        else if (State == RootStates.Levelingup)
        {
            RootDialogue = ReceivedDialogue;
            int randomIndex = Random.Range(0, RootDialogue.Length);
            dialogueBoxInstance.GetComponent<Dialogue>().Lines = RootDialogue[randomIndex].dialogue;
            dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
            State = RootStates.Ready;
        }
        else if (State == RootStates.Damaged)
        {
            RootDialogue = RootStats.DamagedDialogue;
            int randomIndex = Random.Range(0, RootDialogue.Length);
            dialogueBoxInstance.GetComponent<Dialogue>().Lines = RootDialogue[randomIndex].dialogue;
            dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();

        //standard wave progression
        }else if (State != RootStates.Damaged &&
            GameManager.Instance.CurrentState == GameStates.WaveInProgress)
        {
            RootDialogue = RootStats.WaveInProgressDialogue;
            int randomIndex = Random.Range(0, RootDialogue.Length);
            dialogueBoxInstance.GetComponent<Dialogue>().Lines = RootDialogue[randomIndex].dialogue;
            dialogueBoxInstance.GetComponent<Dialogue>().StartDialogue();
        }
        */
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > TimeToStartDialogue)
        {
            CreateDialogue();
        }
    }

    private void Start()
    {
        GameManager.Instance.MaxNumOfUprooters = RootStats.NumOfUprooters;
        Timer = 0f;
        TimeToStartDialogue = Random.Range(MinTimeForDialogue, MaxTimeForDialogue);
    }
}
