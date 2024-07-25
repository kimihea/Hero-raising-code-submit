using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterController : Controller
{
    SpriteRenderer spriteRenderer;
    Color originalColor;
    [NonSerialized] public Character character;
    [NonSerialized] public HealthSystem healthSystem;
    //public List<Skill> SkillList;
    public event Action OnDeath;
    public event Action<int> OnDamage;
    public event Action OnAttack;
    public event Action OnHeal;

    public bool isHit;
    public bool isHeal;
    public bool isDead;
    public bool isChanneling;


    protected override void Awake() 
    {
        base.Awake();
        healthSystem = GetComponent<HealthSystem>();
        character = GetComponent<Character>();
        isDead = false;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }
    protected override void Update() 
    {
        base.Update();
        character.StateMachine.Update(); 
        //if (isHeal)
        //{ 
        //    CallHeal();
        //}
 
    }
    #region Hurt and Death Coroutine
    public override IEnumerator PlayHurtAnimationAndIdleCoroutine()
    {
        #region 경직on
        //if (currentHurtCoroutine != null) StopCoroutine(currentHurtCoroutine);
        //isHit = true;
        //character.Animator.SetTrigger(character.DataAnim.HurtParameterHash);
        //yield return hurtAnimLength;
        //isHit = false;
        #endregion
        #region 경직off
        spriteRenderer.color = Color.red;
        yield return hurtAnimLength;
        spriteRenderer.color = originalColor;
        #endregion
    }
    public override IEnumerator PlayDeathAnimationAndIdleCoroutine()
    {
        character.StateMachine.ChangeState(character.StateMachine.Death);
        yield return deadAnimLength;
        //if (character.EntityType == EEntityType.MONSTER) 
            gameObject.SetActive(false);
        //animator.enabled = false;
    }
    public void OnEnable()
    {
        //animator.enabled = true;
        isDead = false;
        spriteRenderer.color = originalColor;
    }
    #endregion
    #region Action CallBack
    public void CallDeath()
    {
        OnDeath?.Invoke();
        DeathAnim();        
    }
    public void CallDamage(int Damage)
    {
        if (!isDead)
        {
        DamagedAnim();
        OnDamage?.Invoke(Damage);
        }
    }
    public void CallAttack()
    {
        if (!isAttacking & !isDead)
        {
            ChooseAttackType();
            OnAttack?.Invoke();
        }
    }
    public void CallHeal()
    {
        OnHeal?.Invoke();
    }
    #endregion
    [ContextMenu("Play")]
    public void Fight()
    {
         character.StateMachine.ChangeState(character.StateMachine.Pursuit);
    }
    public void ChooseAttackType()
    {
        

    }

}

