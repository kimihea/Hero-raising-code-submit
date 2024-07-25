using UnityEngine;
using System;
using UnityEditor.VersionControl;
using Assets.PixelFantasy.PixelMonsters.Common.Scripts;
using System.Collections.Generic;
using Unity.Mathematics;


public class Player : Character
{
    [Header("스텟 훈련")] 
    public int atkUpgradeLevel;
    public int healthUpgradeLevel;
    public int attackSpeedUpgradeLevel;
    //private CharacterStat upgradeStat = new();       // 매번 업그레이드 시 마다 CharacterStat를 추가하는 것 보다 하나의 스탯으로 통합 관리를 하기 위함.
    public event Action OnChangedStatEvent;
    //public Equipment equipment;

    public System.Random random;
    List<Monster> monsters = new List<Monster>();


    protected override void Awake()
    {
        base.Awake();
        random = new System.Random();

        BaseStat tempStat = StatHandler.baseStat;

        StatHandler.baseStat = new CharacterStat();
        StatHandler.baseStat.StatChangeType = EStatChangeType.OVERRIDE;
        StatHandler.baseStat.Atk = tempStat.Atk;
        StatHandler.baseStat.Health = tempStat.Health;
        StatHandler.baseStat.Defense = tempStat.Defense;
        StatHandler.baseStat.AttackSpeed = tempStat.AttackSpeed;
        StatHandler.baseStat.MoveSpeed = tempStat.MoveSpeed;
        StatHandler.baseStat.AttackRange = tempStat.AttackRange;



        StatHandler.curStat = new CharacterStat();
        StatHandler.UpdateStatModifier();

        //equipment = GetComponent<Equipment>();
    }

    protected override void Start()
    {
        base.Start();
        Health.InitHealth(StatHandler.curStat.Health);//일단 플레이어 시작할 때 curStat으로 체력 초기화,추후에 게임매니저에서 관리 예정
        OnChangedStatEvent += ChangeStat;
        StatUpgradeInit();
    }

    private void StatUpgradeInit()
    {        
        atkUpgradeLevel = 1;
        healthUpgradeLevel = 1;
        attackSpeedUpgradeLevel = 1;
        //StatHandler.AddStatModifier(upgradeStat);
        OnChangedStatEvent?.Invoke();

    }

    public void ChangeStat()
    {
        /*StatHandler.UpdateStatModifier();
        GameManager.Instance.HeroUpdate();*/
    }

    public override void FindTarget()
    {
        monsters.Clear();
        foreach (Monster monster in GameManager.Instance.Monsters)
        {
            if (monster != null && monster.isActiveAndEnabled)
            {
                monsters.Add(monster);
            }
        }
        SetTarget();
        return;
    }
    public override void SetTarget()
    {
        if (monsters.Count > 0)
        {
            int randomIndex = random.Next(monsters.Count);
            Target = monsters[randomIndex].gameObject.transform;
        }
        else
        {
            Target = null;
        }
    }
}
