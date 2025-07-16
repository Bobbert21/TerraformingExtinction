using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPhysical : MonoBehaviour
{
    public Stats Stats;
    public List<EnumActionCharacteristics> actionCharacteristics;
    public List<EnumAppearanceCharacteristics> appearanceCharacteristics;
    public EnumIdentifiers Identifier;
    public EnumActionCharacteristics ActionCommitting;

    public CharacterPhysical(CharacterPhysicalSO characterPhysicalSO)
    {
        Stats = characterPhysicalSO.Stats;
        actionCharacteristics = characterPhysicalSO.actionCharacteristics;
        appearanceCharacteristics = characterPhysicalSO.appearanceCharacteristics;
        Identifier = characterPhysicalSO.Identifier;
        ActionCommitting = characterPhysicalSO.ActionCommitting;
    }
}
