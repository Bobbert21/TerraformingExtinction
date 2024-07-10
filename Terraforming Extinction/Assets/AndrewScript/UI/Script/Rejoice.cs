using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rejoice : MonoBehaviour
{
    public GameObject ObjSelected;
    public PlayerSelects PlayerSelecting;
    public void setObject(GameObject obj) { ObjSelected = obj; }
    public void setPlayer(PlayerSelects playerSelecting) { PlayerSelecting = playerSelecting;  }


    public void SetUprooterActiveState()
    {
        //will prompt dialogue when waking up and in dialogue script will set active
        var uprooterGeneralStatScript = ObjSelected.GetComponent<UprooterGeneralStatsContainer>();
        uprooterGeneralStatScript.StateManager.ActivateUprooter();
        //Should do a UprooterManager instead of GameManager
        UprooterManager.Instance.CurrentNumOfUprooters += 1;
        Destroy(gameObject);
    }

    private void Update()
    {
        //if the player selects something else
        if(ObjSelected != null && PlayerSelecting != null)
        {
            if(ObjSelected != PlayerSelecting.ObjSelected)
            {
                Destroy(gameObject);
            }
        }
    }
}
