using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    public BaseStat baseStat = new();
    public BaseStat curStat = new();

    public int grade = 0;
    public int stars = 0;

    public List<BaseStat> statModifiers = new();

    [Header("디버프 한계치")]
    private readonly float minAtk;
    private readonly float minAttackSpeed;
    private readonly float minMoveSpeed;
    private readonly float minDefense;
    private readonly float minHealth;

    private readonly float minCritRate;

    

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
            EStatChangeType.MULTIPLE => (current, change) => current * change,
            EStatChangeType.GRADE => (current, change) => current * change,
            EStatChangeType.STARS => (current, change) => current * change,
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
    }

    private void UpdateCharacterStat(Func<float, float, float> operation, CharacterStat cStat , CharacterStat modifier)
    {
        cStat.CritRate = Mathf.Max((float)operation(cStat.CritRate, modifier.CritRate), minCritRate);
    }
}

