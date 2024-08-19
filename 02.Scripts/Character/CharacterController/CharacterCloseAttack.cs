using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.TextCore.Text;
public class CharacterCloseAttack : MonoBehaviour
{
    public WaitForSeconds AttackReady;
    public Transform meleePos;
    public Vector2 boxSize;
    [SerializeField]protected CharacterController characterController;
    public CharacterStateMachine stateMachine;
    public Animator MyAnimator;
    public float timeSinceLastAttack;
    public float AttackReadyTime;
    float attackRange => characterController.character.StatHandler.curStat.AttackRange;
    float CurAtk => characterController.character.StatHandler.curStat.Atk;
    protected float CurAs => characterController.character.StatHandler.curStat.AttackSpeed;
    protected float CurAsMul => characterController.character.StatHandler.curStat.AttackSpeedMultiplier;
    public float CriticalChance=0f;
    public virtual void  Awake()
    {
        characterController  = GetComponentInParent<CharacterController>();
        timeSinceLastAttack = 0f;
        AttackReady = new WaitForSeconds(AttackReadyTime == 0f ? 0.85f : AttackReadyTime);
        MyAnimator=GetComponent<Animator>();


    }
    public virtual void Start()
    {
        stateMachine = characterController.character.StateMachine;
    }
    public void FixedUpdate()
    {
        CriticalChance= characterController.character.StatHandler.curStat.GetCurCriticalInfo().CriRate;
        //크리티컬적용확인용
    }
    public void OnDisable()
    {
        characterController.OnAttack -= OnAttack;
        //characterController.OnAttackSpeedChange -= ChangeAttackMotionSpeed;

    }
    public void OnEnable()
    {
        characterController.OnAttack += OnAttack;
        //characterController.OnAttackSpeedChange += ChangeAttackMotionSpeed;
    }

    public void Update()
    {
        if(timeSinceLastAttack > 1 / (CurAs + CurAsMul))
        {
            characterController.isAttacking = false;
        }
        timeSinceLastAttack += Time.deltaTime;
    }
    private void OnAttack()
    {
        ChangeAttackMotionSpeed();
        if (characterController.character.Animator.GetBool(characterController.character.DataAnim.Attack01ParameterHash))
        {
            timeSinceLastAttack = 0f;
            characterController.isAttacking = true;
            StartCoroutine(CloseAttack());
           /*공격 동작(준비/공격/마무리)은 시작했는데,준비동작에서 데미지를 입히면 이상하니까 
            공격동작에서 데미지를 입히게끔 코루틴을 사용하였음*/
        }
        
    }
    private IEnumerator CloseAttack()
    {
        if (characterController.character.Target != null)
            MoveMeleePos(characterController.character.Target.position, attackRange);
        yield return AttackReady;
        if (!characterController.isDead)
        {
            Collider2D[] coliders2Ds = Physics2D.OverlapBoxAll(meleePos.position, boxSize,0, characterController.character.LayerMask);
            foreach (Collider2D colider in coliders2Ds)
            {
                if (colider.GetComponent<Character>().EntityType == characterController.character.TargetType)
                {
                    IDamagable damagable = colider.GetComponent<IDamagable>();
                    var (damage, isCritical) = characterController.CalculateDamage(characterController.character.StatHandler.curStat.GetCurAtk());
                    damagable?.TakeDamage(damage, isCritical);
                }
            }
        }
    }

    /// <summary>
    /// 타겟위치로 근접공격 위치 재설정
    /// </summary>
    /// <param name="target"></param>
    private void MoveMeleePos(Vector3 target ,float range =1f)
    {
        Vector3 direction = Vector3.Normalize(target - characterController.transform.position);
        Vector3 newMeleePos = direction * range;
        meleePos.localPosition = newMeleePos;
        characterController.FlipCharacter(target);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(meleePos.position, boxSize);
    }
    private void SetAttackMotionSpeed(float attackSpeed=1f)
    {
        //MyAnimator.SetFloat(Animator.StringToHash("CurAttackMotionSpeed"), attackSpeed);

        MyAnimator.SetFloat(stateMachine.Character.DataAnim.CurAttackMotionSpeedParameterHash, attackSpeed);
    }
    public void ChangeAttackMotionSpeed()
    {
        //Debug.Log(CurAs + "/" + CurAsMul);
        SetAttackMotionSpeed(CurAs+ CurAsMul);//애니메이션 빠르게
        AttackReady = new WaitForSeconds(AttackReadyTime * (1 / (CurAs + CurAsMul)))
        ; //공격 적용시점도 빠르게
    }
}
