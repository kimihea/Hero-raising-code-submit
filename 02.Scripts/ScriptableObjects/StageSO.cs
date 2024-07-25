using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterSpawn
{
    public string rcode;
    public int count;
}

[Serializable]
public class Wave
{
    public MonsterSpawn[] monsterSpawns;
}

[CreateAssetMenu(fileName = "STG", menuName = "Data/Stage")]
public class StageSO : ScriptableObject
{
    [field: SerializeField] public int ChapterNum { get; private set; }
    [field: SerializeField] public int StageNum { get; private set; }
    [field: SerializeField] public Sprite MapImage { get; private set; }
    [field: SerializeField] public Wave[] Waves { get; private set; }
    [field: SerializeField] public BaseStat MonsterStatModifier { get; private set; }
    [field: SerializeField] public BaseStat BossStatModifier { get; private set; }
    [field: SerializeField] public int GoldReward { get; private set; }
    [field: SerializeField] public int ManaStoneReward { get; private set; }
}