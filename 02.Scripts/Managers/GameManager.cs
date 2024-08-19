using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    public static bool isInit;
    public bool isReady;

    public StageManager Stage;
    public GoldDungeonManager GoldDungeon;
    public MasteryManager Mastery;

    [SerializeField] public Player player;
    public List<Character> EntryList; /* 0 : Player*/ 
    private readonly float minDistance = 0.5f; /* 캐릭터들이 안 겹치게*/  
    public List<Monster> Monsters;
    public int ReadyCount = 0;

    public EBattleType battleType;
    public ECombatConditionType CombatConditionType = ECombatConditionType.READY;

    private int dungeonNum;

    public AlertPanel AlertObj;

    private WaitForSecondsRealtime waitRead;

    public HeroGacha heroGacha;
    
    private IEnumerator Start()
    {
        #region NonIntro
        // TODO : 배포시 삭제
        if (!ResourceManager.Instance.isInit)
        {
            UILoading.Instance.SetBG();            
            yield return new WaitUntil(() => ResourceManager.Instance.isInit);
        }
        #endregion
        Application.targetFrameRate = 60;
        battleType = EBattleType.STAGE;
        CombatConditionType = ECombatConditionType.READY;
        Stage.LoadData();
        GoldDungeon.LoadData();
        yield return new WaitUntil(() => PoolManager.Instance.IsInit);
        UILoading.Hide();
        AudioManager.Instance.PlayBGM(string.Format("STAGEBGM{0}", Stage.ChapterNum));
        StartBattle();
        waitRead = new WaitForSecondsRealtime(3);
    }
    private void Update()
    {
        HeroPosUpdate();
    }
    private void StartBattle()
    {
        switch(battleType)
        {
            case EBattleType.STAGE:                
                Stage.StartStage();
                break;
            case EBattleType.GOLDDUNGEON:
                GoldDungeon.StartDungeon(dungeonNum);
                break;
        }
    }

    public void DefeatBattle()
    {
        switch(battleType)
        {
            case EBattleType.STAGE:
                CombatConditionType = ECombatConditionType.READY;
                RevivalHeroes();
                Stage.FailedStage();
                break;
            case EBattleType.GOLDDUNGEON:
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
            case EBattleType.GOLDDUNGEON:
                //GoldDungeon.ClearDungeon();
                ChangeBattle(EBattleType.STAGE);
                break;
        }
    }

    public void ChangeBattle(EBattleType type)
    {        
        UILoading.Instance.StartFade(ChangeBattleCoroutine, StartBattle, type);
    }

    public IEnumerator ChangeBattleCoroutine(params object[] type)
    {        
        switch (battleType)
        {
            case EBattleType.STAGE:
                Stage.gameObject.SetActive(false);
                Stage.StageBarUI.SetActive(false);
                break;
            case EBattleType.GOLDDUNGEON:
                GoldDungeon.CombatObject.gameObject.SetActive(false);
                GoldDungeon.ProgressBar.SetActive(false);
                break;
        }

        ResetMonsters();
        CombatConditionType = ECombatConditionType.READY;
        battleType = (EBattleType)type[0];
        switch (battleType)
        {
            case EBattleType.STAGE:
                AudioManager.Instance.PlayBGM(string.Format("STAGEBGM{0}", Stage.ChapterNum));
                Stage.gameObject.SetActive(true);
                Stage.StageBarUI.SetActive(true);
                Stage.InitHeroPosition();
                break;
            case EBattleType.GOLDDUNGEON:
                AudioManager.Instance.PlayBGM("DUNGEONBGM");//
                GoldDungeon.CombatObject.gameObject.SetActive(true);
                GoldDungeon.ProgressBar.SetActive(true);
                GoldDungeon.InitHeroPosition();
                break;
        }
        ResetHero();

        yield return new WaitUntil(() => CheckHeroReady());
    }


    public void MonsterDeath(Monster monster)
    {
        Monsters.Remove(monster);
        switch (battleType)
        {
            case EBattleType.STAGE:
                Stage.DropReward(monster);
                break;
            case EBattleType.GOLDDUNGEON:
                GoldDungeon.AddClearPoint(monster);
                break;
        }        
    }
    public void HeroPosUpdate()
    {
        foreach (var character in EntryList)
        {
            foreach(var otherCharacter in EntryList)
            {
                if (character == otherCharacter) continue;

                float distance = Vector3.Distance(character.transform.position, otherCharacter.transform.position);
                if (distance < minDistance)
                {
                    Vector3 direction = (character.transform.position - otherCharacter.transform.position).normalized;
                    character.transform.position += direction * (minDistance - distance) / 2;
                    otherCharacter.transform.position -= direction * (minDistance - distance) / 2;
                }
            }
        }
        
    }
    public void HeroUpdate()
    {
        for ( int i = 0; i < HeroManager.instance.hidList.Count; i++)
        {
            if (HeroManager.instance.heroDict.ContainsKey(HeroManager.instance.hidList[i] ) )
            {
                HeroManager.instance.heroDict[HeroManager.instance.hidList[i]].StatHandler.UpdateStatModifier();
            }
        }
    }

    public void RevivalHeroes()
    {
        if (!EntryList[0].gameObject.activeSelf) SkillManager.Instance.OnBattleChanged();
        foreach(Character hero in EntryList)
        {
            hero.transform.position = hero.DefalutPos;
            if (!hero.gameObject.activeSelf)
            {                
                hero.InitStat();
                hero.gameObject.SetActive(true);                
                ISkillController sc = hero.GetComponent<ISkillController>(); //character로 찾기에 interface를 받아서 메소드 실현하는 것으로 구현
                sc?.ShutDown();//실행되고 있던 스킬들 정지시키기
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
            ISkillController sc = hero.GetComponent<ISkillController>(); //character로 찾기에 interface를 받아서 메소드 실현하는 것으로 구현
            sc?.ShutDown();
        }
        SkillManager.Instance.OnBattleChanged();
    }

    public void ResetMonsters()
    {
        if(battleType == EBattleType.STAGE)
        {
            Stage.ClearMonsters();
            return;
        }
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

    public List<Hero> GetHeroEntry()
    {
        return EntryList.OfType<Hero>().ToList(); 
    }

    public void ChangeEntry()
    {        
        UILoading.Instance.StartFade(ChangeEntryCoroutine, StartBattle);
        //StartCoroutine(WaitFadeCoroutine());
    }
    
    public IEnumerator ChangeEntryCoroutine()
    {
        isReady = false;
        ResetMonsters();
        CombatConditionType = ECombatConditionType.READY;
        for (int i = 1; i < EntryList.Count; i++)
        {
            EntryList[i].gameObject.SetActive(false);
        }
        EntryList.RemoveAll(x => x.GetType() == typeof(Hero));
        EntryList.AddRange(HeroManager.Instance.heroEntry);
        ResetHero();
        isReady = true;
        yield return new WaitUntil(CheckHeroReady);
    }    

    public void GoldDungeonStart(int num)
    {
        dungeonNum = num;
        ChangeBattle(EBattleType.GOLDDUNGEON);
    }

    public void ShowAlert(string message,EAlertType type)
    {
        AlertObj.ShowAlert(message,type);
    }
    public void ShowAlert()
    {
        AlertObj.ShowAlert("개발 예정입니다", EAlertType.NOTIMPLEMENTED);
    }
}