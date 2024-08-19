using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CurrencySaveData
{
    public List<ECurrencyType> CurrencyKeyList = new List<ECurrencyType>();
    public List<int> CurrencyValueList = new List<int>();

    public List<int> SkillCurrencyKeyList = new List<int>();
    public List<int> SkillCurrencyValueList = new List<int>();

    public List<int> HeroCurrencyKeyList = new List<int>();
    public List<int> HeroCurrencyValueList = new List<int>();
}

public class CurrencyManager : Singleton<CurrencyManager>
{
    public RectTransform GoldUI;
    public RectTransform MimicUI;

    public Slider ManaStoneFragmentGauge; // 게이지 바를 나타내는 Slider
    public Text ManaStoneText;           // ManaStone의 양을 나타내는 Text
    public Text EquipmentResultText;    // 장비 뽑기 결과를 표시하는 Text
    public Text AteManaStoneText;       // 지금까지 먹은 마석 갯수를 표시하는 Text
    public Text DiamondText;            // 다이아몬드 양을 나타내는 Text

    public int totalManaStonesConsumed = 0;  // 지금까지 먹은 마석 갯수

    // 모든 통화를 저장하는 딕셔너리
    public Dictionary<ECurrencyType, Currency> CurrencyDict;

    // 스킬 스크롤과 영웅 조각을 저장하는 딕셔너리
    public Dictionary<int, Currency> SkillScrollDict { get; private set; }
    public Dictionary<int, Currency> HeroFragmentDict { get; private set; }

    public CurrencySaveData CurrencySaveData = new();
    



    #region legacy
    //// 게임 내 주요 통화들
    //public Currency ManaStone => GetCurrency("ManaStone");
    //public Currency ManaStoneFragment => GetCurrency("ManaStoneFragment");
    //public Currency Gold => GetCurrency("Gold");
    //public Currency UpgradeStone => GetCurrency("UpgradeStone");
    //public Currency Diamond => GetCurrency("Diamond");
    //public Currency HeroEssence => GetCurrency("HeroEssence");

    //private void Awake()
    //{
    //    // 싱글톤 패턴 구현
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //        InitializeCurrencies();
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}
    //// 통화 조회 메소드
    //public Currency GetCurrency(string currencyName)
    //{
    //    if (CurrencyDict.TryGetValue(currencyName, out Currency currency))
    //    {
    //        return currency;
    //    }
    //    Debug.LogWarning($"Currency {currencyName} not found.");
    //    return null;
    //}
    #endregion

    private void Start()
    {
        InitializeCurrencies();
        UpdateManaStoneText();      // 초기 텍스트 업데이트
        UpdateAteManaStoneText();   // 초기 AteManaStoneText 업데이트
        UpdateDiamondText();        // 초기 DiamondText 업데이트
    }

    // 통화와 관련된 데이터 초기화 메서드
    private void InitializeCurrencies()
    {
        CurrencyDict = new Dictionary<ECurrencyType, Currency>();
        foreach(ECurrencyType type in Enum.GetValues(typeof(ECurrencyType)))
        {
            Currency currency = new Currency();            
            CurrencyDict.Add(type, currency);
        }


        //CurrencyDict[ECurrencyType.HeroEssence].Add(500000);

        //CurrencyDict[ECurrencyType.UpgradeStone].Add(500);

        CurrencyDict[ECurrencyType.Gold].Add(10000);   // 초기 골드 양 설정
        //CurrencyDict[ECurrencyType.Diamond].Add(60000); // 초기 다이아몬드 양 설정


        SkillScrollDict = new Dictionary<int, Currency>();
        HeroFragmentDict = new Dictionary<int, Currency>();


        CurrencySaveData loadData = DataManager.Instance.LoadData<CurrencySaveData>(ESaveType.CURRUNCY);

        if (loadData != null)
        {
            for ( int i = 0; i < loadData.CurrencyKeyList.Count; i++)
            {
                ECurrencyType eCurrencyType = loadData.CurrencyKeyList[i];

                CurrencyDict[eCurrencyType].ChangeAmount(loadData.CurrencyValueList[i]);
            }

            for (int i = 0; i < loadData.SkillCurrencyKeyList.Count; i++)
            {
                int hid = loadData.SkillCurrencyKeyList[i];

                SkillScrollDict[hid].ChangeAmount( loadData.SkillCurrencyValueList[i]);
            }

            for (int i = 0; i < loadData.HeroCurrencyKeyList.Count; i++)
            {
                int hid = loadData.HeroCurrencyKeyList[i];

                Currency currency = new Currency();
                HeroFragmentDict.Add(hid, currency);

                HeroFragmentDict[hid].ChangeAmount(loadData.HeroCurrencyValueList[i]);
            }
        }

    }

    // 통화 획득 메소드
    public void AddCurrency(ECurrencyType currencyType, BigInteger amount)
    {
        CurrencyDict[currencyType].Add(amount);
        
        if (currencyType == ECurrencyType.ManaStoneFragment)
        {
            UpdateManaStoneFragmentGauge();
        }
        if (currencyType == ECurrencyType.ManaStone)
        {
            UpdateManaStoneText();
        }
        if (currencyType == ECurrencyType.Diamond)
        {
            UpdateDiamondText();
        }
    }

    // 통화 사용 메소드
    public bool UseCurrency(ECurrencyType currencyType, BigInteger amount)
    {
        bool result = CurrencyDict[currencyType].TrySpend(amount);
        
        if (result && currencyType == ECurrencyType.ManaStone)
        {
            UpdateManaStoneText();
        }
        if (result && currencyType == ECurrencyType.Diamond)
        {
            UpdateDiamondText();
        }
        return result;
    }    

    // 통화 금액 조회 메소드
    public BigInteger GetCurrencyAmount(ECurrencyType currencyType)
    {
        return CurrencyDict[currencyType]?.Amount ?? 0;
    }

    // ManaStoneFragment 게이지 업데이트 메소드
    private void UpdateManaStoneFragmentGauge()
    {
        BigInteger currentAmount = CurrencyDict[ECurrencyType.ManaStoneFragment].Amount;
        BigInteger maxAmount = 100; // 최대 값을 100로 설정

        ManaStoneFragmentGauge.value = (float)(currentAmount > maxAmount ? maxAmount : currentAmount) / (float)maxAmount;

        // 게이지가 가득 찼는지 확인하고 초기화
        if (ManaStoneFragmentGauge.value >= 1f)
        {
            // 보상 지급
            GiveManaStoneReward();

            // 게이지 초기화
            ResetManaStoneFragmentGauge();
        }
    }

    // ManaStoneFragment 게이지 초기화 메소드
    private void ResetManaStoneFragmentGauge()
    {
        // 게이지 초기화
        ManaStoneFragmentGauge.value = 0;

        // 통화 초기화
        CurrencyDict[ECurrencyType.ManaStoneFragment].ChangeAmount(0);
    }

    // ManaStone 보상 지급 메소드
    private void GiveManaStoneReward()
    {
        // ManaStone 1개 추가
        AddCurrency(ECurrencyType.ManaStone, 1);
    }

    // ManaStone 양 업데이트 메소드
    private void UpdateManaStoneText()
    {
        BigInteger manaStoneAmount = GetCurrencyAmount(ECurrencyType.ManaStone);
        if (ManaStoneText != null)
        {
            ManaStoneText.text = $"장비 뽑기 횟수: {manaStoneAmount}회";
        }
    }

    // Diamond 양 업데이트 메소드
    private void UpdateDiamondText()
    {
        BigInteger diamondAmount = GetCurrencyAmount(ECurrencyType.Diamond);
        if (DiamondText != null)
        {
            DiamondText.text = $"다이아몬드: {diamondAmount}";
        }
    }


    // AteManaStoneText 업데이트 메소드
    public void UpdateAteManaStoneText()
    {
        if (AteManaStoneText != null)
        {
            AteManaStoneText.text = $"지금까지 먹은 마석 갯수 : {totalManaStonesConsumed}회";
        }
    }

    public bool TryDrawEquipment()
    {
        BigInteger manaStoneAmount = GetCurrencyAmount(ECurrencyType.ManaStone);
        EquipmentResultText.gameObject.SetActive(true); //  활성화

        if (manaStoneAmount > 0)
        {
            UseCurrency(ECurrencyType.ManaStone, 1);
            QuestManager.Instance.AddProgress(EQuestType.EQUIP, 1);
            totalManaStonesConsumed++; // 총 먹은 마석 갯수 증가
            UpdateAteManaStoneText(); // 텍스트 업데이트

            if (MimicManager.Instance != null)
            {
                MimicManager.Instance.AddDrawCount();   // 미믹 레벨 업데이트
            }
            return true;
        }
        else
        {
            //EquipmentResultText.text = "ManaStone이 부족합니다.";
            //Invoke("ResetEquipmentResultText", 1.5f); // 1.5초 후 초기화
            GameManager.Instance.ShowAlert("마석이 부족합니다", EAlertType.LACK);
            return false;
        }
    }

    // EquipmentResultText 초기화 메서드
    public void ResetEquipmentResultText()
    {
        EquipmentResultText.text = "";
        EquipmentResultText.gameObject.SetActive(false);    //  비활성화
    }

    public void SaveData()
    {
        CurrencySaveData.CurrencyKeyList.Clear();
        CurrencySaveData.CurrencyValueList.Clear();
        CurrencySaveData.SkillCurrencyKeyList.Clear();
        CurrencySaveData.SkillCurrencyValueList.Clear();
        CurrencySaveData.HeroCurrencyKeyList.Clear();
        CurrencySaveData.HeroCurrencyValueList.Clear();
        if (CurrencyDict == null) return;
        foreach (KeyValuePair<ECurrencyType, Currency> item in CurrencyDict)
        {
            CurrencySaveData.CurrencyKeyList.Add(item.Key);
            CurrencySaveData.CurrencyValueList.Add((int)item.Value.Amount);
        }

        foreach (KeyValuePair<int, Currency> item in SkillScrollDict)
        {
            CurrencySaveData.SkillCurrencyKeyList.Add(item.Key);
            CurrencySaveData.SkillCurrencyValueList.Add((int)item.Value.Amount);
        }

        foreach (KeyValuePair<int, Currency> item in HeroFragmentDict)
        {
            CurrencySaveData.HeroCurrencyKeyList.Add(item.Key);
            CurrencySaveData.HeroCurrencyValueList.Add((int)item.Value.Amount);
        }
    }

    
}
