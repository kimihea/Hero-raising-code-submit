using DarkPixelRPGUI.Scripts.UI.Equipment;
using System;
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
    public SkillManager skillManager;
    public SkillInfo currentSkillInfo;
    public Button EquipBtn;
    public Button UpStarBtn;
    
    public GameObject SelectPanel;

    public Image selectedSkillIcon;
    public Text selectedSkillInfo;
    public Text selectedSkillStarInfo;



    private void Awake()
    {
        UpStarBtn.onClick.AddListener(UpStar);
        EquipBtn.onClick.AddListener(Equip);
    }
    public void GetSelectSkillInfo(int index)
    {
        currentSkillInfo = skillManager.GetSkillInfo(index);
    }
    public void SelectSkill(int index)
    {
        GetSelectSkillInfo(index);
        if (currentSkillInfo != null)
        {
            // UI 텍스트 업데이트
            selectedSkillIcon.sprite = currentSkillInfo.Icon;
            selectedSkillInfo.text = "재사용대기시간"+currentSkillInfo.CoolTime+"초\n"+ currentSkillInfo.Description;
            selectedSkillStarInfo.text = currentSkillInfo.Name + $": {currentSkillInfo.Stars}\n"+currentSkillInfo.PassiveEffect; 
        }
        else
        {
            // 스킬 정보가 없을 경우의 처리
            Debug.LogWarning("Skill info not found for index: " + index);
            // 기본값 설정 또는 UI 비활성화
            selectedSkillIcon.sprite = null;
            selectedSkillInfo.text = "No description available.";
            selectedSkillStarInfo.text = "N/A";
        }

    }
    public void UpStar()
    {
        Debug.Log("각성");
        //star를 받아서 검사한후 가능하면 Skill.UpStar()한다. 
    }
    public void Equip()
    {
        Debug.Log("장착");
        //SkillManager.EquipSKill(index)한다
    }
}
