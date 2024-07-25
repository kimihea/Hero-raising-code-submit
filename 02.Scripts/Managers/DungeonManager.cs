using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private readonly float MONSTER_RESPAWN_DELAY = 2f;
    private WaitForSeconds spawnDelayTime;

    [SerializeField] private List<Transform> MonsterSpawnPositions;
    [SerializeField] private List<Transform> PlayerSpawnPositions;

    [field: SerializeField] public int DungeonNum { get; private set; }
    [field: SerializeField] public DungeonSO Data { get; private set; }
    [field: SerializeField] public int TotalClearPoint { get; private set; }
    [field: SerializeField] public int CurClearPoint { get; private set; }

    private void Start()
    {
        spawnDelayTime = new WaitForSeconds(MONSTER_RESPAWN_DELAY);
    }

    public void InitDungeon(int dungeonNum)
    {
        string rcode = "DNG" + dungeonNum.ToString("D5");
        Data = ResourceManager.Instance.GetResource<DungeonSO>(rcode, EResourceType.DATA);
        DungeonNum = Data.DungeonNum;
        TotalClearPoint = Data.TotalClearPoint;
        CurClearPoint = 0;
        for (int i = 0; i < GameManager.Instance.EntryList.Count; i++)
        {
            Character hero = GameManager.Instance.EntryList[i];
            hero.DefalutPos = PlayerSpawnPositions[i].position;
            hero.gameObject.transform.position = hero.DefalutPos;
        }
    }

    public void StartDungeon(int dungeonNum)
    {
        InitDungeon(dungeonNum);
        StartCoroutine(SpawnMonsters());
    }

    public void ClearDungeon()
    {
        foreach(Reward reward in Data.Rewards)
        {
            CurrencyManager.Instance.AddCurrency(reward.type, reward.amount);
        }
    }

    public IEnumerator SpawnMonsters()
    {
        do
        {
            for (int i = 0; i < MonsterSpawnPositions.Count; i++)
            {
                int rand = Random.Range(0, 100);
                string rcode = null;
                for (int j = 0; j < Data.SpawnInfo.Length; j++)
                {
                    if(rand < Data.SpawnInfo[j].spawnRate)
                    {
                        rcode = Data.SpawnInfo[j].rcode;
                        break;
                    }                    
                }
                Monster mob = PoolManager.Instance.SpawnFromPool(rcode).GetComponent<Monster>();
                mob.gameObject.transform.position = MonsterSpawnPositions[i].position;
                mob.DefalutPos = MonsterSpawnPositions[i].position;
                mob.StatHandler.AddStatModifier(Data.MonsterStatModifier);
                mob.InitStat();
                GameManager.Instance.Monsters.Add(mob);
            }
            yield return spawnDelayTime;
        } while (CurClearPoint < TotalClearPoint);
    }

    public void AddClearPoint(Monster mob)
    {
        int point = Data.SpawnInfo.ToList().Find(x => x.rcode == mob.name).clearPoint;
        CurClearPoint += point;
        if(CurClearPoint > TotalClearPoint)
        {
            GameManager.Instance.CombatConditionType = ECombatConditionType.END;
            GameManager.Instance.VictoryBattle();
        }
    }
}
