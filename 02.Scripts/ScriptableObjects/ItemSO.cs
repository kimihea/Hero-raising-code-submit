using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName = "Item")]
public class ItemSO : ScriptableObject
{
    public EEquipmentType equipmentType;
    public ERarityType rarity;
       
    public Sprite icon;
    public string itemName;

    public int upgradeStoneAmount;

    public CharacterStat PassiveStat = new();

    public CharacterStat GradeStatModifier = new();
    // ???
    //public CharacterStat PassiveStat
}
