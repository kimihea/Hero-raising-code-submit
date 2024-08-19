using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.TextCore.Text;
public interface IDamagable
{
    void TakeDamage(int value,bool critic); 
}

[RequireComponent(typeof(StatHandler))]
[RequireComponent(typeof(CharacterDamaged))]
[RequireComponent(typeof(StatHandler))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(HealthSystem))]
public abstract class Character :MonoBehaviour , IDamagable, IHandleBuff
{
    //public CharacterSO Data;

    public StatHandler StatHandler;
    public Transform Target;
    public CharacterAnimationData DataAnim;
    public CharacterStateMachine StateMachine;
    public CharacterController Controller;
    public LayerMask LayerMask;
    public EEntityType EntityType;
    public EEntityType TargetType;
    public Animator Animator;
    public BodyEffect BodyEffect;
    public int CurAtk => (int)StatHandler.curStat.Atk; 

    public HealthSystem Health { get; private set; }    
    // 스테이지 재 시작 시 캐릭터 생성 될 위치
    public Vector3 DefalutPos;
    protected List<Character> targetList = new List<Character>();
    private Dictionary<CharacterStat, Coroutine> activeBuffs;

    protected virtual void Awake()
    {        
        DataAnim.Initialize();
        Animator = GetComponentInChildren<Animator>();
        StatHandler = GetComponent<StatHandler>();
        Controller = GetComponent<CharacterController>();
        Health = GetComponent<HealthSystem>();
        StateMachine = new CharacterStateMachine(this);
        StateMachine.Initialize();
        StateMachine.ChangeState(StateMachine.Idle);
        //
    }
    protected virtual void Start()
    {
        
        StatHandler.UpdateStatModifier();
        activeBuffs = new();
    }
    protected virtual void OnEnable()
    {
        if(StateMachine != null) StateMachine.ChangeState(StateMachine.Idle);

    }
    public virtual void Update()
    {
        if(Target == null || !Target.gameObject.activeSelf)
        {
            FindTarget();
        }
    }

    public void InitStat()
    {
        StatHandler.UpdateStatModifier();
        Health.InitHealth(StatHandler.curStat.GetCurHealth());
    }

    public abstract void FindTarget();
    public abstract void SetTarget();
    public virtual void TakeDamage(int value, bool critic)
    {
        
        Controller.CallOnDamage(value,critic);
    }
    public void ActiveBuff(CharacterStat buffStat, float time, EBuffType Type)
    {
        switch (Type)
        {
            case EBuffType.ATK:
                BodyEffect.StartEffect(BodyEffect.Data.AtkBuffParameterHash);
                break;
            case EBuffType.DEF:
                BodyEffect.StartEffect(BodyEffect.Data.DefBuffParameterHash);
                break;
        }
        if (activeBuffs.ContainsKey(buffStat))
        {
            // 이미 버프 코루틴이 적용되어 있는 경우, 1)기존 코루틴을 중지하고 새로 시작--<중지만 하면 안되고, 2)/추가로 RemoveStatModifier실행 3)딕셔너리에서 제거
            /* 만약 캐릭터를 각성할 때 - 버프 스킬의 %도 증가하게 만들 경우 그에 대응하는 코드 필요(아닌가?)(버프 스킬의 구별을 string이나 RCode로) 
            현재는 계속 같은 %를 지닌 스킬만 사용함.*/
            StopCoroutine(activeBuffs[buffStat]); //1)
            StatHandler.RemoveStatModifier(buffStat); //2)
            activeBuffs.Remove(buffStat);//3)
            activeBuffs[buffStat] = StartCoroutine(BuffCoroutine(buffStat, time));
        }
        else
        {
            // 새로운 버프 추가
            Coroutine buffCoroutine = StartCoroutine(BuffCoroutine(buffStat, time));
            activeBuffs.Add(buffStat, buffCoroutine);

        }

    }
    IEnumerator BuffCoroutine(CharacterStat buffStat, float wait)
    {

        StatHandler.AddStatModifier(buffStat);
        yield return new WaitForSeconds(wait);

        StatHandler.RemoveStatModifier(buffStat);
        activeBuffs.Remove(buffStat);
    }
}

