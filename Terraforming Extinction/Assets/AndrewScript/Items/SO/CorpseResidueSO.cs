using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CorpseResidueSO", menuName = "ScriptableObject/CorpseResidue")]
public class CorpseResidueSO : ItemSO
{
    public int Damage;
    public int DamageSpeed;
    public int Duration;
    public int Delay;
    public DepositRarities Rarities;
    public FertilizerTypes[] FertilizedTarget;
    public ItemSO[] ItemTargets;
}
