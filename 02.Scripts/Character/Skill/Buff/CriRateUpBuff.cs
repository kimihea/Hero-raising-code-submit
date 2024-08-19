public class CriRateUpBuff : BuffSkillController
{
    public override void SetBuffStat()
    {
        BuffStat.CritRate = skill.DamagePerGradge();
    }
}
