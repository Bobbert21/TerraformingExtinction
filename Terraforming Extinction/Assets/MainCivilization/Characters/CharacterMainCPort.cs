using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMainCPort : MonoBehaviour
{
    
    public CharacterMainCPortSO characterMainCPortSO;

    //Do not edit
    //To-Do: Make these privates
    public string Name;
    public CharacterPsyche characterPsyche;
    public CharacterPhysical characterPhysical;

    private void Start()
    { 
        Name = characterMainCPortSO.Name;
        characterPsyche = new CharacterPsyche(characterMainCPortSO.CharacterPsycheSO);
        characterPhysical = new CharacterPhysical(characterMainCPortSO.CharacterPhysicalSO);
    }

    public void Initialize()
    {
        if(characterMainCPortSO == null)
        {
            Debug.LogError("CharacterMainCPortSO is not assigned!");
            return;
        }

        if(Name == null || Name == "")
        {
            Name = characterMainCPortSO.Name;
            characterPsyche = new CharacterPsyche(characterMainCPortSO.CharacterPsycheSO);
            characterPhysical = new CharacterPhysical(characterMainCPortSO.CharacterPhysicalSO);
        }
    }
}
