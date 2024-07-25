using Assets.PixelFantasy.PixelMonsters.Common.Scripts;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    //private readonly int RANGED_SPAWN_POSITION_IDX = 4;
    //private readonly int STAGE_LAST_IDX = 10;
    private readonly int BOSS_WAVE_IDX = 4;
    [SerializeField] private List<SpriteRenderer> mapRenderer;
    [SerializeField] private List<Transform> MonsterSpawnPositions;
    [SerializeField] private List<Transform> PlayerSpawnPositions;
    [SerializeField] private Transform BossPosition;
    [SerializeField] private Queue<Transform> MapQueue;
    [SerializeField] private List<Transform> MapPosistion;
    [SerializeField] private Image waveProgressBar; // 웨이브 진행 바 추가
    private float scrollRange;

    [field: SerializeField] public int ChapterNum { get; private set; }
    [field: SerializeField] public int StageNum { get; private set; }
    [field: SerializeField] public int WaveNum { get; private set; }
    [field: SerializeField] public StageSO Data { get; private set; }
    public bool IsTryBoss { get; private set; } = true;
    public bool IsMapMoving { get; private set; } = false;

    private void Start()
    {
        //TODO: ���� ��� ���� �� ���� �ʿ�
        //Data = ResourceManager.Instance.GetResource<StageSO>("STG00001", EResourceType.DATA);        
        //InitStage();        
        scrollRange = -25f;
        // 초기화 시 웨이브 진행 바 업데이트
        UpdateWaveProgressBar();
    }

    private void Update()
    {
        if (GameManager.Instance.CombatConditionType == ECombatConditionType.READY && GameManager.Instance.Monsters.Count == 0)
        {
            GameManager.Instance.VictoryBattle();
            return;
        }
    }

    public void InitStage()
    {       
        for(int i = 0; i < mapRenderer.Count; i++)
        {
            mapRenderer[i].sprite = Data.MapImage;
        }
        MapQueue = new Queue<Transform>(MapPosistion);
        ChapterNum = Data.ChapterNum;
        StageNum = Data.StageNum;        
        WaveNum = 0;
        //GameManager.Instance.player.DefalutPos = PlayerSpawnPositions[0].position;
        for(int i = 0; i < GameManager.Instance.EntryList.Count; i++)
        {
            Character hero = GameManager.Instance.EntryList[i];
            hero.DefalutPos = PlayerSpawnPositions[i].position;
            hero.gameObject.transform.position = hero.DefalutPos;
        }
        // 초기화 시 웨이브 진행 바 업데이트
        UpdateWaveProgressBar();
    }

    public void InitStage(int chapNum, int stageNum)
    {
        string rcode = "STG" + ((chapNum - 1) * 10 + stageNum).ToString("D5");
        Data = ResourceManager.Instance.GetResource<StageSO>(rcode, EResourceType.DATA);
        InitStage();
    }

    public void StartStage()
    {
        InitStage();
        StartWave();
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
        UpdateWaveProgressBar();
    }
    
    public void StartBoss()
    {        
        SpawnMonsters();
        //TODO : ������ ����
        GameManager.Instance.ReadyCount = GameManager.Instance.EntryList.Count;
    }    

    public void SpawnMonsters()
    {
        int mobCount = 0;
        if(WaveNum == BOSS_WAVE_IDX)
        {
            MonsterSpawn ms = Data.Waves[WaveNum].monsterSpawns[0];
            Monster mob = PoolManager.Instance.SpawnFromPool(ms.rcode).GetComponent<Monster>();
            mob.gameObject.transform.position = BossPosition.position;
            mob.transform.parent = MapQueue.Peek();
            mob.StatHandler.AddStatModifier(Data.BossStatModifier);
            mob.InitStat();
            GameManager.Instance.Monsters.Add(mob);
            return;
        }        
        foreach (MonsterSpawn ms in Data.Waves[WaveNum].monsterSpawns)
        {
            for (int i = 0; i < ms.count; i++)
            {
                Monster mob = PoolManager.Instance.SpawnFromPool(ms.rcode).GetComponent<Monster>();
                mob.gameObject.transform.position = MonsterSpawnPositions[mobCount].position;
                mob.DefalutPos = MonsterSpawnPositions[mobCount].position;
                mobCount = ++mobCount % MonsterSpawnPositions.Count;
                mob.StatHandler.AddStatModifier(Data.MonsterStatModifier);
                mob.InitStat();
                GameManager.Instance.Monsters.Add(mob);
            }
        }        
    }    

    [ContextMenu("ClearStage")]
    public void ClearWave()
    {
        ClearMonsters(); // Test��

        if(WaveNum == BOSS_WAVE_IDX)
        {
            if (StageNum == 10)
            {
                ChapterNum++;
            }

            StartStage(ChapterNum + (StageNum + 1) / 10, (StageNum + 1) % 10);            
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
            WaveNum = BOSS_WAVE_IDX - 1;
            StartWave();
        }
        else
        {                        
            StartWave();
        }        
    }

    public void DropReward(Monster monster)
    {
        DropCurrency goldObj = PoolManager.Instance.SpawnFromPool<DropCurrency>("CRC00001");        
        goldObj.transform.position = monster.transform.position;
        goldObj.TargetPos = CurrencyManager.Instance.GoldUI;
        goldObj.InitDropReward(ECurrencyType.Gold, Data.GoldReward);
        goldObj.GetRewards();
        DropCurrency manaObj = PoolManager.Instance.SpawnFromPool<DropCurrency>("CRC00002");
        manaObj.TargetPos = CurrencyManager.Instance.MimicUI;
        manaObj.InitDropReward(ECurrencyType.ManaStoneFragment, Data.ManaStoneReward);
        manaObj.transform.position = monster.transform.position;
        manaObj.GetRewards();
        if (GameManager.Instance.Monsters.Count == 0)
        {
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
    }

    private void UpdateWaveProgressBar()
    {
        if (waveProgressBar != null)
        {
            waveProgressBar.fillAmount = (float)WaveNum / BOSS_WAVE_IDX;
        }
    }

    public void MoveMap()
    {
        if (IsMapMoving) return;
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
            for(int i = 0; i < MapPosistion.Count; i++)
            {
                MapPosistion[i].Translate(Vector2.left * Time.deltaTime * 2f);
            }            
            curTime += Time.deltaTime;
            yield return null;
        } while (curTime < 2f);

        Transform mp = MapQueue.Peek();
        if(mp.position.x < scrollRange)
        {
            mp = MapQueue.Dequeue();
            mp.position = mp.position + Vector3.right * 80f;
            MapQueue.Enqueue(mp);
        }                
        IsMapMoving = false;
    }    
}