using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


[Serializable]
public class Skill :MonoBehaviour
{
    public SkillSO Data;
    public string Description;
    public int Stars;
    public int Count;
    

    [SerializeField] private float coolTime;
    [SerializeField] private float duration;
    [NonSerialized]public bool IsCharge;
    public GameObject SkillObj;
    public float CoolTime
    {
        get => coolTime;
    }

    public float Duration
    {
        get => duration;
    }
    public float DamagePerGradge()
    {
        return (Data.DamageMultiplier.DefaultValue + Data.DamageMultiplier.ModifierPerGrade * Stars) / 100f;
    }

    public string GetSkillDescription(float atk)
    {
        if (Data.Type == ESkillType.AOE)
        {
            return string.Format(Data.Description, Data.TicDamageMultiplier, atk * Data.TicDamageMultiplier * DamagePerGradge(), atk * Data.EndDamageMultiplier * DamagePerGradge());
        }
        else if (Data.Type == ESkillType.PROJECTILE)
            return string.Format(Data.Description, atk * DamagePerGradge());
        else if (Data.Type == ESkillType.BUFF) return string.Format(Data.Description, duration, atk * DamagePerGradge());

        else return string.Empty;
    }
}
