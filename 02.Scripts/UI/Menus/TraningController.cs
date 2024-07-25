using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingController : MonoBehaviour
{
    // 배율 버튼
    public Button x1Button;
    public Button x10Button;
    public Button x100Button;

    // 배율 버튼 텍스트 컴포넌트
    private Text x1Text;
    private Text x10Text;
    private Text x100Text;

    // 공격력 증가
    public Button STREnhanceButton;
    public Text STRCostText;
    public Text STRCurrentLevelText;

    // 체력 증가
    public Button HPEnhanceButton;
    public Text HPCostText;
    public Text HPCurrentLevelText;

    // 체력 재생 증가
    public Button HPREnhanceButton;
    public Text HPRCostText;
    public Text HPRCurrentLevelText;

    // 방어력 증가
    public Button DEFEnhanceButton;
    public Text DEFCostText;
    public Text DEFCurrentLevelText;

    // 치명타 확률 증가
    public Button CRTEnhanceButton;
    public Text CRTCostText;
    public Text CRTCurrentLevelText;

    // 초기 레벨과 비용
    private int STRLevel = 1;
    private int STRCost = 100;

    private int HPLevel = 1;
    private int HPCost = 100;

    private int HPRLevel = 1;
    private int HPRCost = 100;

    private int DEFLevel = 1;
    private int DEFCost = 100;

    private int CRTLevel = 1;
    private int CRTCost = 100;

    // 선택된 배율을 저장할 변수
    private int selectedMultiplier = 1;


    void Start()
    {
        // 배율 버튼의 텍스트 컴포넌트 초기화
        x1Text = x1Button.GetComponentInChildren<Text>();
        x10Text = x10Button.GetComponentInChildren<Text>();
        x100Text = x100Button.GetComponentInChildren<Text>();

        // 배율 버튼 
        x1Button.onClick.AddListener(() => SelectMultiplier(1));
        x10Button.onClick.AddListener(() => SelectMultiplier(10));
        x100Button.onClick.AddListener(() => SelectMultiplier(100));

        /*// 강화 버튼 클릭 시 해당 함수 호출
        STREnhanceButton.onClick.AddListener(() => OnEnhanceButtonClick("STR"));
        HPEnhanceButton.onClick.AddListener(() => OnEnhanceButtonClick("HP"));
        HPREnhanceButton.onClick.AddListener(() => OnEnhanceButtonClick("HPR"));
        DEFEnhanceButton.onClick.AddListener(() => OnEnhanceButtonClick("DEF"));
        CRTEnhanceButton.onClick.AddListener(() => OnEnhanceButtonClick("CRT"));*/

        // 강화 버튼 클릭 시 해당 함수 호출
        STREnhanceButton.onClick.AddListener(() => StatManager.Instance.StatLevelUp((int)EStatType.ATK));
        STREnhanceButton.onClick.AddListener(() => UpdateUI());

        HPEnhanceButton.onClick.AddListener(() => StatManager.Instance.StatLevelUp((int)EStatType.HEALTH));
        HPEnhanceButton.onClick.AddListener(() => UpdateUI());

        HPREnhanceButton.onClick.AddListener(() => StatManager.Instance.StatLevelUp((int)EStatType.DEFENSE));
        HPREnhanceButton.onClick.AddListener(() => UpdateUI());

        DEFEnhanceButton.onClick.AddListener(() => StatManager.Instance.StatLevelUp((int)EStatType.ATKSPEED));
        DEFEnhanceButton.onClick.AddListener(() => UpdateUI());

        CRTEnhanceButton.onClick.AddListener(() => StatManager.Instance.StatLevelUp((int)EStatType.CRITRATE));
        CRTEnhanceButton.onClick.AddListener(() => UpdateUI());
        // 초기 배율 버튼 상태 설정
        SelectMultiplier(1);
    }

    private void Update()
    {
        UpdateUI();
    }

    // 선택된 배율에 따라 배율 버튼 상태를 변경하는 메서드
    private void SelectMultiplier(int multiplier)
    {
        selectedMultiplier = multiplier;

        // 모든 배율 버튼 초기화
        x1Button.interactable = true;
        x10Button.interactable = true;
        x100Button.interactable = true;

        // 모든 배율 버튼의 텍스트 색상 초기화
        x1Text.color = Color.black;
        x10Text.color = Color.black;
        x100Text.color = Color.black;

        // 선택된 배율 버튼을 비활성화
        if (multiplier == 1)
        {
            x1Button.interactable = false;
            x1Text.color = Color.yellow;
        }
        else if (multiplier == 10)
        {
            x10Button.interactable= false;
            x10Text.color = Color.yellow;
        }
        else if (multiplier == 100)
        {
            x100Button.interactable= false;
            x100Text.color = Color.yellow;
        }

        StatManager.Instance.SelectRepeatCount(multiplier);

        UpdateUI();
    }

    // 강화 종류별 배율 적용 함수
    private void OnEnhanceButtonClick(string statType)
    {
        if (statType == "STR")
        {
            STRLevel += selectedMultiplier;
            STRCost += 50 * selectedMultiplier;
        }
        else if (statType == "HP")
        {
            HPLevel += selectedMultiplier;
            HPCost += 50 * selectedMultiplier;
        }
        else if (statType == "HPR")
        {
            HPRLevel += selectedMultiplier;
            HPRCost += 50 * selectedMultiplier;
        }
        else if (statType == "DEF")
        {
            DEFLevel += selectedMultiplier;
            DEFCost += 50 * selectedMultiplier;
        }
        else if(statType == "CRT")
        {
            CRTLevel += selectedMultiplier;
            CRTCost += 50 * selectedMultiplier;
        }

        UpdateUI();
    }

    // UI를 업데이트하는 메서드
    private void UpdateUI()
    {
        STRCostText.text = StatManager.Instance.Stats[(int)EStatType.ATK].totalCost + " G";
        STRCurrentLevelText.text = $"{StatManager.Instance.Stats[(int)EStatType.ATK].statDescription} Lv. {StatManager.Instance.Stats[(int)EStatType.ATK].statLevel}";

        HPCostText.text = StatManager.Instance.Stats[(int)EStatType.HEALTH].totalCost + " G";
        HPCurrentLevelText.text = $"{StatManager.Instance.Stats[(int)EStatType.HEALTH].statDescription} Lv. {StatManager.Instance.Stats[(int)EStatType.HEALTH].statLevel}";

        HPRCostText.text = StatManager.Instance.Stats[(int)EStatType.DEFENSE].totalCost + " G";
        HPRCurrentLevelText.text = $"{StatManager.Instance.Stats[(int)EStatType.DEFENSE].statDescription} Lv. {StatManager.Instance.Stats[(int)EStatType.DEFENSE].statLevel}";

        DEFCostText.text = StatManager.Instance.Stats[(int)EStatType.ATKSPEED].totalCost + " G";
        DEFCurrentLevelText.text = $"{StatManager.Instance.Stats[(int)EStatType.ATKSPEED].statDescription} Lv. {StatManager.Instance.Stats[(int)EStatType.ATKSPEED].statLevel}";

        CRTCostText.text = StatManager.Instance.Stats[(int)EStatType.CRITRATE].totalCost + " G";
        CRTCurrentLevelText.text = $"{StatManager.Instance.Stats[(int)EStatType.CRITRATE].statDescription} Lv. {StatManager.Instance.Stats[(int)EStatType.CRITRATE].statLevel}";

        STRCostText.color = StatManager.Instance.Stats[(int)EStatType.ATK].totalCost <= CurrencyManager.Instance.CurrencyDict[ECurrencyType.Gold].Amount ? Color.white : Color.red;
        HPCostText.color = StatManager.Instance.Stats[(int)EStatType.HEALTH].totalCost <= CurrencyManager.Instance.CurrencyDict[ECurrencyType.Gold].Amount ? Color.white : Color.red;
        HPRCostText.color = StatManager.Instance.Stats[(int)EStatType.DEFENSE].totalCost <= CurrencyManager.Instance.CurrencyDict[ECurrencyType.Gold].Amount ? Color.white : Color.red;
        DEFCostText.color = StatManager.Instance.Stats[(int)EStatType.ATKSPEED].totalCost <= CurrencyManager.Instance.CurrencyDict[ECurrencyType.Gold].Amount ? Color.white : Color.red;
        CRTCostText.color = StatManager.Instance.Stats[(int)EStatType.CRITRATE].totalCost <= CurrencyManager.Instance.CurrencyDict[ECurrencyType.Gold].Amount ? Color.white : Color.red;


        // 강화 비용 텍스트 업데이트
        /*STRCostText.text = STRCost + " G";

        // 레벨 텍스트 업데이트
        STRCurrentLevelText.text = "공격력 증가 Lv." + STRLevel;


        HPCostText.text = HPCost + " G";
        HPCurrentLevelText.text = "체력 증가 Lv." + HPLevel;

        HPRCostText.text = HPRCost + " G";
        HPRCurrentLevelText.text = "체력 재생 증가 Lv." + HPRLevel;

        DEFCostText.text = DEFCost + " G";
        DEFCurrentLevelText.text = "방어력 증가 Lv." + DEFLevel;

        CRTCostText.text = CRTCost + " G";
        CRTCurrentLevelText.text = "치명타 확률 증가 Lv." + CRTLevel;*/

        // 배율 버튼 상태 업데이트
        x1Button.interactable = selectedMultiplier != 1;
        x10Button.interactable = selectedMultiplier != 10;
        x100Button.interactable = selectedMultiplier != 100;
    }
}