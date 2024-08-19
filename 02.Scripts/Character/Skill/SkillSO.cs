using System;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]

public class SkillSO : ScriptableObject
{
    public ESkillType Type;
    public int HeroId;
    public int SkillId;
    public ERarityType Rarity;
    public float SkillMoveSpeed;
    public float ChannelingTime;
    public float SkillRange;
    public string Name;
    public string Description;
    public string Passive;
    public Sprite Icon;
    public GradeMultiplier DamageMultiplier;

    [Header("AOE's Field")]
    public float TicDamageMultiplier;
    public float EndDamageMultiplier;
    public float Interval;
}

[Serializable]
public class GradeMultiplier
{
    public int DefaultValue;
    public int ModifierPerGrade;
}