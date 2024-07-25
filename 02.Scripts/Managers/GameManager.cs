using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public StageManager Stage;
    public DungeonManager Dungeon;

    [SerializeField] public Player player;
    public List<Character> EntryList; // 0 : Player
    public List<Monster> Monsters;
    public int ReadyCount = 0;

    private EBattleType battleType;
    public ECombatConditionType CombatConditionType;

    private int dungeonNum;

    public HeroGacha heroGacha;
    private void Start()
    {
        //player = GameObject.FindWithTag("Player").GetComponent<Player>();
        //player.healthSystem.OnDieEvent += DefeatBattle;
        battleType = EBattleType.STAGE;
        CombatConditionType = ECombatConditionType.READY;
        StartBattle();
    }

    private void Update()
    {
        if (!EntryList[0].gameObject.activeSelf)
        {            
            DefeatBattle();
            return;
        }             
    }

    private void StartBattle()
    {
        switch(battleType)
        {
            case EBattleType.STAGE:                
                Stage.StartStage();
                break;
            case EBattleType.DUNGEON:
                Dungeon.StartDungeon(dungeonNum);
                break;
        }
    }

    private void DefeatBattle()
    {
        switch(battleType)
        {
            case EBattleType.STAGE:
                CombatConditionType = ECombatConditionType.READY;
                RevivalHeroes();
                Stage.FailedStage();
                break;
            case EBattleType.DUNGEON:
                ChangeBattle(EBattleType.STAGE);
                break;
        }
    }

    public void VictoryBattle()
    {
        switch (battleType)
        {
            case EBattleType.STAGE:
                if(CheckHeroReady())
                {
                    //CombatConditionType = ECombatConditionType.READY;
                    RevivalHeroes();
                    Stage.ClearWave();
                }                
                break;
            case EBattleType.DUNGEON:
                Dungeon.ClearDungeon();
                ChangeBattle(EBattleType.STAGE);
                break;
        }
    }

    public void ChangeBattle(EBattleType type)
    {
        CombatConditionType = ECombatConditionType.READY;
        // TODO : 화면 전환 연출(Fadeout > fadein ?)
        switch (battleType)
        {
            case EBattleType.STAGE:
                Stage.gameObject.SetActive(false);
                break;
            case EBattleType.DUNGEON:
                Dungeon.gameObject.SetActive(false);
                break;
        }

        ResetMonsters();
        battleType = type;
        
        switch (battleType)
        {
            case EBattleType.STAGE:
                Stage.gameObject.SetActive(true);
                Stage.StartStage();
                break;
            case EBattleType.DUNGEON:
                Dungeon.gameObject.SetActive(true);
                Dungeon.StartDungeon(dungeonNum);
                break;
        }
        ResetHero();
    }

    public void MonsterDeath(Monster monster)
    {
        Monsters.Remove(monster);
        switch (battleType)
        {
            case EBattleType.STAGE:
                Stage.DropReward(monster);
                break;
            case EBattleType.DUNGEON:
                Dungeon.AddClearPoint(monster);
                break;
        }        
    }

    public void HeroUpdate()
    {
        //foreach (Hero hero in EntryList)
        //{
        //    hero.ChangeStat();
        //}
    }

    public void RevivalHeroes()
    {
        foreach(Character hero in EntryList)
        {
            if (!hero.gameObject.activeSelf)
            {
                hero.transform.position = hero.DefalutPos;
                hero.InitStat();
                hero.gameObject.SetActive(true);
            }
        }
    }
    public void ResetHero()
    {
        foreach (Character hero in EntryList)
        {
            hero.transform.position = hero.DefalutPos;
            hero.InitStat();
            hero.gameObject.SetActive(true);
        }        
    }

    public void ResetMonsters()
    {
        foreach (Monster monster in Monsters)
        {
            monster.gameObject.SetActive(false);
        }
        Monsters.RemoveAll(x => true);
    }

    public bool CheckHeroReady()
    {
        //if (!player.gameObject.activeSelf) return false;
        foreach(Character hero in EntryList)
        {
            if (!hero.gameObject.activeSelf) continue;
            IState state = hero.StateMachine.currentState;
            if(!(state is CharacterIdleState))
            {
                return false;
            }
        }
        return true;
    }

    [ContextMenu("DungeonStart")]
    public void DungeonStart()
    {
        dungeonNum = 1;
        ChangeBattle(EBattleType.DUNGEON);
    }
}