using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


[System.Serializable]
public class Skill :MonoBehaviour
{
    public SkillSO Data;
    public string Description;
    public int Stars;
    public int Count;
    

    [SerializeField] private float coolTime;
    [SerializeField] private float duration;

    [NonSerialized]public bool IsCharge;
    public ESkillMotion Motion;

    [SerializeField]public GameObject SkillObj;
    public bool IsSustainChanneling;
    [Range(0f, 1f)] public float StartTime;

    [Range(0f, 1f)] public float EndTime;

    public List<float> BuffStatRateList= new List<float>();//하나의 buffSkill에서 여러개의 buffStat을 변경하고 표시할 때 써줌
    private string timeColor;
    private string valueColor;
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
        return (Data.DamageMultiplier.DefaultValue + Data.DamageMultiplier.ModifierPerGrade * Stars)/100f;
    }
    public string ColorText(string color,float value)
    {
        return color + value.ToString() + "</color>";
    }

    /// <summary>
    /// 버프스킬 설명해줄 때 매개변수로 표시할 때 params rate 1로 표시해 줄 것
    /// </summary>
    /// <param name="atk"></param>
    /// <param name="rate"></param>
    /// <returns></returns>
    public string GetSkillDescription(float  atk = 1)
    {
        if (Data.Type == ESkillType.AOE)
        {
            timeColor = "<color=#da70d6>"; valueColor = "<color=#ff4500>";
            return string.Format(Data.Description, 
                ColorText(timeColor, duration), 
                ColorText(timeColor, Data.Interval),
                ColorText(valueColor, atk * Data.TicDamageMultiplier * DamagePerGradge() * 100), 
                ColorText(valueColor, atk * Data.EndDamageMultiplier * DamagePerGradge() * 100));
        }
        else if (Data.Type == ESkillType.PROJECTILE)
        {
            valueColor = "<color=#ff4500>";
            return string.Format(Data.Description, 
                ColorText(valueColor, atk * DamagePerGradge() * 100));
        }

        else if (Data.Type == ESkillType.BUFF)
        {
            timeColor = "<color=#da70d6>";valueColor ="<Color=#ffff00>";
            return string.Format(Data.Description, 
                ColorText(timeColor,duration),
                ColorText(valueColor, (atk * DamagePerGradge() * 100 - 100)* (IsBuffSkillSolo() ? 1 : BuffStatRateList[0])),
                ColorText(valueColor, (atk * DamagePerGradge() * 100 - 100) * (IsBuffSkillSolo() ? 1 : BuffStatRateList[1])));
        }
        else if( Data.Type == ESkillType.Heal)
        {
            timeColor = "<color=#da70d6>"; valueColor = "<Color=#33ff33>";
            return string.Format(Data.Description,
                ColorText(timeColor, duration),
                ColorText(valueColor, atk * DamagePerGradge() * 100));
        }
        else return string.Empty;
    }
    private bool IsBuffSkillSolo()
    {
        return BuffStatRateList.Count ==0;
    }
}
