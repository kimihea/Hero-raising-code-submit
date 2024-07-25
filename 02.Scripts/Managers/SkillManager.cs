using System;
using System.Collections.Generic;
using UnityEngine;
public class SkillManager : Singleton<SkillManager>
{
    public List<Skill> SkillList = new List<Skill>();
    public Dictionary<int, List<Skill>> SkillDic = new Dictionary<int, List<Skill>>();

    [ContextMenu("CountUp")]
    public void CountUp()
    {
        SkillList[0].Count++;
    }
    protected override void Awake()
    {
        base.Awake();
        ConvertListToDict();
    }
    void ConvertListToDict()
    {
        SkillDic.Clear();
        foreach (Skill skill in SkillList)
        {
            if (!SkillDic.TryGetValue(skill.Data.SkillId, out List<Skill> skills))
            {
                skills = new List<Skill>();
                SkillDic[skill.Data.SkillId] = skills;
            Debug.Log($"Skill ID: {skill.Data.SkillId} added to the dictionary.");
            }
            skills.Add(skill);
        }
    }
    [ContextMenu("디버깅찍기")]
    List<string> HeroIdToSkill()
    {
        List<string> strings = new List<string>();
        List<Skill> skills = SkillDic[0];
        foreach (Skill skill in skills)
        {
            string str = skill.GetSkillDescription(777);
            strings.Add(str);
            Debug.Log(str);

        }
        Debug.Log(strings);
        return strings;
    }
    public List<string> HeroIdToSkill(int id)
    {
        List<string> strings = new List<string>();
        if (SkillDic.TryGetValue(id, out List<Skill> skills))
        {
            foreach (Skill skill in skills)
            {
                string str = skill.GetSkillDescription(777);
                strings.Add(str);
            }
        }
        return strings;
    }
    public List<T> HeroIdToSprite<T>(int id, Func<Skill, T> skillSelector,T defaultValue)
    {
        List<T> result = new List<T>();
        if (SkillDic.TryGetValue(id, out List<Skill> skills))
        {
            foreach (Skill skill in skills)
            {
                T selectedValue = skillSelector(skill);
                result.Add(selectedValue);
            }
        }
        else
        {
            result.Add(defaultValue);
        }
        return result;
    }

    public SkillInfo GetSkillInfo(int index)
    {
        Skill value = GetSkill(index);
        SkillInfo returnSKill = new SkillInfo()
        {
            Stars = value.Stars,
            Count = value.Count,
            Name = value.Data.Name,
            Description = value.GetSkillDescription(1),
            CoolTime = value.CoolTime.ToString(),
            PassiveEffect = value.Data.Passive,
            Icon = value.Data.Icon,
        };
        return returnSKill;
    }
    private Skill GetSkill(int index)
    {
        if (SkillDic.TryGetValue(index, out List<Skill> skills))
            return skills[0];
        else
        {
            Debug.Log(index +"없는 스킬입니다");
            return skills[0];
        
        }

    }
}

