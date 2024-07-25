using UnityEngine;
using System;
using System.Collections.Generic;


public class Hero : Character
{
    public HeroSO data;
    public ERoleType roleType;
    public ERarityType rarityType;
    public int gradeLevel;
    public int StartsLevel;
    public System.Random random;
    public CharacterStat gradeStatModifier;
    public CharacterStat starsStatModifier;
    //List<Monster> monsters = new List<Monster>();
    protected override void Awake()
    {
        base.Awake();
        random= new System.Random();
    }

    protected override void Start()
    {
        base.Start();
        
        //StatHandler.baseStat = StatManager.Instance.statHandler.curStat;
        StatHandler.AddStatModifier(data.multipleStat);
        Health.InitHealth(StatHandler.curStat.Health);
        //Target = GameManager.Instance.Monsters[0].transform;

    }


    public void ChangeStat()
    {
        StatHandler.UpdateStatModifier();
    }

    public override void FindTarget()
    {
        targetList.Clear();
        foreach (Character monster in GameManager.Instance.Monsters)
        {
            if (monster != null && monster.isActiveAndEnabled)
            {
                targetList.Add(monster);
            }
        }
        SetTarget();
        return;
    }
    public override void SetTarget()
    {
        if(targetList.Count > 0) 
        {
            int randomIndex = random.Next(targetList.Count);
            Target = targetList[randomIndex].gameObject.transform;
        }
        else
        {
            Target = null;
        }
    }
}