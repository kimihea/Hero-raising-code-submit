using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAddressableType
{
    PREFAB,
    DATA,
    AUDIO,
    IMAGE,
    UI
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
    /*WEAPON = 0,
    HEAD = 1,
    CHEST = 2,
    HANDS = 3,
    LEGS = 4,
    FOOT = 5,*/

    HEAD = 0,
    CHEST = 1,
    WEAPON = 2,
    LEGS = 3,
    FOOT = 4,
    HANDS = 5,
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
    GOLDDUNGEON,
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
    BUFF,
    Heal
}
public enum EAlertType
{
    LACK,
    NOTIMPLEMENTED,
    SUCCESS,
    CHAPTER,
}

public enum ESkillMotion
{
    MOTION1,
    MOTION2,
}





