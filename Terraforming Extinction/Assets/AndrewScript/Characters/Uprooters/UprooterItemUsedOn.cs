using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Will have to change in future if more ways to level change
public class UprooterItemUsedOn : MonoBehaviour
{
    public bool UseStatContainer = false;

    [Header("Local variables to debug")]
    public UprooterStateManager LocalStateManager;
    public NutrientSO nutrientTestAdd;
    public FertilizerSO fertilizerTestAdd;

    // Local variables: Need to make get, set

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ItemUsedOn(nutrientTestAdd);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            ItemUsedOn(fertilizerTestAdd);
        }
    }




    private UprooterStateManager StateManager
    {
        get => UseStatContainer ? GetComponent<UprooterGeneralStatsContainer>().StateManager : LocalStateManager;
    }


    // when N or F is used on the uprooter
    public void ItemUsedOn(ItemSO itemUsed)
    {
        if (itemUsed is FertilizerSO fertilizerUsed)
        {
            StateManager.FertilizerUsedOn(fertilizerUsed);
        }
        else if (itemUsed is NutrientSO nutrientUsed)
        {
            StateManager.NutrientUsedOn(nutrientUsed);
        }
        else
        {
            Debug.Log("Item used is not applicable to uprooter");
        }
    }
}
