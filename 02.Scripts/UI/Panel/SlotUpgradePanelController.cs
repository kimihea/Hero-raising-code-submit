using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotUpgradePanelController : MonoBehaviour
{
    public int index;

    public Text costText;

    int defaultCost;
    int increaseCost;
    int slotLevel;
    int cost;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickUpgrade()
    {
        if (CurrencyManager.Instance.CurrencyDict[ECurrencyType.UpgradeStone].TrySpend(cost))
        {
            StatManager.Instance.equipment.currentEquipment[(EEquipmentType)index].SlotLevelUp();
            StatManager.Instance.equipment.EquipStatRefresh();
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        defaultCost = StatManager.Instance.equipment.currentEquipment[(EEquipmentType)index].UpgradeDefaultCost;
        increaseCost = StatManager.Instance.equipment.currentEquipment[(EEquipmentType)index].UpgradeIncreaseCost;
        slotLevel = StatManager.Instance.equipment.currentEquipment[(EEquipmentType)index].slotLevel;

        cost = defaultCost + (slotLevel * increaseCost);

        costText.text = $"강화석 {cost} 개 필요";
    }
}
