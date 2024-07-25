using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EResourceType
{
    PREFAB,
    DATA,
    AUDIO
}

public enum ECurrencyType
{
    ManaStone,
    ManaStoneFragment,
    Gold,
    UpgradeStone,
    Diamond,
    HeroEssence
}

public enum EStatChangeType
{
    ADD,    
    GRADE,
    STARS,
    MULTIPLE,
    OVERRIDE
}

public enum ERoleType
{
    TANKER = 0,
    HEALER =2,
    DEALER =1
}

public enum EEquipmentType
{
    WEAPON = 0,
    HEAD = 1,
    CHEST = 2,
    HANDS = 3,
    LEGS = 4,
    FOOT = 5,
}

public enum ERarityType
{
    COMMON = 1,
    RARE = 2,
    EPIC = 3,
    LEGEND = 4,
}

public enum EEntityType
{
    PLAYER,
    MONSTER
}

public enum EStatType
{
    ATK = 0,
    HEALTH = 1,
    DEFENSE = 2,
    ATKSPEED = 3,

    CRITRATE = 4,
    CRITMULTIPLIER = 5,
    SKILLMULTIPLIER = 6,
    DAMAGEMULTIPLIER = 7,
    HEALMULTIPLIER = 8
}

public enum EBattleType
{
    STAGE,
    DUNGEON,
    AWAKEN
}

public enum ECombatConditionType
{
    START,
    END,    
    READY
}

public enum EAudioMixerType
{
    MASTER,
    BGM,
    SFX
}

public enum ESkillType
{
    PROJECTILE,
    AOE, //Area of Effect
    BUFF
}






