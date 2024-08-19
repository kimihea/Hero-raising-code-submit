using System;
using UnityEngine;

[Serializable]
public class BaseStat 
{
    public EStatChangeType StatChangeType;
    public float Atk;
    public float Health;
    public float Defense;
    public float AttackSpeed;
    public float MoveSpeed;
    public float AttackRange;

    public float AtkMultiplier;
    public float HealthMultiplier;
    public float DefenseMultiplier;
    public float AttackSpeedMultiplier;
    public float MoveSpeedMultiplier;

    public float GetCurHealth()
    {
        return Health * (1 + HealthMultiplier / 100f);
    }

    public float GetCurDefense()
    {
        return Defense * (1 + DefenseMultiplier / 100f); 
    }

    public virtual int GetCurAtk()
    {
        return (int)(Atk * (1 + AtkMultiplier / 100f));
    }
    public virtual (float CriRate, float CriDamage) GetCurCriticalInfo()
    {
        float criRate = 0.25f;
        float criDamage = 2;
        return (criRate, criDamage);
    }
    public virtual float GetDamageMuliplier()
    {
        return 1;
    }
}

[Serializable]
public class CharacterStat : BaseStat
{
    public float CritRate = 0f;
    public float CritMultiplier = 0f;
    public float SkillMultiplier = 0f; 
    public float DamageMultiplier = 0f;
    public float HealMultiplier = 0f;
    public CharacterStat(BaseStat baseStat = null)
    {
        if (baseStat != null)
        {
            // BaseStat의 필드들을 복사
            this.StatChangeType = baseStat.StatChangeType;
            this.Atk = baseStat.Atk;
            this.Health = baseStat.Health;
            this.Defense = baseStat.Defense;
            this.AttackSpeed = baseStat.AttackSpeed;
            this.MoveSpeed = baseStat.MoveSpeed;
            this.AttackRange = baseStat.AttackRange;

            this.AtkMultiplier = baseStat.AtkMultiplier;
            this.HealthMultiplier = baseStat.HealthMultiplier;
            this.DefenseMultiplier = baseStat.DefenseMultiplier;
            this.AttackSpeedMultiplier = baseStat.AttackSpeedMultiplier;
            this.MoveSpeedMultiplier = baseStat.MoveSpeedMultiplier;
        }
    }

    public override int GetCurAtk()
    {
        return (int)(Atk * (1 + AtkMultiplier / 100f));
    }
    public override (float CriRate, float CriDamage) GetCurCriticalInfo()
    {
        float criRate = CritRate;
        float criDamage = 2+ CritMultiplier/100;
        return (criRate, criDamage);
    }
    public override float GetDamageMuliplier()
    {
        return (1+DamageMultiplier);
    }
}

