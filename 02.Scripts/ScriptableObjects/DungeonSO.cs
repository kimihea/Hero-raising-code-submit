using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DungeonSpawn
{
    public string rcode;
    [Range(0, 100)] public int spawnRate;
    public int clearPoint;
}

[Serializable]
public class Reward
{
    public ECurrencyType type;
    public int amount;
}

[CreateAssetMenu(fileName = "DNG", menuName = "Data/Dungeon")]
public class DungeonSO : ScriptableObject
{
    [field: SerializeField] public int DungeonNum { get; private set; }
    //[field: SerializeField] public Sprite MapImage { get; private set; }
    [field: SerializeField] public DungeonSpawn[] SpawnInfo { get; private set; }
    [field: SerializeField] public int TotalClearPoint { get; private set; }
    [field: SerializeField] public BaseStat MonsterStatModifier { get; private set; }
    //[field: SerializeField] public BaseStat BossStatModifier { get; private set; }
    [field: SerializeField] public Reward[] Rewards { get; private set; }    
}
