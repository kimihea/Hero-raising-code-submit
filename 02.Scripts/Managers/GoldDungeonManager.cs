using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GoldDungeonSaveData
{
    public static GoldDungeonSaveData SaveData { get => GameManager.Instance.GoldDungeon.SaveData; }

    public int LevelNum;
    public int TicketNum;

    public GoldDungeonSaveData()
    {
        LevelNum = 0;
        TicketNum = 2;
    }
}

public class GoldDungeonManager : MonoBehaviour
{
    private readonly float MONSTER_RESPAWN_DELAY = 1f;
    public readonly int MAX_DUNGEON_COUNT = 10;
    public readonly float TIME_LIMIT = 60f;
    private WaitForSeconds spawnDelayTime;
    public Transform BasePos;
    public LayerMask MobLayerMask;
    [SerializeField] private List<Transform> MonsterSpawnPositions;
    [SerializeField] private List<Transform> PlayerSpawnPositions;
    [NonSerialized] public GoldDungeonSaveData SaveData = new GoldDungeonSaveData();
    public GoldDungeonCombat CombatObject;
    public GameObject ProgressBar;

    [field: SerializeField] public int DungeonNum { get; private set; }
    [field: SerializeField] public List<GoldDungeonSO> DataList { get; private set; }
    [field: SerializeField] public int TotalClearPoint { get; private set; }
    [field: SerializeField] public int CurClearPoint { get; private set; }
    public float RemainTime;
    //[field: SerializeField] public int TicketCount { get; private set; }

    private IEnumerator Start()
    {
        spawnDelayTime = new WaitForSeconds(MONSTER_RESPAWN_DELAY);
        yield return new WaitUntil(() => ResourceManager.Instance.isInit);
        CombatObject = GetComponentInChildren<GoldDungeonCombat>();
        Init();
        CombatObject.gameObject.SetActive(false);
    }

    public void LoadData()
    {
        SaveData = DataManager.Instance.LoadData<GoldDungeonSaveData>(ESaveType.GOLDDUNGEON);
        if (SaveData == null)
        {
            SaveData = new GoldDungeonSaveData();
        }
        else
        {
            if (DateTime.Now.Day != DataManager.Instance.UserLoadData.LastUpdateTime.GetDateTime().Day)
            {
                if(SaveData.TicketNum < 2)
                    SaveData.TicketNum = 2;
            }
        }
    }

    private async void Init()
    {
        for (int i = 0; i < MAX_DUNGEON_COUNT; i++)
        {
            string rcode = "DNG" + (i + 1).ToString("D5");
            var data = await ResourceManager.Instance.GetResource<GoldDungeonSO>(rcode, EAddressableType.DATA);
            DataList.Add(data);
        }        
    }        

    public IEnumerator InitNextDungeon()
    {
        DungeonNum++;
        GameManager.Instance.ResetHero();
        yield return null;
    }    

    public void InitDungeon(int dungeonNum)
    {
        var data = DataList[dungeonNum - 1];
        DungeonNum = data.DungeonNum;
        TotalClearPoint = data.TotalClearPoint;
        CurClearPoint = 0;
        RemainTime = TIME_LIMIT;
        CombatObject.IsWin = false;
        for (int i = 0; i < GameManager.Instance.EntryList.Count; i++)
        {
            Character hero = GameManager.Instance.EntryList[i];
            hero.DefalutPos = PlayerSpawnPositions[i].position;
            hero.gameObject.transform.position = hero.DefalutPos;
        }
        GameManager.Instance.CombatConditionType = ECombatConditionType.READY;
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

    public void StartDungeon()
    {
        StartDungeon(DungeonNum);
    }

    public void StartDungeon(int dungeonNum)
    {
        InitDungeon(dungeonNum);
        StartCoroutine(SpawnMonsters());
    }

    public async void ClearDungeon()
    {
        QuestManager.Instance.AddProgress(EQuestType.DUNGEON, 1);
        GameManager.Instance.ResetMonsters();
        StopAllCoroutines();
        CombatObject.IsWin = true;
        foreach (Reward reward in DataList[DungeonNum - 1].Rewards)
        {
            CurrencyManager.Instance.AddCurrency(reward.type, reward.amount);
        }
        if(DungeonNum > SaveData.LevelNum) SaveData.LevelNum++;
        SaveData.TicketNum--;
        GameObject obj = await ResourceManager.Instance.GetResource<GameObject>("DungeonClearPanel", EAddressableType.UI);
        AudioManager.Instance.PlaySFX("SUCCESS");
        UIDungeonClearPanel newObj = Instantiate(obj, UIManager.Instance.transform).GetComponent<UIDungeonClearPanel>();
        newObj.GoldDungeon = this;
        newObj.RewardTxt.text = DataList[DungeonNum - 1].Rewards[0].amount.ToString();      
        //newObj.Show();
        //newObj.InitUI(Mastery.Info, Icon.sprite);
    }

    public void SweepDungeon(int count, int level)
    {
        if (SaveData.TicketNum <= 0) return;
        SaveData.TicketNum -= count;
        QuestManager.Instance.AddProgress(EQuestType.DUNGEON, count);

        foreach (Reward reward in DataList[level - 1].Rewards)
        {
            CurrencyManager.Instance.AddCurrency(reward.type, reward.amount * count);
        }
    }

    public IEnumerator SpawnMonsters()
    {
        do
        {
            for (int i = 0; i < MonsterSpawnPositions.Count; i++)
            {
                int rand = UnityEngine.Random.Range(0, 100);
                string rcode = null;
                for (int j = 0; j < DataList[DungeonNum - 1].SpawnInfo.Length; j++)
                {
                    if(rand < DataList[DungeonNum - 1].SpawnInfo[j].spawnRate)
                    {
                        rcode = DataList[DungeonNum - 1].SpawnInfo[j].rcode;
                        break;
                    }                    
                }
                Monster mob = PoolManager.Instance.SpawnFromPool(rcode).GetComponent<Monster>();
                mob.gameObject.transform.position = MonsterSpawnPositions[i].position;
                mob.DefalutPos = MonsterSpawnPositions[i].position;
                mob.StatHandler.statModifiers.RemoveAll(x => true);
                mob.StatHandler.AddStatModifier(DataList[DungeonNum - 1].MonsterStatModifier);
                mob.InitStat();
                GameManager.Instance.Monsters.Add(mob);
                yield return spawnDelayTime;
            }            
        } while (CurClearPoint < TotalClearPoint);
    }

    public void StopSpawn()
    {
        StopAllCoroutines();
    }

    public void AddClearPoint(Monster mob)
    {
        int point = DataList[DungeonNum - 1].SpawnInfo.ToList().Find(x => x.rcode == mob.name).clearPoint;
        CurClearPoint += point;
        if(CurClearPoint >= TotalClearPoint)
        {
            GameManager.Instance.CombatConditionType = ECombatConditionType.END;
            ClearDungeon();      
            
            //GameManager.Instance.VictoryBattle();
        }
    }
    public void CheckBase(float Radius, out List<Transform> target)
    {
        target = new(); // 기본값으로 null 설정

        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(BasePos.position, Radius, MobLayerMask.value); //몬스터들의 colider

        foreach (Collider2D collider in colliders2D)
        {
            Character character = collider.GetComponent<Character>();
            if (character != null)
            {
                target.Add(character.transform);
            }
        }
    }
}
