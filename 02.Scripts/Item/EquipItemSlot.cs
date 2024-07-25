using UnityEditor.PackageManager;
using UnityEngine;

public class EquipItemSlot
{
    public EquipItem EquipItem = new();

    public StatHandler StatHandler= new();

    public int slotLevel = 1;

    public int slotExp = 100;
    public int curSlotExp = 0;

    public void SlotLevelUp()
    {
        slotExp *= 2;
        curSlotExp = 0;

        slotLevel++;
        StatHandler.grade++;

        StatManager.Instance.statHandler.UpdateStatModifier();
        GameManager.Instance.player.StatHandler.UpdateStatModifier();
        GameManager.Instance.HeroUpdate();
    }

}
