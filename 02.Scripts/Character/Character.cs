using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.TextCore.Text;
public interface IDamagable
{
    void TakeDamage(int value); //지원님 코드랑 합칠때 수정부분
}

[RequireComponent(typeof(StatHandler))]
[RequireComponent(typeof(CharacterDamaged))]
[RequireComponent(typeof(StatHandler))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(HealthSystem))]
public abstract class Character :MonoBehaviour , IDamagable
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
    public int CurAtk => (int)StatHandler.curStat.Atk; //지원님 코드랑 합칠때 수정부분

    public HealthSystem Health { get; private set; }    
    // 스테이지 재 시작 시 캐릭터 생성 될 위치
    public Vector3 DefalutPos;
    protected List<Character> targetList = new List<Character>();

    protected virtual void Awake()
    {        
        DataAnim.Initialize();
        Animator = GetComponentInChildren<Animator>();
        StatHandler = GetComponent<StatHandler>();
        Controller = GetComponent<CharacterController>();
        Health = GetComponent<HealthSystem>();
        
    }
    protected virtual void Start()
    {
        StateMachine = new CharacterStateMachine(this);
        StateMachine.Initialize();
        StateMachine.ChangeState(StateMachine.Idle);        
        StatHandler.UpdateStatModifier();
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
        Health.InitHealth(StatHandler.curStat.Health);
    }

    protected float CalcDamage()
    {
        Debug.Log("스탯에 따른 데미지 최종 계산 메소드");

        return 0;
    }

    public abstract void FindTarget();
    public abstract void SetTarget();
    public virtual void TakeDamage(int value) //지원님 코드랑 합칠때 수정부분
    {
        Controller.CallDamage(value);
    }

}

