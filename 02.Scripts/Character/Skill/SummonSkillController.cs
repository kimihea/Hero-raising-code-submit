using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SummonSkillController : SkillObjectController
{
    public Character character;
    private float interval;
    private Transform originalParent;
    public string RCode;
    protected override void Awake()
    {
        base.Awake();
        originalParent = transform.parent;
    }
    protected override void ExecuteSkill()
    {
        if (GameManager.Instance.CombatConditionType != ECombatConditionType.START) return;
        transform.position = new Vector3(character.Target.position.x, 7f, 0);
        transform.SetParent(null);
        interval = float.MinValue;
    }

    protected override void MoveSkill()
    {
        if (GameManager.Instance.CombatConditionType != ECombatConditionType.START) return;
        if(character.Target != null)
            transform.position += new Vector3((character.Target.position.x - transform.position.x) * skill.Data.SkillMoveSpeed*Time.deltaTime,0,0);
        if (interval <= 0)
        {
            GameObject p = PoolManager.Instance.SpawnFromPool(RCode);

            p.transform.position = transform.position;
            p.transform.position += new Vector3(Random.Range(-0.2f, 0.3f), 0, 0);
            var (damage, isCritical) = character.Controller.CalculateDamage(SkillDamage(character, skill));
            p.GetComponent<ProjectileController>().Initialize(Vector3.down, 180, damage,isCritical);
            interval = skill.Data.Interval;
        }
        interval -= Time.deltaTime;
    }
    protected override void TerminateSkill()
    {
        base.TerminateSkill();
        transform.SetParent(originalParent);
    }
    protected internal override void InterruptSkill()
    {
        base.InterruptSkill();
        gameObject.transform.SetParent(originalParent);
    }
}
