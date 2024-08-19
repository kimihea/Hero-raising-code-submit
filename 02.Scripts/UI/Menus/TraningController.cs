using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
    public List<TextMeshProUGUI> StatText = new();
    public List<Text>  EnhanceInofoText = new();
    public List<Button> buttons;
    #region 스탯
    // 공격력 증가
    public Button STREnhanceButton;
    public TextMeshProUGUI STRCostText;
    public Text STRCurrentLevelText;

    // 체력 증가
    public Button HPEnhanceButton;
    public TextMeshProUGUI HPCostText;
    public Text HPCurrentLevelText;

    //공격속도증가
    public Button ATKSpeedEnhanceButton;
    public TextMeshProUGUI ATKSppedCostText;
    public Text ATKSpeedCurrentLevelText;

    // 방어력 증가
    public Button DEFEnhanceButton;
    public TextMeshProUGUI DEFCostText;
    public Text DEFCurrentLevelText;

    // 치명타 확률 증가
    public Button CRTEnhanceButton;
    public TextMeshProUGUI CRTCostText;
    public Text CRTCurrentLevelText;
    //치피증
    public Button CRMEnhanceButton;
    public TextMeshProUGUI CRMCostText;
    public Text CRMCurrentLevelText;
    #endregion
    #region
    // 초기 레벨과 비용
    private int STRLevel = 1;
    private int STRCost = 100;

    private int HPLevel = 1;
    private int HPCost = 100;

    private int ATKSpeedLevel = 1;
    private int ATKSpeedCost = 100;

    private int DEFLevel = 1;
    private int DEFCost = 100;

    private int CRTLevel = 1;
    private int CRTCost = 100;

    private int CRMLevel = 1;
    private int CRMCost = 100;
    private bool isEnhancing = false; // 스탯 강화가 진행 중인지 여부
    //public float enhancementInterval = 0.05f; // 강화 간격 (초 단위)
    private WaitForSecondsRealtime upgradeInterval = new WaitForSecondsRealtime(0.1f);
    private WaitForSecondsRealtime waitPressing = new WaitForSecondsRealtime(0.5f);

    // 선택된 배율을 저장할 변수
    private int selectedMultiplier = 1;
    #endregion

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
        int TempIndex=0;
        
        foreach (var button in buttons)
        {
            button.onClick.RemoveAllListeners();
            int currentIndex = TempIndex;
            var buttonEventTrigger = button.gameObject.AddComponent<EventTrigger>();

            var pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data, currentIndex); });

            var pointerUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data, currentIndex); });

            buttonEventTrigger.triggers.Add(pointerDownEntry);
            buttonEventTrigger.triggers.Add(pointerUpEntry);
            TempIndex++;
        }
        // 강화 버튼 클릭 시 해당 함수 호출
        STREnhanceButton.onClick.AddListener(() => StatManager.Instance.StatLevelUp((int)EStatType.ATK));
        STREnhanceButton.onClick.AddListener(() => UpdateUI());

        HPEnhanceButton.onClick.AddListener(() => StatManager.Instance.StatLevelUp((int)EStatType.HEALTH));
        HPEnhanceButton.onClick.AddListener(() => UpdateUI());

        DEFEnhanceButton.onClick.AddListener(() => StatManager.Instance.StatLevelUp((int)EStatType.DEFENSE));
        DEFEnhanceButton.onClick.AddListener(() => UpdateUI());

        ATKSpeedEnhanceButton.onClick.AddListener(() => StatManager.Instance.StatLevelUp((int)EStatType.ATKSPEED));
        ATKSpeedEnhanceButton.onClick.AddListener(() => UpdateUI());

        CRTEnhanceButton.onClick.AddListener(() => StatManager.Instance.StatLevelUp((int)EStatType.CRITRATE));
        CRTEnhanceButton.onClick.AddListener(() => UpdateUI());

        CRMEnhanceButton.onClick.AddListener(() => StatManager.Instance.StatLevelUp((int)EStatType.CRITMULTIPLIER));
        CRMEnhanceButton.onClick.AddListener(() => UpdateUI());
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
        else if (statType == "AS")
        {
            ATKSpeedLevel += selectedMultiplier;
            ATKSpeedCost += 50 * selectedMultiplier;
        }
        else if (statType == "DEF")
        {
            DEFLevel += selectedMultiplier;
            DEFCost += 50 * selectedMultiplier;
        }
        else if (statType == "CRT")
        {
            CRTLevel += selectedMultiplier;
            CRTCost += 50 * selectedMultiplier;
        }
        else if (statType == "CRM")
        {
            CRMLevel += selectedMultiplier;
            CRMCost += 50 * selectedMultiplier;
        }
        UpdateUI();
    }

    // UI를 업데이트하는 메서드
    private void UpdateUI()
    {
        if (StatManager.Instance.Stats.Count > 0)
        {
            EStatType[] statTypes = {
        EStatType.ATK,
        EStatType.HEALTH,
        EStatType.ATKSPEED,
        EStatType.DEFENSE,
        EStatType.CRITRATE,
        EStatType.CRITMULTIPLIER//그 원래 코드가 반복문 없이 땡으로 박는거여서 임시 조치했습니다. 아 그 하다가 오류 날까봐 일단 안전하게 했습니다.
    };

            TextMeshProUGUI[] costTexts = {
        STRCostText,
        HPCostText,
        ATKSppedCostText,
        DEFCostText,
        CRTCostText,
        CRMCostText
    };

            Text[] levelTexts = {
        STRCurrentLevelText,
        HPCurrentLevelText,
        ATKSpeedCurrentLevelText,
        DEFCurrentLevelText,
        CRTCurrentLevelText,
        CRMCurrentLevelText
    };
            //STRCostText.text = new BigInteger(StatManager.Instance.Stats[(int)EStatType.ATK].totalCost).ToAbbreviatedString();
            //STRCurrentLevelText.text = $"{StatManager.Instance.Stats[(int)EStatType.ATK].statDescription} Lv. {StatManager.Instance.Stats[(int)EStatType.ATK].statLevel}";

            //HPCostText.text = new BigInteger(StatManager.Instance.Stats[(int)EStatType.HEALTH].totalCost).ToAbbreviatedString();
            //HPCurrentLevelText.text = $"{StatManager.Instance.Stats[(int)EStatType.HEALTH].statDescription} Lv. {StatManager.Instance.Stats[(int)EStatType.HEALTH].statLevel}";

            //ATKSppedCostText.text = new BigInteger(StatManager.Instance.Stats[(int)EStatType.ATKSPEED].totalCost).ToAbbreviatedString();
            //ATKSpeedCurrentLevelText.text = $"{StatManager.Instance.Stats[(int)EStatType.ATKSPEED].statDescription} Lv. {StatManager.Instance.Stats[(int)EStatType.ATKSPEED].statLevel}";

            //DEFCostText.text = new BigInteger(StatManager.Instance.Stats[(int)EStatType.DEFENSE].totalCost).ToAbbreviatedString();
            //DEFCurrentLevelText.text = $"{StatManager.Instance.Stats[(int)EStatType.DEFENSE].statDescription} Lv. {StatManager.Instance.Stats[(int)EStatType.DEFENSE].statLevel}";

            //CRTCostText.text = new BigInteger(StatManager.Instance.Stats[(int)EStatType.CRITRATE].totalCost).ToAbbreviatedString();
            //CRTCurrentLevelText.text = $"{StatManager.Instance.Stats[(int)EStatType.CRITRATE].statDescription} Lv. {StatManager.Instance.Stats[(int)EStatType.CRITRATE].statLevel}";
            //CRMCostText.text = new BigInteger(StatManager.Instance.Stats[(int)EStatType.CRITMULTIPLIER].totalCost).ToAbbreviatedString();
            //CRMCurrentLevelText.text = $"{StatManager.Instance.Stats[(int)EStatType.CRITMULTIPLIER].statDescription} Lv. {StatManager.Instance.Stats[(int)EStatType.CRITMULTIPLIER].statLevel:D4)}";
            for (int i = 0; i < statTypes.Length; i++)
            {
                var stat = StatManager.Instance.Stats[(int)statTypes[i]];
                costTexts[i].text = new BigInteger(stat.totalCost).ToAbbreviatedString();

                int levelLength = stat.statLevel.ToString().Length;
                int padding = 4 - levelLength; // 최대 자리 수에서 현재 자리 수를 뺀 값을 패딩으로 사용
                levelTexts[i].text = $"{stat.statDescription}" ;
                EnhanceInofoText[i].text =  $"Lv.{stat.statLevel.ToString().PadLeft(padding + levelLength)} --> Lv.{(stat.statLevel+ selectedMultiplier).ToString().PadLeft(padding + levelLength)}";
                //levelTexts[i].text = $"{stat.statDescription} Lv. {stat.statLevel.ToString().PadLeft(padding + levelLength)}";
            }
            for (int i = 0; i < StatText.Count; i++)
            {
                SetClaimTextColor(StatText[i],i);
            }


            // 강화 비용 텍스트 업데이트
            /*STRCostText.countText = STRCost + " G";

            // 레벨 텍스트 업데이트
            STRCurrentLevelText.countText = "공격력 증가 Lv." + STRLevel;


            HPCostText.countText = HPCost + " G";
            HPCurrentLevelText.countText = "체력 증가 Lv." + HPLevel;

            HPRCostText.countText = ATKSpeedCost + " G";
            HPRCurrentLevelText.countText = "체력 재생 증가 Lv." + ATKSpeedLevel;

            DEFCostText.countText = DEFCost + " G";
            DEFCurrentLevelText.countText = "방어력 증가 Lv." + DEFLevel;

            CRTCostText.countText = CRTCost + " G";
            CRTCurrentLevelText.countText = "치명타 확률 증가 Lv." + CRTLevel;*/

            // 배율 버튼 상태 업데이트
            x1Button.interactable = selectedMultiplier != 1;
            x10Button.interactable = selectedMultiplier != 10;
            x100Button.interactable = selectedMultiplier != 100;
        } 
    }
    public void SetClaimTextColor(TextMeshProUGUI target,int index)
    {
        target.color = StatManager.Instance.Stats[index].totalCost <= CurrencyManager.Instance.CurrencyDict[ECurrencyType.Gold].Amount ? Color.white : Color.red;
    }
    private IEnumerator EnhanceStrengthRoutine(int index)
    {
        isEnhancing = true;
        yield return waitPressing;
        while (isEnhancing)
        {
            yield return upgradeInterval; // 0.05초 대기
            StatManager.Instance.StatLevelUp(index);
            UpdateUI();
        }
    }
    void OnPointerDown(PointerEventData eventData,int index)
    {
        if (!isEnhancing)
        {
            StartCoroutine(EnhanceStrengthRoutine(index));
        }
    }

    void OnPointerUp(PointerEventData eventData,int index)
    {
        StopCoroutine(EnhanceStrengthRoutine(index));
        isEnhancing = false;
    }
}