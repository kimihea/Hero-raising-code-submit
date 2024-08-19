using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AOESkillController : SkillObjectController
{
    Character character;
    private Dictionary<GameObject, Coroutine> damagingMonsters;
    private WaitForSeconds interval;
    private float TicDamage;
    private float EndDamage;
    public bool IsAura;
    
    protected override void Awake()
    {
        base.Awake();
        character = GetComponentInParent<Character>();
        interval = new WaitForSeconds(skill.Data.Interval);
        TicDamage = skill.Data.TicDamageMultiplier;
        EndDamage = skill.Data.EndDamageMultiplier;
        damagingMonsters = new();
    }
    protected override void ExecuteSkill()
    {
        damagingMonsters.Clear();
        if (!IsAura && character.Target != null)
        {
            transform.position = character.Target.position;
        }
    }
    protected override void TerminateSkill()
    {
        damagingMonsters.Clear();
    }
    internal protected override void InterruptSkill()
    {
        //damagingMonsters.Clear();
    }
    protected override void MoveSkill()
    {
        if (GameManager.Instance.CombatConditionType != ECombatConditionType.START) return;
        if (!IsAura && character.Target != null)
        {
            transform.position += (character.Target.position - transform.position).normalized *skill.Data.SkillMoveSpeed* Time.deltaTime;
        }
    
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsLayerMatched(TargetCollisionLayer.value, collision.gameObject.layer))
        {
            if (!damagingMonsters.ContainsKey(collision.gameObject))
            {
                if (!this.isActiveAndEnabled) return;//어쩌다 버그 떳는데 다시 못찾아서 안전핀걸음
                Coroutine damageCoroutine = StartCoroutine(ApplyDamageOverTime(collision.gameObject));
                damagingMonsters.Add(collision.gameObject, damageCoroutine);
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        //if (!collision.gameObject.activeSelf) return;
        if (IsLayerMatched(TargetCollisionLayer.value, collision.gameObject.layer))
        {
            if (damagingMonsters.ContainsKey(collision.gameObject))
                StopCoroutine(damagingMonsters[collision.gameObject]);
           //SoundManager.PlayFx(SoundFx.SkillHit);
        }
    }
    private IEnumerator ApplyDamageOverTime(GameObject mob)
    {
        int count = 0;
        while (true)
        {
            //if (IsLayerMatched(TargetCollisionLayer.value, LayerMask.NameToLayer("NPC"))) //AOE형 버프? 만들다가 Heal스킬로 재정의함.
            //{
            //    CharacterController controller = mob.GetComponent<CharacterController>();
            //    if (count == 10)
            //    {
            //        controller.CallHeal(((int)(EndDamage * SkillDamage(character, skill))));
            //        count = 0;
            //    }
            //    controller.CallHeal(((int)(TicDamage * SkillDamage(character, skill))));
            //} 
            //else
            {
                IDamagable damagable = mob.GetComponent<IDamagable>();
                var (damage, isCritical) = character.Controller.CalculateDamage(SkillDamage(character, skill));

                if (count == 10) 
                {
                    damagable?.TakeDamage((int)(EndDamage*damage), isCritical);
                    count = 0;
                }
                else
                {
                    damagable?.TakeDamage((int)(TicDamage * damage), isCritical);
                }
            }
            yield return interval;
            count++;
        }
    }
}
