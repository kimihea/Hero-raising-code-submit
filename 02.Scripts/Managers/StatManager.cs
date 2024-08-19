using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatManager : Singleton<StatManager>
{   
    public List<Stat> Stats;

    [SerializeField] public CharacterStat upgradeStat = new();

    public int repeatCount = 1;

    public event Action OnUpgrade;

    public StatHandler statHandler;

    public Equipment equipment;
    protected override void Awake()
    {
        base.Awake();

        statHandler = GetComponent<StatHandler>();
        statHandler.baseStat = new CharacterStat();
        statHandler.baseStat.StatChangeType = EStatChangeType.OVERRIDE;
        statHandler.curStat = new CharacterStat();

        equipment = GetComponent<Equipment>();
    }

    private void Start()
    {
        OnUpgrade += StatModifierUpdate;

        StatLoadData();

        GameManager.Instance.player.StatHandler.AddStatModifier(statHandler.curStat);

        statHandler.AddStatModifier(upgradeStat);
        StatModifierUpdate();
        CostCalc();
    }

    private void StatInitialize()
    {
        Stats.Add(new Stat(EStatType.ATK           , 1, 100, 100, 1, 1, "공격력 증가", 50));
        Stats.Add(new Stat(EStatType.HEALTH        , 1, 100, 100, 1, 10, "체력 증가", 50));
        Stats.Add(new Stat(EStatType.DEFENSE       , 1, 100, 100, 1, 1, "방어력 증가",50));
        Stats.Add(new Stat(EStatType.ATKSPEED      , 1, 100, 500, 1, 0.01f, "공격 속도 증가",500));
        Stats.Add(new Stat(EStatType.CRITRATE      , 1, 100, 500, 1, 0.03f, "치명타 확률 증가", 500));
        Stats.Add(new Stat(EStatType.CRITMULTIPLIER, 1, 100, 500, 1, 10, "치명타 피해 증가", 500));
    }

    public void StatLevelUp(int statType)
    {
        if (CurrencyManager.Instance.CurrencyDict[ECurrencyType.Gold].TrySpend((int)Stats[statType].totalCost))
        {
            for (int i = 0; i < repeatCount; i++)
            {
                Stats[statType].statLevel++;
                Stats[statType].statCost = (int)(Stats[statType].statCost + Stats[statType].increaseCost);
                Stats[statType].totalValue += Stats[statType].upValue;

                Stats[statType].totalValue = Mathf.Round(Stats[statType].totalValue * 100.0f) / 100.0f;


                OnUpgrade?.Invoke();
            }
            switch (statType)
            {
                case 0:
                    QuestManager.Instance.AddProgress(EQuestType.ATKSTAT, Stats[statType].statLevel);
                    break;
                case 1:
                    QuestManager.Instance.AddProgress(EQuestType.HEALTHSTAT, Stats[statType].statLevel);
                    break;
                case 2:
                    QuestManager.Instance.AddProgress(EQuestType.DEFSTAT, Stats[statType].statLevel);
                    break;
                default:
                    break;
            }
            CostCalc();
        }


        //Debug.Log(CurrencyManager.Instance.CurrencyDict[ECurrencyType.Gold].Amount);
    }

    public void StatModifierUpdate()
    {
        upgradeStat.Atk = Stats[(int)EStatType.ATK].totalValue;
        upgradeStat.Health = Stats[(int)EStatType.HEALTH].totalValue;
        upgradeStat.Defense = Stats[(int)EStatType.DEFENSE].totalValue;
        upgradeStat.AttackSpeedMultiplier = Stats[(int)EStatType.ATKSPEED].totalValue;
        upgradeStat.CritRate = Stats[(int)EStatType.CRITRATE].totalValue;
        upgradeStat.CritMultiplier = Stats[(int)EStatType.CRITMULTIPLIER].totalValue;
        /*
        upgradeStat.SkillMultiplier = Stats[(int)EStatType.SKILLMULTIPLIER].totalValue;
        upgradeStat.DamageMultiplier = Stats[(int)EStatType.DAMAGEMULTIPLIER].totalValue;
        upgradeStat.HealMultiplier = Stats[(int)EStatType.HEALMULTIPLIER].totalValue;*/

        statHandler.UpdateStatModifier();
        GameManager.Instance.player.StatHandler.UpdateStatModifier();
        GameManager.Instance.HeroUpdate();

        //HeroManager.Instance.DataUpdate();
    }

    public void SelectRepeatCount(int count)
    {
        repeatCount = count;

        CostCalc();
    }

    public void CostCalc()
    {
        for (int i = 0; i < Stats.Count; i++)
        {
            int requiredCost = Stats[i].statCost;
            int totalCost = requiredCost;

            for (int j = 0; j < repeatCount - 1; j++)
            {
                requiredCost = (int)(requiredCost + Stats[i].increaseCost);
                totalCost += requiredCost;
            }

            Stats[i].totalCost = totalCost;
        }
    }

    public void StatLoadData()
    {
        //Debug.Log($"<color=red> 스탯 로드 데이터 </color>");

        List<Stat> loadData = DataManager.Instance.LoadData<List<Stat>>(ESaveType.STAT);

        if (loadData == null)
        {
            //Debug.Log($"<color=red> 스탯 이니셜라이즈 </color>");

            StatInitialize();
        }
        else
        {
            //Debug.Log($"<color=red> 스탯 로드 </color>");

            Stats = loadData;
            OnUpgrade?.Invoke();
            
        }
                
    }
}

[System.Serializable]
public class Stat
{
    public EStatType StatType;
    public int statLevel;
    public int statMaxLevel;
    public int statCost;
    public int unlockLevel;
    public float totalValue;
    public float upValue;
    public string statDescription;
    public float increaseCost;

    public long totalCost;

    public Stat(EStatType statType, int level, int maxLevel, int cost, int unlockLevel, float upValue, string statDesc, float increaseCost)
    {
        this.StatType = statType;
        this.statLevel = level;
        this.statMaxLevel = maxLevel;
        this.statCost = cost;
        this.unlockLevel = unlockLevel;
        this.upValue = Mathf.Round(upValue * 100.0f) / 100.0f;
        this.statDescription = statDesc;
        this.increaseCost = increaseCost;
    }
}