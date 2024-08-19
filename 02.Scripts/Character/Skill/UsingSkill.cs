using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsingSkill : MonoBehaviour
{
    public int[] skillIndexs = new int[3];
    public List<Image> SImages= new List<Image>();
    public Image[] CoverImages = new Image[3];
    private SkillController skillController;
    List<Sprite> sprites = new();
    public Button[] Buttons = new Button[3];
    public bool Clickable;
    public void Start()
    {
        skillController = SkillManager.Instance.PlayerSkillController;
        
        for (int i = 0; i < 3; i++)
        {
            int capturedIndex = i;
            if (Buttons[i] != null)
            {
                Buttons[i].onClick.AddListener(() => OnImageClicked(capturedIndex));
            }
        }

    }
    public void Update()
    {
        if(Clickable)
            SetCoolTimeCover();
    }
    /// <summary>
    /// 스킬매니저로부터 sprite를 받아오는 함수,매개변수로 true를 주면 일시적으로 바뀐 이미지를 보여준다
    /// </summary>
    /// <param name="temp"></param>
    public void UpdateImage(bool temp=false)
    {
        skillIndexs = temp? SkillManager.Instance.TempSkillIndex: SkillManager.Instance.PSkillIndex; //현재 들어간 스킬의 index
        sprites = SkillManager.Instance.HeroIdToSprite(0);//모든 스킬의 sprite를 가져온다, sprite 리스트는 정렬되어 있다.

        for (int i = 0; i < 3; i++)
        {
            if (skillIndexs[i] != 99)
            {
                SImages[i].sprite = sprites[skillIndexs[i]];
            }
            else
            {
                SImages[i].sprite = SkillManager.Instance.DefalutSprite;
            }
        }
    }
    public void OnImageClicked(int index)
    {
        skillController.StartSkill(index);
    }
    public void OnClickAuto()
    {
        skillController.IsAuto = !skillController.IsAuto;
    }
    public void SetCoolTimeCover()//스킬이 중간에 비어있는 경우 대응하지 못함
    {
        int index=0;//index를 새로 추가해서 관리
        for (int i = 0; i < 3; i++)
        {

            if (skillIndexs[i] != 99 && CoverImages[i] != null)//스킬이 비어있지 않으면
            {
                CoverImages[i].fillAmount = skillController.SkillCoolTimeAmount(index);
                index++;
            }
            else if(skillIndexs[i] == 99 && CoverImages[i] != null)
                CoverImages[i].fillAmount = 1;
        }
    }
}

