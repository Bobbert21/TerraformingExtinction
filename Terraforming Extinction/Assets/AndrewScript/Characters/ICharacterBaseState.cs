using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterBaseState
{
    bool PlayerSelects(ICharacterStateManager stateManager);
    void PlayerDeselects(ICharacterStateManager stateManager);
}
