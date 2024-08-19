using UnityEngine;

public class OverClockBuff : BuffSkillController
{
    public GameObject AnimObj;
    public Character character;
    public override void SetBuffStat()
    {
        BuffStat.DefenseMultiplier = skill.DamagePerGradge()-1;
        BuffStat.DamageMultiplier = (skill.DamagePerGradge()-1)/2;
    }
    protected override void ExecuteSkill()
    { 
        IHandleBuff buff = GetComponentInParent<IHandleBuff>();
        buff?.ActiveBuff(BuffStat, skill.Duration, Type);
    }
    //protected internal override void InterruptSkill()
    //{
    //    base.InterruptSkill();
    //    AnimObj?.SetActive(false);
    //}
}
