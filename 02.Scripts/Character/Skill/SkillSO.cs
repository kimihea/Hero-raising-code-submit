using System;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]

public class SkillSO : ScriptableObject
{
    public ESkillType Type;
    public int SkillId;
    public float SkillMoveSpeed;
    public float ChannelingTime;
    public string Name;
    public string Description;
    public string Passive;
    public Sprite Icon;
    public GradeMultiplier DamageMultiplier;

    [Header("AOE's Field")]
    public float TicDamageMultiplier;
    public float EndDamageMultiplier;
}

[Serializable]
public class GradeMultiplier
{
    public int DefaultValue;
    public int ModifierPerGrade;
}