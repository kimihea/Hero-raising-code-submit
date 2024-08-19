using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Controller : MonoBehaviour
{
    protected float attackDelay;
    public bool isAttacking;
    public WaitForSeconds hurtAnimLength;
    public WaitForSeconds deadAnimLength;
    protected Animator animator;
    protected Coroutine currentHurtCoroutine;
    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        isAttacking = false;
        hurtAnimLength = new WaitForSeconds(0.2f);
        deadAnimLength = new WaitForSeconds(0.8f);
        attackDelay = 0f;
    }
    protected virtual void Update() 
    {
        
    }
    public void Attack()
    {
        if (!isAttacking) StartCoroutine(CoAttack());

    }
    public void DamagedAnim()
    {
        if(isActiveAndEnabled==true)
            currentHurtCoroutine =  StartCoroutine(PlayHurtAnimationAndIdleCoroutine());
    }
    public void DeathAnim()
    {
        StartCoroutine(PlayDeathAnimationAndIdleCoroutine());
    }
    protected IEnumerator CoAttack()
    {
        //isAttacking = true;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        //isAttacking = false;
    }


    public  virtual IEnumerator PlayHurtAnimationAndIdleCoroutine()
    {

        yield return hurtAnimLength;

    }
    public virtual IEnumerator PlayDeathAnimationAndIdleCoroutine()
    {

        yield return deadAnimLength;
    }

}