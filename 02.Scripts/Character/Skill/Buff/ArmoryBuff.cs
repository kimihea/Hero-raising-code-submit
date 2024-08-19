using System;

public class ArmoryBuff : BuffSkillController
{
    //BuffSkillController에서 모든 작업 해주고 여기는 더할 스탯을 초기화 해주는 역할
    public override void SetBuffStat()
    {
        BuffStat.Health = skill.DamagePerGradge();
    }
    
}
