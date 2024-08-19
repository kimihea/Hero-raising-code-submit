using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerCloseAttack : CharacterCloseAttack
{
    public override void Start()
    {
        base.Start();
        ChangeAttackMotionSpeed();
        CriticalChance= characterController.character.StatHandler.curStat.GetCurCriticalInfo().CriRate;
    }
}
