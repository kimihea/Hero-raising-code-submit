using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
public class EquipItem
{
    public ItemInfo itemSO= new();

    // 랜덤 스텟
    public CharacterStat GradeStatModifier = new();

    //public int itemLevel;
}

public class ItemInfo
{
    public EEquipmentType equipmentType;
    public ERarityType rarity;

    public Sprite icon;
    public string itemName;

    public int upgradeStoneAmount;

    public CharacterStat PassiveStat = new();
    // 강화 시 올라가는 스텟 량.
    public CharacterStat GradeStatModifier = new();
    public bool isEmpty = true;
    
    public ItemInfo(ItemSO so)
    {
        equipmentType = so.equipmentType;
        rarity = so.rarity;
        icon = so.icon;
        itemName = so.itemName;
        upgradeStoneAmount = so.upgradeStoneAmount;
        PassiveStat = so.PassiveStat;
        GradeStatModifier = so.GradeStatModifier;
        isEmpty = false;
    }

    public ItemInfo()
    {

    }
}