using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName ="New Item", menuName = "Item")]
public class ItemSO : ScriptableObject
{
    public EEquipmentType equipmentType;
    public ERarityType rarity;
       
    public Sprite icon;
    public string itemName;

    public int upgradeStoneAmount;

    public CharacterStat PassiveStat = new();

    // 강화 시 올라가는 스텟 량.
    public CharacterStat GradeStatModifier = new();
    // ???
    //public CharacterStat PassiveStat
}
