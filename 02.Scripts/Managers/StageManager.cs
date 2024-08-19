using Assets.PixelFantasy.PixelMonsters.Common.Scripts;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

[Serializable]
public class StageSaveData
{
    public static StageSaveData SaveData { get => GameManager.Instance.Stage.SaveData; }

    public int ChapterNum;
    public int StageNum;

    public StageSaveData()
    {
        ChapterNum = 1;
        StageNum = 1;
    }
}

public class StageManager : MonoBehaviour
{
    //private readonly int RANGED_SPAWN_POSITION_IDX = 4;
    private readonly int LAST_CHAPTER_NUM = 9;
    public readonly int BOSS_WAVE_IDX = 4;
    private readonly float CHAPTER_BONUS_REWARD = 0.05f;
    private readonly float STAGE_BONUS_REWARD = 0.7f;
    private int bossOrder;
    private int bossCount;
    [SerializeField] private List<Image> mapRenderer;
    [SerializeField] private List<Transform> MonsterSpawnPositions;
    [SerializeField] private List<Transform> PlayerSpawnPositions;
    [SerializeField] private Transform BossPosition;
    [SerializeField] private Queue<RectTransform> MapQueue;
    [SerializeField] private List<RectTransform> MapPosistion;
    [SerializeField] private Image waveProgressBar; // 웨이브 진행 바 추가
    private float scrollRange;
    private bool IsSpawn = false;
    private bool IsInit;
    private int totalMonsterCount;

    public GameObject StageBarUI;
    [Range(0f, 1f)] public float StageProgress;

    [NonSerialized] public StageSaveData SaveData = new StageSaveData();

    [field: SerializeField] public int ChapterNum { get; private set; }
    [field: SerializeField] public int StageNum { get; private set; }
    [field: SerializeField] public int WaveNum { get; private set; }
    [field: SerializeField] public StageSO Data { get; private set; }
    [field: SerializeField] public BaseStat StageStatModifier { get; private set; }
    [field: SerializeField] public BaseStat ChapterStatModifier { get; private set; }
    [field: SerializeField] public int GoldReward { get; private set; }
    [field: SerializeField] public int ManaStoneFragmentReward { get; private set; }
    [field: SerializeField] public Reward[] ClearRewards { get; private set; }
    [field: SerializeField] public Reward[] AFKRewards { get; private set; }
    public bool IsTryBoss { get; private set; } = true;
    public bool IsMapMoving { get; private set; } = false;

    private void Start()
    {
        //TODO: ���� ��� ���� �� ���� �ʿ�
        //Data = ResourceManager.Instance.GetResource<StageSO>("STG00001", EResourceType.DATA);        
        //InitStage();                
        MapQueue = new Queue<RectTransform>(MapPosistion);
        scrollRange = MapQueue.Peek().position.x - 32f;
        //GameManager.Instance.Stage = this;
        // 초기화 시 웨이브 진행 바 업데이트
        //UpdateWaveProgressBar();
    }

    private void Update()
    {
        if (!PoolManager.Instance.IsInit || !IsSpawn) return;
        if (GameManager.Instance.CombatConditionType == ECombatConditionType.READY && GameManager.Instance.Monsters.Count == 0)
        {
            GameManager.Instance.VictoryBattle();
            return;
        }

        if (!GameManager.Instance.EntryList[0].gameObject.activeSelf)
        {
            GameManager.Instance.DefeatBattle();
            return;
        }
    }

    public async void LoadData()
    {
        bool isDataExist = true;
        SaveData = DataManager.Instance.LoadData<StageSaveData>(ESaveType.STAGE);
        if (SaveData == null)
        {
            SaveData = new StageSaveData();
            isDataExist = false;
        }        
        ChapterNum = SaveData.ChapterNum;
        StageNum = SaveData.StageNum;
        string rcode = "STG" + ChapterNum.ToString("D5");
        Data = await ResourceManager.Instance.GetResource<StageSO>(rcode, EAddressableType.DATA);
        for (int i = 0; i < mapRenderer.Count; i++)
        {
            mapRenderer[i].sprite = Data.MapImage;
        }
        if (isDataExist)
        {
            TimeSpan span = DateTime.Now - DataManager.Instance.UserLoadData.LastUpdateTime.GetDateTime();
            int idleMinute = (int)span.TotalMinutes;
            foreach (var reward in AFKRewards)
            {
                int amount = 0;
                if (reward.type == ECurrencyType.ManaStoneFragment) amount = reward.amount * idleMinute;
                else amount = (int)(reward.amount * (1 + StageNum * STAGE_BONUS_REWARD + ChapterNum * CHAPTER_BONUS_REWARD)) * idleMinute;
                CurrencyManager.Instance.AddCurrency(reward.type, amount);
            }
        }
    }

    public void InitStage()
    {
        WaveNum = 0;
        StageProgress = 0f;
        if (Data == null || Data.ChapterNum != ChapterNum)
        {
            IsInit = false;
            //string rcode = "STG" + ((ChapterNum - 1) * 10 + StageNum).ToString("D5");
            UILoading.Instance.StartFade(InitCoroutine, StartWave);
            AudioManager.Instance.PlayBGM(string.Format("STAGEBGM{0}", ChapterNum));
            //StartCoroutine(InitCoroutine());
            //StartCoroutine(WaitFadeCoroutine());
            return;
        }  
        //GameManager.Instance.player.DefalutPos = PlayerSpawnPositions[0].position;
        for (int i = 0; i < GameManager.Instance.EntryList.Count; i++)
        {
            Character hero = GameManager.Instance.EntryList[i];
            if (!hero.gameObject.activeSelf) hero.gameObject.SetActive(true);
            hero.DefalutPos = PlayerSpawnPositions[i].position;
            hero.gameObject.transform.position = hero.DefalutPos;
        }
        // 초기화 시 웨이브 진행 바 업데이트
        //UpdateWaveProgressBar();
        StartWave();
    }

    public void InitHeroPosition()
    {
        for (int i = 0; i < GameManager.Instance.EntryList.Count; i++)
        {
            Character hero = GameManager.Instance.EntryList[i];
            hero.DefalutPos = PlayerSpawnPositions[i].position;
            hero.gameObject.transform.position = hero.DefalutPos;
        }
    }

    public bool CheckInit()
    {
        return IsInit;
    }    

    private async Task InitAsync()
    {
        string rcode = "STG" + ChapterNum.ToString("D5");
        Data = await ResourceManager.Instance.GetResource<StageSO>(rcode, EAddressableType.DATA);
        
        for (int i = 0; i < mapRenderer.Count; i++)
        {
            mapRenderer[i].sprite = Data.MapImage;
        }
        StageNum = 1;
        IsInit = true;
    }

    private IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return PoolManager.TaskAsIEnumerator(InitAsync());
    }

    public void InitStage(int chapNum, int stageNum)
    {
        //string rcode = "STG" + ((chapNum - 1) * 10 + stageNum).ToString("D5");
        //Data = await ResourceManager.Instance.GetResource<StageSO>(rcode, EAddressableType.DATA);
        InitStage();
    }

    public void StartStage()
    {
        InitStage();        
    }

    public void StartStage(int chapNum, int stageNum)
    {
        InitStage(chapNum, stageNum);
        StartWave();
    }

    [ContextMenu("StartWave")]
    public void StartWave()
    {
        SpawnMonsters();
        GameManager.Instance.ReadyCount = GameManager.Instance.EntryList.Count;
        // 웨이브 시작 시 웨이브 진행 바 업데이트
        //UpdateWaveProgressBar();
    }

    public void StartBoss()
    {
        SpawnMonsters();
        //TODO : ������ ����
        GameManager.Instance.ReadyCount = GameManager.Instance.EntryList.Count;
        //UpdateWaveProgressBar();
    }

    public void SpawnMonsters()
    {
        int mobCount = 0;
        //if (WaveNum == BOSS_WAVE_IDX)
        //{
        //    MonsterSpawn ms = Data.Waves[WaveNum].monsterSpawns[0];
        //    Monster mob = PoolManager.Instance.SpawnFromPool(ms.rcode).GetComponent<Monster>();
        //    mob.gameObject.transform.position = BossPosition.position;
        //    mob.transform.parent = MapQueue.Peek();
        //    mob.StatHandler.statModifiers.RemoveAll(x => true);
        //    mob.StatHandler.AddStatModifier(Data.BossStatModifier);
        //    mob.InitStat();
        //    GameManager.Instance.Monsters.Add(mob);
        //    return;
        //}
        if (WaveNum == BOSS_WAVE_IDX)
        {
            bossOrder = StageNum % 3;
            bossCount = 0;
        }
        totalMonsterCount = 0;
        foreach (MonsterSpawn ms in Data.Waves[WaveNum].monsterSpawns)
        {
            if(WaveNum== BOSS_WAVE_IDX)
            {//3색 바리에이션
                if (bossOrder!=bossCount++)
                    continue;
            }
            for (int i = 0; i < ms.count; i++)
            {
                Monster mob = PoolManager.Instance.SpawnFromPool(ms.rcode).GetComponent<Monster>();                
                //mob.DefalutPos = MonsterSpawnPositions[mobCount].position;
                if (WaveNum == BOSS_WAVE_IDX)
                {
                    mob.gameObject.transform.position = BossPosition.position;
                }
                else
                {
                    mob.gameObject.transform.position = MonsterSpawnPositions[mobCount].position;
                }
                mob.StatHandler.statModifiers.RemoveAll(x => true);
                mob.StatHandler.grade = StageNum;
                mob.StatHandler.stars = ChapterNum - 1;
                                
                mob.StatHandler.AddStatModifier(StageStatModifier);
                mob.StatHandler.AddStatModifier(ChapterStatModifier);

                mob.InitStat();
                GameManager.Instance.Monsters.Add(mob);
                mobCount = ++mobCount % MonsterSpawnPositions.Count;
                totalMonsterCount++;
            }
        }
        IsSpawn = true;
    }
    [ContextMenu("ClearStage")]
    public void ClearWave()
    {
        ClearMonsters(); // Test��

        if (WaveNum == BOSS_WAVE_IDX) //보스 스테이지면 스테이지 클리어 보상 지급
        {
            QuestManager.Instance.AddProgress(EQuestType.STAGEPROGRESS, SaveData.ChapterNum * 100 + SaveData.StageNum);

            IsTryBoss = true;
            foreach(var reward in ClearRewards) 
            {
                int amountBasedOnStage = (int)(reward.amount * (1 + StageNum * STAGE_BONUS_REWARD + ChapterNum * CHAPTER_BONUS_REWARD));
                CurrencyManager.Instance.AddCurrency(reward.type, amountBasedOnStage);
            }            

            if (StageNum == 10)  //챕터 넘어가는지 검사
            {
                ChapterNum++;
            }

            StageNum = (StageNum + 1) % 11;
            
            if(ChapterNum > LAST_CHAPTER_NUM)
            {
                ChapterNum = LAST_CHAPTER_NUM;
                StageNum = 10;
            }

            if (SaveData.ChapterNum * 10 + SaveData.StageNum < ChapterNum * 10 + StageNum) // 최고 클리어 기록 갱신
            {
                CurrencyManager.Instance.AddCurrency(ECurrencyType.Diamond, 200);
                SaveData.ChapterNum = ChapterNum;
                SaveData.StageNum = StageNum;
                //DataManager.Instance.SaveData(ESaveType.STAGE);
            }

            StartStage();
            //StartStage(ChapterNum + (StageNum + 1) / 10, (StageNum + 1) % 10);            
            return;
        }

        if (WaveNum != BOSS_WAVE_IDX - 1 || IsTryBoss)
        {
            WaveNum++;
        }

        if (WaveNum == BOSS_WAVE_IDX && IsTryBoss)
        {
            StartBoss();
        }
        else
        {
            StartWave();
        }
    }

    [ContextMenu("FailedStage")]
    public void FailedStage()
    {
        ClearMonsters();
        if (WaveNum == BOSS_WAVE_IDX)
        {
            IsTryBoss = false;
            //WaveNum = BOSS_WAVE_IDX - 1;
            if (StageProgress >= 1f) WaveNum = BOSS_WAVE_IDX - 1;
            else
            {
                WaveNum = (int)(StageProgress * BOSS_WAVE_IDX);
                StageProgress = (float)WaveNum / BOSS_WAVE_IDX;
            }
            StartWave();
        }
        else
        {
            StageProgress = (float)WaveNum / BOSS_WAVE_IDX;
            StartWave();
        }
    }

    public void StartBossWave()
    {        
        UILoading.Instance.StartFade(WaitFadeCoroutine, StartWave);
        
        //StartCoroutine(WaitFadeCoroutine());
    }
    
    private IEnumerator WaitFadeCoroutine()
    {
        ClearMonsters();
        GameManager.Instance.CombatConditionType = ECombatConditionType.READY;
        GameManager.Instance.ResetHero();
        WaveNum = BOSS_WAVE_IDX;
        yield return new WaitUntil(() => GameManager.Instance.CheckHeroReady());
    }

    public void DropReward(Monster monster)
    {
        DropCurrency goldObj = PoolManager.Instance.SpawnFromPool<DropCurrency>("CRC00001");
        goldObj.transform.position = monster.transform.position;
        goldObj.TargetPos = CurrencyManager.Instance.GoldUI;
        goldObj.InitDropReward(ECurrencyType.Gold, (int)Math.Ceiling(GoldReward * (1 + StageNum * STAGE_BONUS_REWARD + ChapterNum * CHAPTER_BONUS_REWARD)));
        goldObj.GetRewards();
        DropCurrency manaObj = PoolManager.Instance.SpawnFromPool<DropCurrency>("CRC00002");
        manaObj.TargetPos = CurrencyManager.Instance.MimicUI;
        manaObj.InitDropReward(ECurrencyType.ManaStoneFragment, ManaStoneFragmentReward);
        manaObj.transform.position = monster.transform.position;
        manaObj.GetRewards();
        if (WaveNum == BOSS_WAVE_IDX) StageProgress = 1f;
        else
        {
            QuestManager.Instance.AddProgress(EQuestType.MONSTER, 1);
        }
        if (StageProgress < 1f) StageProgress += 1f / (totalMonsterCount * BOSS_WAVE_IDX);
        if (GameManager.Instance.Monsters.Count == 0)
        {
            foreach(Character ch in GameManager.Instance.EntryList)
            {
                ISkillController ISC = ch.GetComponent<ISkillController>();
                ISC?.ShutDown(false);
            }
            GameManager.Instance.CombatConditionType = ECombatConditionType.END;
        }
    }

    public void ClearMonsters()
    {
        foreach (var mob in GameManager.Instance.Monsters)
        {
            mob.gameObject.SetActive(false);
        }
        GameManager.Instance.Monsters.RemoveAll(x => true);
        IsSpawn = false;
    }

    //private void UpdateWaveProgressBar()
    //{
    //    if (waveProgressBar != null)
    //    {
    //        waveProgressBar.fillAmount = (float)WaveNum / BOSS_WAVE_IDX;
    //    }
    //}
    
    public float GetWaveProgress()
    {
        float monsterRate = 0;
        if (totalMonsterCount != 0)
        {
            monsterRate = (float)(totalMonsterCount - GameManager.Instance.Monsters.Count) / (totalMonsterCount * BOSS_WAVE_IDX);
        }
        
        return (float)WaveNum / BOSS_WAVE_IDX + monsterRate;
    }

    public void MoveMap()
    {
        if (IsMapMoving || GameManager.Instance.battleType != EBattleType.STAGE) return;
        IsMapMoving = true;
        GameManager.Instance.ReadyCount = GameManager.Instance.EntryList.Count;
        StartCoroutine(MoveMapCoroutine());
        GameManager.Instance.CombatConditionType = ECombatConditionType.READY;
        foreach (Character hero in GameManager.Instance.EntryList)
        {
            if (!hero.gameObject.activeSelf) continue;
            hero.StateMachine.ChangeState(hero.StateMachine.Pursuit);
        }
        //GameManager.Instance.CombatConditionType = ECombatConditionType.READY;        
    }

    private IEnumerator MoveMapCoroutine()
    {
        float curTime = 0;
        do
        {
            for (int i = 0; i < MapPosistion.Count; i++)
            {
                MapPosistion[i].Translate(Vector2.left * Time.deltaTime * 2f);
            }
            curTime += Time.deltaTime;
            yield return null;
        } while (curTime < 2f);

        RectTransform mp = MapQueue.Peek();
        if (mp.position.x < scrollRange)
        {
            mp = MapQueue.Dequeue();
            mp.position = mp.position + Vector3.right * 32f * 2f;            
            MapQueue.Enqueue(mp);
        }
        IsMapMoving = false;
    }
}