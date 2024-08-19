using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Tilemaps;

public enum EQuestType
{
    STAGEPROGRESS,
    MONSTER,
    EQUIP,
    DUNGEON,
    ATKSTAT,
    HEALTHSTAT,
    DEFSTAT,
    SKILLCAST,
    MASTERY,
    EQUIPUPGRADE,
    HEROUPGRADE,
    HEROGACHA,
    BOSSBUTTON,
    HEROUPSTAR,
    HEROENTRY
}

public enum EQuestRewardType
{
    GOLD,
    DIAMOND,    
    MANASTONE,
    HEROESSENCE,
    UPGRADESTONE,
    TICKET
}

[Serializable]
public class QuestSaveData
{
    public static QuestSaveData SaveData { get => QuestManager.Instance.SaveData; }

    public int Index;    
    public int TotalEquipGacha;
    public int DungeonClear;
    public int MonsterKill;
    public int StageProgress;
    public bool IsComplete = false;
    public int TotalEquipUpgrade;
    public int TotalHeroUpgrade;
    public int TargetCount;
}

[Serializable]
public class QuestData
{
    public EQuestType QuestType;
    public int Value;
    public EQuestRewardType RewardType;
    public int Amount;
}

public class QuestList
{
    public List<QuestData> QuestDatas;
}

public class QuestManager : Singleton<QuestManager>
{
    public QuestSaveData SaveData = new QuestSaveData();
    [field: SerializeField] public List<QuestData> QuestList { get; private set; }
    public QuestData CurQuest;
    public Dictionary<EQuestType, Action<EQuestType, int>> QuestEventDictionary = new Dictionary<EQuestType, Action<EQuestType, int>>();
    public bool IsInit;

    public event Action OnChangeQuestTargetEvnet;

    protected override void Awake()
    {
        base.Awake();
        QuestEventDictionary.Add(EQuestType.STAGEPROGRESS, OnStageChange);
        QuestEventDictionary.Add(EQuestType.MONSTER, OnMonsterKill);
        QuestEventDictionary.Add(EQuestType.EQUIP, OnEquipGacha);
        QuestEventDictionary.Add(EQuestType.DUNGEON, OnDungeonClear);
        QuestEventDictionary.Add(EQuestType.SKILLCAST, OnCompleteTarget);
        //QuestEventDictionary.Add(EQuestType.MASTERY, OnCompleteTarget);
        QuestEventDictionary.Add(EQuestType.HEROUPGRADE, OnHeroUpgrade);
        QuestEventDictionary.Add(EQuestType.HEROGACHA, OnTargetCountChange);
        QuestEventDictionary.Add(EQuestType.BOSSBUTTON, OnCompleteTarget);
        QuestEventDictionary.Add(EQuestType.HEROUPSTAR, OnCompleteTarget);
        QuestEventDictionary.Add(EQuestType.HEROENTRY, OnCompleteTarget);
    }

    private void Start()
    {
        LoadQuestList();        
    }

    private async void LoadQuestList()
    {
        TextAsset data = await ResourceManager.Instance.GetResource<TextAsset>("QuestData", EAddressableType.DATA);
        QuestList = JsonUtility.FromJson<QuestList>(data.text).QuestDatas;
        SaveData = DataManager.Instance.LoadData<QuestSaveData>(ESaveType.QUEST);
        if (SaveData == null) SaveData = new QuestSaveData();
        if (SaveData.Index < QuestList.Count) CurQuest = QuestList[SaveData.Index];
        else CurQuest = null;
        IsInit = true;        
    }

    public void AddProgress(EQuestType type, int value)
    {
        if (CurQuest == null) return;
        if (QuestEventDictionary.ContainsKey(type))
        {
            QuestEventDictionary[type]?.Invoke(type, value);
        }
        if (type == CurQuest.QuestType)
        {
            OnChangeQuestTargetEvnet?.Invoke();
        }
    }

    private void OnStageChange(EQuestType type, int value)
    {
        SaveData.StageProgress = value;
    }

    private void OnMonsterKill(EQuestType type, int value)
    {
        if (CurQuest.QuestType != EQuestType.MONSTER) return;
        SaveData.MonsterKill += value;
    }

    private void OnEquipGacha(EQuestType type, int value)
    {
        SaveData.TotalEquipGacha += value;
    }

    private void OnDungeonClear(EQuestType type, int value)
    {
        SaveData.DungeonClear += value;
    }

    private void OnHeroUpgrade(EQuestType type, int value)
    {
        SaveData.TotalHeroUpgrade += value;
    }

    private void OnCompleteTarget(EQuestType type, int value)
    {
        SaveData.IsComplete = true;
    }

    private void OnTargetCountChange(EQuestType type, int value)
    {
        SaveData.TargetCount += value;
    }

    // TODO : Case > 추상 클래스로 리펙토링 필요

    public bool CheckCompleteQuest()
    {        
        switch (CurQuest.QuestType)
        {
            case EQuestType.STAGEPROGRESS:
                return SaveData.StageProgress >= CurQuest.Value;
            case EQuestType.MONSTER:
                return SaveData.MonsterKill >= CurQuest.Value;
            case EQuestType.EQUIP:
                return SaveData.TotalEquipGacha >= CurQuest.Value;
            case EQuestType.DUNGEON:
                return SaveData.DungeonClear >= CurQuest.Value;
            case EQuestType.ATKSTAT:
                return StatManager.Instance.Stats[(int)EStatType.ATK].statLevel >= CurQuest.Value;
            case EQuestType.HEALTHSTAT:
                return StatManager.Instance.Stats[(int)EStatType.HEALTH].statLevel >= CurQuest.Value;
            case EQuestType.DEFSTAT:
                return StatManager.Instance.Stats[(int)EStatType.DEFENSE].statLevel >= CurQuest.Value;
            case EQuestType.SKILLCAST:
                return SaveData.IsComplete;
            case EQuestType.MASTERY:
                return GameManager.Instance.Mastery.NodeList[0].Info.Condition >= EMasteryCondition.ISRESEARCHING;
            case EQuestType.EQUIPUPGRADE:
                return StatManager.Instance.equipment.TotalUpgradeNum() > SaveData.TotalEquipUpgrade;
            case EQuestType.HEROUPGRADE:
                return SaveData.TotalHeroUpgrade >= CurQuest.Value;
            case EQuestType.HEROGACHA:
                return SaveData.TargetCount >= CurQuest.Value;
            case EQuestType.BOSSBUTTON:
                return SaveData.IsComplete;
            case EQuestType.HEROUPSTAR:
                return SaveData.IsComplete;
            case EQuestType.HEROENTRY:
                return SaveData.IsComplete;
            default:
                return false;
        }
    }

    public void GetQuestReward()
    {
        switch (CurQuest.RewardType)
        {
            case EQuestRewardType.DIAMOND:
                CurrencyManager.Instance.AddCurrency(ECurrencyType.Diamond, new BigInteger(CurQuest.Amount));
                break;
            case EQuestRewardType.GOLD:
                CurrencyManager.Instance.AddCurrency(ECurrencyType.Gold, new BigInteger(CurQuest.Amount));
                break;
            case EQuestRewardType.MANASTONE:
                CurrencyManager.Instance.AddCurrency(ECurrencyType.ManaStone, new BigInteger(CurQuest.Amount));
                break;
            case EQuestRewardType.HEROESSENCE:
                CurrencyManager.Instance.AddCurrency(ECurrencyType.HeroEssence, new BigInteger(CurQuest.Amount));
                break;
            case EQuestRewardType.UPGRADESTONE:
                CurrencyManager.Instance.AddCurrency(ECurrencyType.UpgradeStone, new BigInteger(CurQuest.Amount));
                break;
            case EQuestRewardType.TICKET:
                GameManager.Instance.GoldDungeon.SaveData.TicketNum += CurQuest.Amount;
                break;
        }
    }

    public void CompleteQuest()
    {        
        GetQuestReward();

        // 퀘스트 완료 순간 초기화 필요한 데이터
        switch (CurQuest.QuestType)
        {   
            case EQuestType.EQUIP:
                SaveData.TotalEquipGacha -= CurQuest.Value;
                break;
            case EQuestType.EQUIPUPGRADE:
                SaveData.TotalEquipUpgrade += CurQuest.Value;
                break;
            case EQuestType.HEROUPGRADE:
                SaveData.TotalHeroUpgrade -= CurQuest.Value;
                break;
            default:
                break;
        }
        SaveData.Index++;
        DataManager.Instance.SaveData();
        if (SaveData.Index >= QuestList.Count) return;
        CurQuest = QuestList[SaveData.Index];

        // 퀘스트 수주 순간 초기화 필요한 데이터
        switch (CurQuest.QuestType)
        {
            case EQuestType.MONSTER:
                SaveData.MonsterKill = 0;
                break;
            case EQuestType.DUNGEON:
                SaveData.DungeonClear = 0;
                break;
            case EQuestType.SKILLCAST:
                SaveData.IsComplete = false;
                break;            
            case EQuestType.HEROGACHA:
                SaveData.TargetCount = 0;
                break;
            case EQuestType.BOSSBUTTON:
                SaveData.IsComplete = false;
                break;
            case EQuestType.HEROUPSTAR:
                SaveData.IsComplete = false;
                break;
            case EQuestType.HEROENTRY:
                SaveData.IsComplete = false;
                break;
            default:
                break;
        }        
    }

    public string GetQuestDesc()
    {  
        switch (CurQuest.QuestType)
        {
            case EQuestType.STAGEPROGRESS:
                int chapter = CurQuest.Value / 100;
                int stage = CurQuest.Value % 100;
                return string.Format("스테이지 {0} - {1}을 클리어 하세요", chapter, stage);
            case EQuestType.MONSTER:
                return string.Format("몬스터 {0}마리를 사냥하세요", CurQuest.Value);
            case EQuestType.EQUIP:
                return string.Format("미믹으로부터 장비를 {0}회 소환하세요", CurQuest.Value);
            case EQuestType.DUNGEON:
                return string.Format("던전을 {0}회 클리어하세요", CurQuest.Value);
            case EQuestType.ATKSTAT:
                return string.Format("훈련에서 공격력을 {0}Level까지 올리세요", CurQuest.Value);
            case EQuestType.HEALTHSTAT:
                return string.Format("훈련에서 체력을 {0}Level까지 올리세요", CurQuest.Value);
            case EQuestType.DEFSTAT:
                return string.Format("훈련에서 방어력을 {0}Level까지 올리세요", CurQuest.Value);
            case EQuestType.SKILLCAST:
                return string.Format("스킬을 장착하세요");
            case EQuestType.MASTERY:
                return string.Format("특성을 배우세요");
            case EQuestType.EQUIPUPGRADE:
                return string.Format("장비를 {0}회 강화하세요", CurQuest.Value);
            case EQuestType.HEROUPGRADE:
                return string.Format("동료를 {0}회 강화하세요", CurQuest.Value);
            case EQuestType.HEROGACHA:
                return string.Format("동료를 {0}회 모집하세요", CurQuest.Value);
            case EQuestType.BOSSBUTTON:
                return string.Format("보스 버튼을 눌러 보스 전에 진입하세요");
            case EQuestType.HEROUPSTAR:
                return string.Format("동료를 각성시키세요");
            case EQuestType.HEROENTRY:
                return string.Format("동료를 장착하세요");
            default:
                return null;
        }
    }

    public string GetQuestProgress()
    {
        switch (CurQuest.QuestType)
        {
            case EQuestType.STAGEPROGRESS:
                return string.Format("{0} / 1", CheckCompleteQuest() ? 1 : 0);
            case EQuestType.MONSTER:
                return string.Format("{0} / {1}", SaveData.MonsterKill, CurQuest.Value);
            case EQuestType.EQUIP:
                return string.Format("{0} / {1}", SaveData.TotalEquipGacha, CurQuest.Value);
            case EQuestType.DUNGEON:
                return string.Format("{0} / {1}", SaveData.DungeonClear, CurQuest.Value);
            case EQuestType.ATKSTAT:
                return string.Format("{0} / {1}", StatManager.Instance.Stats[(int)EStatType.ATK].statLevel, CurQuest.Value);
            case EQuestType.HEALTHSTAT:
                return string.Format("{0} / {1}", StatManager.Instance.Stats[(int)EStatType.HEALTH].statLevel, CurQuest.Value);
            case EQuestType.DEFSTAT:
                return string.Format("{0} / {1}", StatManager.Instance.Stats[(int)EStatType.DEFENSE].statLevel, CurQuest.Value);
            case EQuestType.SKILLCAST:
                return string.Format("{0} / 1", SaveData.IsComplete ? 1 : 0);
            case EQuestType.MASTERY:
                return string.Format("{0} / 1", GameManager.Instance.Mastery.NodeList[0].Info.Condition >= EMasteryCondition.ISRESEARCHING ? 1 : 0);
            case EQuestType.EQUIPUPGRADE:
                int curValue = StatManager.Instance.equipment.TotalUpgradeNum() - SaveData.TotalEquipUpgrade;
                curValue = Mathf.Max(curValue, 0);
                return string.Format("{0} / {1}", curValue, CurQuest.Value);
            case EQuestType.HEROUPGRADE:
                return string.Format("{0} / {1}", SaveData.TotalHeroUpgrade, CurQuest.Value);
            case EQuestType.HEROGACHA:
                return string.Format("{0} / {1}", SaveData.TargetCount, CurQuest.Value);
            case EQuestType.BOSSBUTTON:
                return string.Format("{0} / 1", SaveData.IsComplete ? 1 : 0);
            case EQuestType.HEROUPSTAR:
                return string.Format("{0} / 1", SaveData.IsComplete ? 1 : 0);
            case EQuestType.HEROENTRY:
                return string.Format("{0} / 1", SaveData.IsComplete ? 1 : 0);
            default:
                return null;
        }
    }
}
