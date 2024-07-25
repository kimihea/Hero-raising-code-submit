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
}

[Serializable]
public class CharacterStat : BaseStat 
{
    public CharacterStat(BaseStat baseStat = null)
    {
        if (baseStat == null)
        {
            baseStat = new BaseStat();
        }
        this.StatChangeType = baseStat.StatChangeType;
        this.Atk = baseStat.Atk;
        this.Health = baseStat.Health;
        this.Defense = baseStat.Defense;
        this.AttackSpeed = baseStat.AttackSpeed;
        this.MoveSpeed = baseStat.MoveSpeed;
        this.AttackRange = baseStat.AttackRange;
    }
    public float CritRate;
    public float CritMultiplier;
    public float SkillMultiplier;
    public float DamageMultiplier;
    public float HealMultiplier;
}

