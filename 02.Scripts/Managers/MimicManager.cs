using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MimicSaveData
{
    public int mimicLevel;
    public int drawCount;
    public int drawPerLevel;
}

public class MimicManager : Singleton<MimicManager>
{
    public Text MimicLevelText;         // Mimic의 레벨을 나타내는 Text
    public GameObject[] MimicObject;    // 각 레벨의 미믹 오브젝트 배열
    public GameObject[] MimicBtnObject; // 각 레벨의 미믹 버튼 오브젝트 배열

    private int mimicLevel = 1;     // 초기 Mimic 레벨
    private int drawCount = 0;      // 장비 뽑기 횟수
    private int drawPerLevel = 10;   // 레벨업에 필요한 장비 뽑기 횟수 (유지보수를 위해 변수로 설정)

    public MimicSaveData MimicSaveData = new();

    public EquipmentGacha EquipmentGacha;
    //// 싱글톤 인스턴스
    //public static MimicManager Instance { get; private set; }

    //private void Awake()
    //{
    //    if(Instance == null)
    //    {
    //        Instance = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    void Start()
    {
        MimicLoadData();

        UpdateMimicLevelText(); // 초기 텍스트 업데이트
        UpdateMimicObject();    // 초기 미믹 오브젝트 업데이트
    }
    public void AddDrawCount()
    {
        drawCount++;
        UpdateMimicLevel();
        UpdateMimicLevelText();
    }

    private void UpdateMimicLevel()
    {
        int newLevel = (drawCount /  drawPerLevel) +1;

        newLevel = newLevel > MimicObject.Length ? MimicObject.Length : newLevel;

        if (newLevel >= mimicLevel && newLevel <= MimicObject.Length)
        {
            mimicLevel = newLevel;

            //Debug.Log(EquipmentGacha);

            EquipmentGacha.tierTable.tableLevel = mimicLevel;
            EquipmentGacha.UpdateProbability();

            UpdateMimicObject();
        }
    }

    private void UpdateMimicObject()
    {
        for (int i = 0; i < MimicObject.Length; i++)
        {
            MimicObject[i].SetActive(i == mimicLevel - 1);
        }

        for (int i = 0; i < MimicBtnObject.Length; i++)
        {
            bool isActive = i == mimicLevel - 1;
            MimicBtnObject[i].SetActive(i == mimicLevel - 1);
            
            if (MimicBtnObject[i].activeSelf)
            {
                if (!Equipment.isGachaPossible)
                {
                    return;     
                }
                MimicAnimationController animationController = MimicBtnObject[i].GetComponent<MimicAnimationController>();
                if (animationController != null)
                {
                    animationController.SetTrigger(i);
                }
            }
        }
    }

    private void UpdateMimicLevelText()
    {
        if (MimicLevelText != null)
        {
            MimicLevelText.text = $"Lv. {mimicLevel}   Mimic Name";
        }
    }

    public void SaveData()
    {
        MimicSaveData.mimicLevel = mimicLevel;
        MimicSaveData.drawCount = drawCount;
        MimicSaveData.drawPerLevel = drawPerLevel;
    }

    private void MimicLoadData()
    {
        MimicSaveData loadData = DataManager.Instance.LoadData<MimicSaveData>(ESaveType.MIMIC);

        if (loadData == null)
        {
            return;
        }
        else
        {
            mimicLevel = loadData.mimicLevel;
            drawCount = loadData.drawCount;
            drawPerLevel = loadData.drawPerLevel;

            CurrencyManager.Instance.totalManaStonesConsumed = drawCount;
            CurrencyManager.Instance.UpdateAteManaStoneText();

            UpdateMimicLevel();
        }

    }
}
