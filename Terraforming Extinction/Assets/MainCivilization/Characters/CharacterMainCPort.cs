using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMainCPort : MonoBehaviour
{
    public string Name;
    public CharacterPsyche characterPsyche;
    public CharacterPhysical characterPhysical;

    public CharacterMainCPort(CharacterMainCPortSO characterMainCPortSO)
    {
        Name = characterMainCPortSO.Name;
        characterPsyche = new CharacterPsyche(characterMainCPortSO.characterPsycheSO);
        characterPhysical = new CharacterPhysical(characterMainCPortSO.CharacterPhysicalSO);
    }
}
