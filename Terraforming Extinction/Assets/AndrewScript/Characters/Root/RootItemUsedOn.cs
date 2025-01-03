using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class RootItemUsedOn : MonoBehaviour
{
    public List<ItemSO> CommonItems = new();
    public List<ItemSO> UncommonItems = new();
    public List<ItemSO> RareItems = new();
    public List<ItemSO> AllowedInputItems = new();
    private int CommonCutoff = 100;
    private int UncommonCutoff;
    private int RareCutoff;
    public bool UseStatContainer = false;
    [Header("Local Variables")]
    [SerializeField]
    private RootStateManager LocalStateManager;
    [SerializeField]
    private RootLevelStatSO LocalRootStats;
    [SerializeField]
    private int LocalNutrientLevel;
    [SerializeField]
    private int LocalNutrientIntensity;

    private RootStateManager StateManager
    {
        get => UseStatContainer ? GetComponent<RootStateStatContainer>().StateManager : LocalStateManager;
    }

    private RootLevelStatSO RootStats
    {
        get => UseStatContainer ? (RootLevelStatSO)GetComponent<CharacterStateStatContainer>().Stats : LocalRootStats;
        set
        {
            if (UseStatContainer)
            {
                GetComponent<RootStateStatContainer>().Stats = value;
            }
            else
            {
                LocalRootStats = value;
            }
        }
    }

    private int NutrientLevel
    {
        get => UseStatContainer ? GetComponent<RootStateStatContainer>().NutrientLevel : LocalNutrientLevel;
        set
        {
            if (UseStatContainer)
            {
                GetComponent<RootStateStatContainer>().NutrientLevel = value;
            }
            else
            {
                LocalNutrientLevel = value;
            }
        }
    }
    private int NutrientIntensity
    {
        get => UseStatContainer ? GetComponent<RootStateStatContainer>().NutrientIntensity : LocalNutrientIntensity;
        set
        {
            if (UseStatContainer)
            {
                GetComponent<RootStateStatContainer>().NutrientIntensity = value;
            }
            else
            {
                LocalNutrientIntensity = value;
            }
        }
    }
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
            /*
            NutrientIntensity += 1;
            //remove the fertilizer from inventory
            InventoryManager.Instance.RemoveItem(itemUsed);
            if (NutrientIntensity >= RootStats.NutrientTransitions.limit && RootStats.NutrientTransitions.limit != -1)
            {
                //Usestate manager to change stats
                //State = RootStates.Levelingup;
                //Make this below in state manager
                //CreateDialogue(RootStats.NutrientTransitions.dialogue);
                RootStats = (RootSO)RootStats.NutrientTransitions.characterTransition;
                NutrientLevel += 1;
                NutrientIntensity = 0;
                GameManager.Instance.MaxNumOfUprooters = RootStats.NumOfUprooters;
                Debug.Log("Transition to " + RootStats.Name);
            }
            */
            StateManager.NutrientUsedOn(nutrientUsed);
        }
    }
}
