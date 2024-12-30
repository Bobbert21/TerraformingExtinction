using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterStateManager 
{
    void CreateDialogue();
    GameObject PlayerSelected();
    void PlayerDeselected();
}
