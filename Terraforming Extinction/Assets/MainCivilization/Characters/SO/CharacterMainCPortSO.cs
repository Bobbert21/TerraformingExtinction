using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMainCPortSO", menuName = "ScriptableObject/CPort/CharacterMainCPortSO")]
public class CharacterMainCPortSO : ScriptableObject
{
    public string Name;
    public CharacterPsycheSO characterPsycheSO;
    public CharacterPhysicalSO CharacterPhysicalSO;

}
