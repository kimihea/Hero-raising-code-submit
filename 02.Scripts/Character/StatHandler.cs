using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    public BaseStat baseStat = new();
    public BaseStat curStat = new();
    
    public int grade = 1;
    public int stars = 1;

    public List<BaseStat> statModifiers = new();

    [Header("디버프 한계치")]
    private readonly float minAtk;
    private readonly float minAttackSpeed;
    private readonly float minMoveSpeed;
    private readonly float minDefense;
    private readonly float minHealth;

    private readonly float minCritRate;
    private readonly float minCritMultiplier;
    private readonly float minSkillMultiplier;
    private readonly float minDamageMultiplier;
    private readonly float minHealMultiplier;

    private readonly float minAtkMultiplier;
    private readonly float minHealthMultiplier;
    private readonly float minDefenseMultiplier;
    private readonly float minAttackSpeedMultiplier;
    private readonly float minMoveSpeedMultiplier;


    private void Awake()
    {
        baseStat.StatChangeType = EStatChangeType.OVERRIDE;
        curStat.StatChangeType = EStatChangeType.ADD;
    }

    private void Start()
    {
        UpdateStatModifier();
    }

    public void AddStatModifier(BaseStat modifier)
    {
        statModifiers.Add(modifier);
        UpdateStatModifier();
    }

    public void RemoveStatModifier(BaseStat modifier)
    {
        statModifiers.Remove(modifier);
        UpdateStatModifier();
    }

    public void UpdateStatModifier()
    {        
        ApplyStatModifier(baseStat);

        foreach (BaseStat stat in statModifiers.OrderBy(stat => stat.StatChangeType))
        {
            //Debug.Log(stat.atk);
            ApplyStatModifier(stat);
        }
    }

    private void ApplyStatModifier(BaseStat modifier)
    {
        Func<float, float, float> operation = modifier.StatChangeType switch
        {
            EStatChangeType.ADD => (current, change) => current + change,            
            EStatChangeType.GRADE => (current, change) => current + (grade * change),
            EStatChangeType.STARS => (current, change) => current + (stars * change),
            EStatChangeType.MULTIPLE => (current, change) => current * change,
            EStatChangeType.OVERRIDE => (current, change) => change,
            _ => (current, change) => change
        };

        UpdateBaseStat(operation, modifier);

        if (curStat is CharacterStat cStat && modifier is CharacterStat newModifier)
        {
            UpdateCharacterStat(operation, cStat, newModifier);
        }
    }

    private void UpdateBaseStat(Func<float, float, float> operation, BaseStat modifier)
    {        
        curStat.Health = Mathf.Max((float)operation(curStat.Health, modifier.Health), minHealth);
        curStat.MoveSpeed = Mathf.Max((float)operation(curStat.MoveSpeed, modifier.MoveSpeed), minMoveSpeed);

        curStat.Atk = Mathf.Max((float)operation(curStat.Atk, modifier.Atk), minAtk);
        curStat.Defense = Mathf.Max((float)operation(curStat.Defense, modifier.Defense), minDefense);
        curStat.AttackSpeed = Mathf.Max((float)operation(curStat.AttackSpeed, modifier.AttackSpeed), minAttackSpeed);
        curStat.AttackRange = Mathf.Max((float)operation(curStat.AttackRange, modifier.AttackRange));
        //curStat.CritRate = Mathf.Max((float)operation(curStat.CritRate, modifier.CritRate), minCritRate);
        curStat.AtkMultiplier = Mathf.Max((float)operation(curStat.AtkMultiplier, modifier.AtkMultiplier), minAtkMultiplier);
        curStat.HealthMultiplier = Mathf.Max((float)operation(curStat.HealthMultiplier, modifier.HealthMultiplier), minHealthMultiplier);
        curStat.DefenseMultiplier = Mathf.Max((float)operation(curStat.DefenseMultiplier, modifier.DefenseMultiplier), minDefenseMultiplier);
        curStat.AttackSpeedMultiplier = Mathf.Max((float)operation(curStat.AttackSpeedMultiplier, modifier.AttackSpeedMultiplier), minAttackSpeedMultiplier);
        curStat.MoveSpeedMultiplier = Mathf.Max((float)operation(curStat.MoveSpeedMultiplier, modifier.MoveSpeedMultiplier), minMoveSpeedMultiplier);

    }

    private void UpdateCharacterStat(Func<float, float, float> operation, CharacterStat cStat, CharacterStat modifier)
    {
        cStat.CritRate = Mathf.Max((float)operation(cStat.CritRate, modifier.CritRate), minCritRate);
        cStat.CritMultiplier = Mathf.Max((float)operation(cStat.CritMultiplier, modifier.CritMultiplier), minCritMultiplier);
        cStat.SkillMultiplier = Mathf.Max((float)operation(cStat.SkillMultiplier, modifier.SkillMultiplier), minSkillMultiplier);
        cStat.DamageMultiplier = Mathf.Max((float)operation(cStat.DamageMultiplier, modifier.DamageMultiplier), minDamageMultiplier);
        cStat.HealMultiplier = Mathf.Max((float)operation(cStat.HealMultiplier, modifier.HealMultiplier), minHealMultiplier);

        //cStat.AtkMultiplier = Mathf.Max((float)operation(cStat.AtkMultiplier, modifier.AtkMultiplier), minAtkMultiplier);
        //cStat.HealthMultiplier = Mathf.Max((float)operation(cStat.HealthMultiplier, modifier.HealthMultiplier), minHealthMultiplier);
        //cStat.DefenseMultiplier = Mathf.Max((float)operation(cStat.DefenseMultiplier, modifier.DefenseMultiplier), minDefenseMultiplier);
        //cStat.AttackSpeedMultiplier = Mathf.Max((float)operation(cStat.AttackSpeedMultiplier, modifier.AttackSpeedMultiplier), minAttackSpeedMultiplier);
        //cStat.MoveSpeedMultiplier = Mathf.Max((float)operation(cStat.MoveSpeedMultiplier, modifier.MoveSpeedMultiplier), minMoveSpeedMultiplier);        
    }

    public void ChangeCharacterStat()
    {
        baseStat = new CharacterStat(baseStat);
        baseStat.StatChangeType = EStatChangeType.OVERRIDE;

        curStat = new CharacterStat(curStat);
    }
}

