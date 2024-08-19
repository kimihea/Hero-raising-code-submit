using DarkPixelRPGUI.Scripts.UI.Equipment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class SkillInfo
{
    public Sprite Icon { get; set; }
    public string Name { get; set; }
    public string CoolTime { get; set; }
    public string Description { get; set; }
    public string PassiveEffect { get; set; }
    public int Stars { get; set; }
    public int Count { get; set; }
}
public class SkillMenu : MonoBehaviour
{
    public Button EquipBtn;
    public Button UnEquipBtn;
    public Button UpStarBtn;
    public Button ApplyBtn;


    public GameObject SelectPanel;
    public SkillInfo currentSkillInfo;
    public Skill curSkill;
    public int curSkillIndex;
    public Image selectedSkillIcon;
    public Text selectedSkillInfo;
    public Text selectedSkillStarInfo;
    public Image selectedSkillCounter;
    public Text selectedSkillCounterTxt;

    public List<SkillSlot> slots;
    public bool[] IsLockArray = new bool[8];

    public GameObject NavigateToDungeon;
    public GameObject DungeonMenu;
    private Coroutine NavigateCO;
    private WaitForSecondsRealtime waitRead;
    private void Awake()
    {
        UpStarBtn.onClick.AddListener(UpStar);
        EquipBtn.onClick.AddListener(Equip);
        UnEquipBtn.onClick.AddListener(UnEquip);
        ApplyBtn.onClick.AddListener(Apply);
        waitRead = new WaitForSecondsRealtime(3f);
    }
    private void OnEnable()
    {
        //스킬창이 열릴 때마다 해줄 일들
        UpdateSlotCount(); //1)스킬 count변동사항 체크.
        //2) 사운드?
        NavigateToDungeon.SetActive(false);//3)던전이동창 꺼주기?-켜주는 곳에서 해야할듯.
    }
    public void GetSelectSkillInfo(int index)
    {
        curSkillIndex = index;
        curSkill = SkillManager.Instance.IndexToPlayerSkill(index);
        currentSkillInfo = SkillManager.Instance.GetPlayerSkillInfo(index);
    }
    public void SelectSkill(int index)
    {
        GetSelectSkillInfo(index);
        if (currentSkillInfo != null)
        {
            // UI 텍스트 업데이트
            selectedSkillIcon.sprite = currentSkillInfo.Icon;
            selectedSkillInfo.text = "재사용 대기시간"+currentSkillInfo.CoolTime+": 초\n"+ currentSkillInfo.Description;
            selectedSkillStarInfo.text = currentSkillInfo.Name+ "\n" + new string('★', currentSkillInfo.Stars)+"\n"+currentSkillInfo.PassiveEffect ;
            selectedSkillCounter.fillAmount = (float)curSkill.Count / 20;
            selectedSkillCounterTxt.text = $"{curSkill.Count}/ 20";
        }
        else
        {
            // 스킬 정보가 없을 경우의 처리
            Debug.LogWarning("Skill info not found for index: " + index);
            // 기본값 설정 또는 UI 비활성화
            selectedSkillIcon.sprite = null;
            selectedSkillInfo.text = "No description available.";
            selectedSkillStarInfo.text = "N/A";
            selectedSkillCounter.fillAmount = 0;
        }

    }
    public void UpStar()
    {
        if(curSkill.Count >= 20 && curSkill.Stars<5)
        {
            //Debug.Log("각성가능");
            //각성 판넬 띄우고 각성 하겠습니까?
            curSkill.Count = 0;
            curSkill.Stars++;//스타 1업
            SelectSkill(curSkillIndex);
            UpdateSlotCount();
        }
        else 
        {
            if(curSkill.Stars == 5)//별 다섯개
            {
                //Debug.Log("각성불가, 최고 각성단계");
            }
            else
            {
                //Debug.Log("각성불가, 스킬 스크롤이 충분하지 않음");
            }

        }
 
    }
    public void Equip()
    {
        if(SkillManager.Instance.EquipSkill(curSkillIndex))
             UpdateEquipUI(curSkillIndex, true);
    }
    public void UnEquip()
    {
        if(SkillManager.Instance.UnEquipSkill(curSkillIndex))
            UpdateEquipUI(curSkillIndex, false);
    }
    public void Apply()
    {
        SkillManager.Instance.ApplySkill();
    }

    public void UpdateSlotCount() //count업데이트
    {
        foreach(SkillSlot slot in slots)
        {
            slot.UpdateCount();
        }
    }
    public void UpdateEquipUI(int index, bool active)
    {
        if (index == 99) return;
        slots[index].IsEquip.SetActive(active);
    }
    public void LoadEquip(int[] PSkillIndex)
    {
        foreach( int index in PSkillIndex)
        {
            UpdateEquipUI(index, true);
        }
    }
    //스테이지한테서 스테이지 정보를 받아오고  스킬 하나씩 해금 되게
    public void UnLockSkill() 
    {
        
    }

    internal void MoveToAwakeDungeon()
    {
        NavigateToDungeon.SetActive(true);
        if (NavigateCO != null) StopCoroutine(NavigateCO);
        NavigateCO = StartCoroutine(NaviCo());
    }
    
    public void OnYesButtonClicked()
    {
        DungeonMenu.SetActive(true);
        NavigateToDungeon.SetActive(false);
    }
    public void OnNoButtonClicked()
    {
        NavigateToDungeon.SetActive(false);
    }
    public IEnumerator NaviCo()
    {
        yield return waitRead;
        NavigateToDungeon.SetActive(false);
    }
    public void SlotSaveData()
    {
        for (int i = 0; i < slots.Count; i++)
            IsLockArray[i] = slots[i].IsLock;
    }
    public void SlotLoadData()
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].IsLock = IsLockArray[i];
    }
}
