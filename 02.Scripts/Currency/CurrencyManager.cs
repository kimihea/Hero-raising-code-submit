using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : Singleton<CurrencyManager>
{
    public RectTransform GoldUI;
    public RectTransform MimicUI;

    // 싱글톤 인스턴스
    //public static CurrencyManager Instance { get; private set; }

    // 모든 통화를 저장하는 딕셔너리
    public Dictionary<ECurrencyType, Currency> CurrencyDict;

    // 스킬 스크롤과 영웅 조각을 저장하는 딕셔너리
    public Dictionary<int, Currency> SkillScrollDict { get; private set; }
    public Dictionary<int, HeroFragment> HeroFragmentDict { get; private set; }

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
    }

    private void InitializeCurrencies()
    {
        CurrencyDict = new Dictionary<ECurrencyType, Currency>();
        foreach(ECurrencyType type in Enum.GetValues(typeof(ECurrencyType)))
        {
            Currency currency = new Currency();            
            CurrencyDict.Add(type, currency);
        }
        CurrencyDict[ECurrencyType.Gold].Add(500000);        

        SkillScrollDict = new Dictionary<int, Currency>();
        HeroFragmentDict = new Dictionary<int, HeroFragment>();
    }

    // 통화 획득 메소드
    public void AddCurrency(ECurrencyType currencyType, int amount)
    {
        CurrencyDict[currencyType].Add(amount);
    }

    // 통화 사용 메소드
    public bool UseCurrency(ECurrencyType currencyType, int amount)
    {
        return CurrencyDict[currencyType].TrySpend(amount);
    }    

    // 통화 금액 조회 메소드
    public int GetCurrencyAmount(ECurrencyType currencyType)
    {
        return CurrencyDict[currencyType]?.Amount ?? 0;
    }
}
