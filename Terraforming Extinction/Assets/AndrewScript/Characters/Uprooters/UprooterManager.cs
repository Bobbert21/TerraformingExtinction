using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UprooterManager : MonoBehaviour
{
    public int Health;
    public int MaxHealth;
    public UprooterSO Uprooter;
    //use enum to figure out which fertilizer it is keeping track of. like red is 0 so index 0 in the array
    private int[] FertilizerIntensity;
    public int[] FertilizerLevel;

    private void Start()
    {
        FertilizerIntensity= new int[(int)FertilizerTypes.Count];
        FertilizerLevel = new int[(int)FertilizerTypes.Count];
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

            //in the uprooterSO, find the limit of this fertilizer 
            foreach (FertilizerTransition fertilizerTransition in Uprooter.FertilizerTransitions)
            {
                //if the index (fertilizer enum) = enum of fertilizerlimit in uprooters
                if(usedFertilizerIndex == (int)fertilizerTransition.type)
                {
                    usedFertilizerLimit = fertilizerTransition.limit;
                    uprooterTransition = fertilizerTransition.uprooterTransition;
                    break;
                }
            }
            //If the fertilizer increases beyond the limit. -1 means there are no more limits
            if(usedFertilizerIntensity >= usedFertilizerLimit && usedFertilizerLimit != -1)
            {
                Debug.Log(usedFertilizerLimit);
                Uprooter = uprooterTransition;
                FertilizerLevel[usedFertilizerIndex] += 1;
                Debug.Log("Transition to " + Uprooter.Name);
            }
        }
    }

}
