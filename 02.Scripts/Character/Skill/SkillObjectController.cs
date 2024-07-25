using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public abstract class SkillObjectController : MonoBehaviour
{
    protected float currentDuration;
    public Skill skill;
    [SerializeField] protected LayerMask WallCollisionLayer;
    [SerializeField] protected LayerMask TargetCollisionLayer;
    protected Func<SkillSO, int, float> getDamageMultiplier = (data, stars) =>
    {
        return (data.DamageMultiplier.DefaultValue +
               data.DamageMultiplier.ModifierPerGrade * stars) / 100f;
    };
    protected abstract void MoveSkill();
    protected abstract void ExecuteSkill(); 
    protected virtual void Awake()
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
    protected float SkillDamage(Character character, Skill skill)
    {
        return character.StatHandler.curStat.Atk * getDamageMultiplier(skill.Data, skill.Stars);
    }
    private void UpdateDuration()
    {
        currentDuration += Time.deltaTime;
        if (currentDuration > skill.Duration)
        {
            gameObject.SetActive(false);
        }
    }
    protected  bool IsLayerMatched(int layerMask, int objectLayer)
    {
        return layerMask == (layerMask | (1 << objectLayer));
    }
}
