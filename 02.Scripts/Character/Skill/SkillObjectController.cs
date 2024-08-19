using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;
public abstract class SkillObjectController : MonoBehaviour
{
    [SerializeField] protected float currentDuration;
    public Skill skill;
    [SerializeField] protected LayerMask WallCollisionLayer;
    [SerializeField] protected LayerMask TargetCollisionLayer;
    protected Func<SkillSO, int, float> getDamageMultiplier = (data, stars) =>
    {
        return (data.DamageMultiplier.DefaultValue +
               data.DamageMultiplier.ModifierPerGrade * stars) / 100f;
    };

    /// <summary>
    /// update문에서 실행됩니다.
    /// </summary>
    protected abstract void MoveSkill();

    /// <summary>
    /// 오브젝트가 활성화되면 실행됩니다.
    /// </summary>
    protected abstract void ExecuteSkill();

    /// <summary>
    /// 스킬이 지속시간이 정상적으로 끝날때  해줄 작업
    /// </summary>
    protected virtual void TerminateSkill()
    {
        //종료되기 직전에  실행
    }
    /// <summary>
    /// 스킬이 중간에 종료될 때 해줄 작업
    /// </summary>
    internal protected virtual void InterruptSkill()
    {

    }
    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

    }
    protected void OnEnable()
    {
        ExecuteSkill();
        currentDuration = 0f;
    }
    protected void Update()
    {
        MoveSkill();
        UpdateDuration();
    }
    protected int SkillDamage(Character character, Skill skill)
    {
        return (int)(character.StatHandler.curStat.GetCurAtk() * getDamageMultiplier(skill.Data, skill.Stars));
    }
    private void UpdateDuration()
    {
        currentDuration += Time.deltaTime;
        if (currentDuration > skill.Duration)
        {
            TerminateSkill();
            gameObject.SetActive(false);
        }
    }
    protected  bool IsLayerMatched(int layerMask, int objectLayer)
    {
        return layerMask == (layerMask | (1 << objectLayer));
    }
}
