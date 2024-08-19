using UnityEditor.PackageManager;
using UnityEngine;

[System.Serializable]
public class EquipItemSlot
{
    public EquipItem EquipItem = new();

    //public StatHandler StatHandler= new();

    public int slotLevel = 0;

    public int slotExp = 100;
    public int curSlotExp = 0;

    public int UpgradeDefaultCost = 50;
    public int UpgradeIncreaseCost = 10;

    public int cost = 0;

    public int grade = 0;    

    // 또는 코스트
    /*public GameObject StatHandlerObject;

    public StatHandler StatHandler;*/

    public void SlotLevelUp()
    {       

        slotExp *= 2;
        curSlotExp = 0;

        slotLevel++;
        //StatHandler.grade++;
        grade++;
        QuestManager.Instance.AddProgress(EQuestType.EQUIPUPGRADE, 1);

        StatManager.Instance.statHandler.UpdateStatModifier();
        GameManager.Instance.player.StatHandler.UpdateStatModifier();
        GameManager.Instance.HeroUpdate();
    }

}
