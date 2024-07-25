using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class SkillController : MonoBehaviour
{
    public Character character;
    public List<Skill> SkillList;
    protected List<WaitForSeconds> waitSkillMotionList = new List<WaitForSeconds>();
    public List<float> CoolDownList = new List<float>();
    public bool IsAuto;

    private int i;
    public void Start()
    {
        UpdateSkillList(SkillList);
    }
    
    public void Update()
    {
        UpdateCooldowns();
        if(IsAuto) AutoPlaySkill();
       
    }
    #region 스킬사용 메소드
    public void StartSkill(int index)
    {
        if(GameManager.Instance.CombatConditionType == ECombatConditionType.START)
        {
            SkillList[index].IsCharge = false;
            character.Controller.isChanneling = true;
            character.StateMachine.ChangeState(character.StateMachine.Attack03);
            StartCoroutine(Casting(index));
        }
    }
    IEnumerator Casting(int index)
    {
        yield return waitSkillMotionList[index];
        UseSkill(index);
        character.Controller.isChanneling = false;
        character.StateMachine.ChangeState(character.StateMachine.previousState);
    }
    
    public void UseSkill(int index)
    {
        SkillList[index].SkillObj.SetActive(true);
    }
    #endregion
    #region 업데이트 관련 메소드
    public void UpdateCooldowns()
    {
         for (i = 0; i<SkillList.Count; i++)
        {
            if (!SkillList[i].IsCharge)
            {
                CoolDownList[i] -= Time.deltaTime;
                if(CoolDownList[i] <= 0)
                {
                    CoolDownList[i] = SkillList[i].CoolTime;
                    SkillList[i].IsCharge = true;
                }
            }

        }
    }
    public void AutoPlaySkill()
    {
        if (character.Controller.isChanneling) return;
        for (i = 0; i < SkillList.Count; i++)
        {
            if (SkillList[i].IsCharge)
            {
                StartSkill(i);
                return;
            }

        }
    }
    private void UpdateSkillList(List<Skill> skillList)
    {
        CoolDownList.Clear();
        waitSkillMotionList.Clear();
        foreach (Skill skill in skillList)
        {
            skill.IsCharge = true;
            CoolDownList.Add(skill.CoolTime);
            waitSkillMotionList.Add(new WaitForSeconds(skill.Data.ChannelingTime));
        }
    }
    #endregion
}
