using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AOESkillController : SkillObjectController
{
    Character character;
    private Dictionary<GameObject, Coroutine> damagingMonsters = new Dictionary<GameObject, Coroutine>();
    private WaitForSeconds interval;
    private float TicDamage;
    private float EndDamage;
    protected override void Awake()
    {
        base.Awake();
        character = GetComponentInParent<Character>();
        interval = new WaitForSeconds(0.5f);
        TicDamage = skill.Data.TicDamageMultiplier;
        EndDamage = skill.Data.EndDamageMultiplier;
    }
    protected override void ExecuteSkill()
    {
        if(character.Target != null)
        {
            transform.position = character.Target.position;
        }
    }

    protected override void MoveSkill()
    {
        if (character.Target != null)
        {
            transform.position = Vector3.Lerp(transform.position, character.Target.position, 0.05f);
        }
    
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsLayerMatched(TargetCollisionLayer.value, collision.gameObject.layer))
        {
            //collision.gameObject.GetComponent<>
            if (!damagingMonsters.ContainsKey(collision.gameObject))
            {
                Coroutine damageCoroutine = StartCoroutine(ApplyDamageOverTime(collision.gameObject));
                damagingMonsters.Add(collision.gameObject, damageCoroutine);
            }
        }
    }
    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.activeSelf) return;
        if (IsLayerMatched(TargetCollisionLayer.value, collision.gameObject.layer))
        {
           StopCoroutine(damagingMonsters[collision.gameObject]);
           //SoundManager.PlayFx(SoundFx.SkillHit);
        }
    }
    /* 스킬 도중에 나갔다 들어 왔을 때와,스킬 시전 도중에 들어왔을 상황에서 대처할 코드를 추가해야한다*/
    private IEnumerator ApplyDamageOverTime(GameObject mob)
    {
        int count = 0;
        while (true)
        {
            IDamagable damagable = mob.GetComponent<IDamagable>();
            if(count == 10)
            {
                damagable?.TakeDamage((int)(EndDamage * SkillDamage(character, skill)));
                count = 0;
            }
            damagable?.TakeDamage((int)(TicDamage * SkillDamage(character, skill)));
            yield return interval;
            count++;
        }
    }
}
