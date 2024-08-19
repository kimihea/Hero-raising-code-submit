using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Build.Pipeline;
using UnityEditor.Experimental.GraphView;
using UnityEditor.U2D.Animation;
using UnityEditorInternal;
using UnityEngine;
public class EffectAnimationData
{
    string atkBuffName = "AtkBuff";
    string defBuffName = "DefBuff";
    string skillParameterName = "Skill";
    public int AtkBuffParameterHash { get; private set; }
    public int DefBuffParameterHash { get; private set; }
    public int SkillParameterHash { get; private set; }
    public void Initialize()
    {
        AtkBuffParameterHash = Animator.StringToHash(atkBuffName);
        DefBuffParameterHash = Animator.StringToHash(defBuffName);
        SkillParameterHash = Animator.StringToHash(skillParameterName);
    }
}

public class SkillController : MonoBehaviour
{
    [Header("스킬 애니메이션 제어")]
    public Character character;
    public BodyEffect BodyEffect; //casting도중 보여주는 이펙트
    AnimatorStateInfo state;

    [Header("스킬들 제어")]
    public List<Skill> SkillList;
    public List<WaitForSeconds> waitSkillMotionList = new List<WaitForSeconds>();
    public List<float> CoolDownList = new List<float>();
    //private List<SkillObjectController> SOC = new List<SkillObjectController>();

    public bool IsAuto;
    [Header("채널링 스킬 제어")]
    private int CSI; //Current Skill Index
    private float startTime;
    private float endTime;
    private bool IsSustain;
    private Coroutine channeling;
    public bool CanMove; //스킬을 사용하면서 움직이게 할려면 인스펙터에서 활성화
    
    private int i;
    public void Start()
    {

        UpdateSkillList(SkillList);
        IsSustain = false;
    }


    public void Update()
    {
        UpdateCooldowns();
        if (IsAuto)
        {

            AutoPlaySkill();
        }
        SustainAndMove(IsSustain, CanMove, startTime,endTime);
    }
    public void OnDisable()
    {
        IsSustain = false;
    }
   

    #region 스킬사용 메소드
    public void StartSkill(int index)
    {
        CSI = index;
        if (index >= SkillList.Count || !SkillList[index].IsCharge) return;
        if(GameManager.Instance.CombatConditionType == ECombatConditionType.START)
        {
            SkillList[index].IsCharge = false;
            CoolDownList[index] = SkillList[index].CoolTime;
            character.Controller.isChanneling = true;
            BodyEffect.StartEffect(BodyEffect.Data.SkillParameterHash);

            //(SkillList[index].Motion == ESkillMotion.MOTION1);
            character.StateMachine.ChangeState(SkillList[index].Motion == ESkillMotion.MOTION1 ? character.StateMachine.Attack03: character.StateMachine.Attack02);
            channeling=StartCoroutine(Casting(index));
        }
    }
    IEnumerator Casting(int index)
    {
        yield return waitSkillMotionList[index];//casting time
        UseSkill(index);
        //만약 계속 Channeling하는 스킬이라면 현재 state를 반복
        if (IsSustainChanneling(index))
        {
            IsSustain = true;
            startTime = SkillList[index].StartTime;
            endTime= SkillList[index].EndTime;
            yield return new WaitForSeconds(SkillList[index].Duration);
        }
        IsSustain = false;
        character.Controller.isChanneling = false;
        character.StateMachine.ChangeState(character.StateMachine.previousState);
    }
    
    public void UseSkill(int index)
    {
        //스킬 오브젝트를 활성화
        SkillList[index].SkillObj.SetActive(true);
    }
    public bool IsSustainChanneling(int index)
    {
        return (SkillList[index].IsSustainChanneling);

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
                    SkillList[i].IsCharge = true;
                }
            }

        }
    }
    public void AutoPlaySkill()
    {
        if (character.Controller.isChanneling) return;
        if(character.Target ==null) return;
        for (i = 0; i < SkillList.Count; i++)
        {
            if (SkillList[i].IsCharge&& (SkillList[i].Data.SkillRange >= DistanceToTarget(character.Target)))
            {
                StartSkill(i);
                return;
            }

        }
    }
    public void UpdateSkillList(List<Skill> skillList)
    {
        CoolDownList.Clear();
        waitSkillMotionList.Clear();
        foreach (Skill skill in skillList)
        {
            skill.IsCharge = true;
            CoolDownList.Add(0);//쿨다운이 0이면 스킬 사용 가능
            waitSkillMotionList.Add(new WaitForSeconds(skill.Data.ChannelingTime));
            //CashingObjectController(skill);
        }
    }
    //private void CashingObjectController(Skill skill)
    //{
    //        SkillObjectController soc = skill.SkillObj.GetComponent<SkillObjectController>();
    //        SOC.Add(soc);
    //}
    private void SustainAndMove(bool IsSustain, bool CanMove =false, float startTime = 0.5f,float endTime = 0.99f)
    {
        
        if (IsSustain)
        {
            /*StateInfo를 계속 받아오는 이유는 주소 참조가 아닌 값 참조형식이기 때문에 계속 받아와서 현재 진행상황을 받아와야한다..
            주소 참조인줄 알았는데 아니였다....*/
            state = character.Animator.GetCurrentAnimatorStateInfo(0);
            if (CanMove)
            {
                if(character.Target!=null)character.StateMachine.Pursuit.MoveTowardsTarget(character.Target.position);
            }
            if (state.normalizedTime >= endTime)
            {
                //startTime = Mathf.Clamp(startTime, 0, 1);   
                //character.Animator.Play("Attack03Knight", 0, startTime);
                character.Animator.Play(state.shortNameHash, 0, startTime);
            }
        }
    }
    #endregion
    public void ShutDownSkill(List<Skill> skillList)
    {
            InterruptChaneeling();

        foreach (Skill skill in skillList)
        {
            skill.SkillObj.GetComponent<SkillObjectController>().InterruptSkill();
            skill.SkillObj.SetActive(false);
        }
    }
    /// <summary>
    /// 코루틴이 중지될 때의 예외처리.
    /// 만약 채널링 도중이라면 채널링을 중지하며, object도 비활성화한다
    /// </summary>
    public void InterruptChaneeling()
    {
        if (channeling != null)
        {
            StopCoroutine(channeling);
            if (SkillList[CSI].IsSustainChanneling)
            {
                IsSustain = false;
            }
                character.StateMachine.ChangeState(character.StateMachine.Pursuit);
                character.Controller.isChanneling = false;
                SkillList[CSI].SkillObj.GetComponent<SkillObjectController>().InterruptSkill();
                SkillList[CSI].SkillObj.SetActive(false);
            
        }
    }

    #region 스킬 쿨타임 반환
    public float SkillCoolTimeAmount(int index)
    {        
        return Math.Clamp(CoolDownList[index]/SkillList[index].CoolTime,0f,1f);
    }
    #endregion
    public float DistanceToTarget(Transform Target)
    {
        return (character.transform.position - Target.position).magnitude;
    }
}
