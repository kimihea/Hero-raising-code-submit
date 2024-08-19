using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMastery : BaseMastery
{
    public CharacterStat StatModifier;

    public override void ApplyMastery()
    {
        StatManager.Instance.statHandler.AddStatModifier(StatModifier);
    }    
}
