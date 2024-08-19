using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


[Serializable]
public class SlotInfomation
{
    public int slotLevel;
    public int slotGrade;

    // 아이템 SO
    public ItemSaveData itemInfo = new();

    public bool isEmpty = true;
}


[System.Serializable]
public class ItemSaveData
{
    // 아이템 SO
    public EEquipmentType equipmentType;
    public ERarityType rarity;
    public string icon;
    public string itemName;
    public int upgradeStoneAmount;
    public CharacterStat PassiveStat;
    public CharacterStat GradeStatModifier;
    public CharacterStat ItemRandomStat;

    public bool isEmpty = true;
}

[Serializable]
public class EquipmentSaveData
{
    public int[] slotIndex = new int[System.Enum.GetValues(typeof(EEquipmentType)).Length];
    public SlotInfomation[] slotInfo = new SlotInfomation[System.Enum.GetValues(typeof(EEquipmentType)).Length];

    public ItemSaveData switchItem;
    public bool isGachaPossible;
}

public class Equipment : MonoBehaviour
{
    //[SerializeField] private EquipItemSlot[] currentEquipment;

    public Dictionary<EEquipmentType, EquipItemSlot> currentEquipment = new();

    public Image[] ItemSlotIconArray;

    public Image[] ItemSlotBackgroundArray;

    private CharacterStat equipmentStat = new();

    Player player;

    public EquipItem switchingItem = new();

    public GameObject equipItemPanel;
    

    public Image switchingItemSprite;
    public Image switchingItemBG;
    public Text switchingItemName;
    public Text switchingItemDescription;


    public GameObject currentItemPanel;
    public Image equipItemSprite;
    public Image equipItemBG;
    public Text equipItemName;
    public Text equipItemDescription;

    public static bool isGachaPossible = true;

    
    public EquipmentSaveData EquipmentSaveData = new EquipmentSaveData();

    public Sprite[] sprites = new Sprite[4];

    private void Awake()
    {
        ItemSlotIconArray = new Image[System.Enum.GetValues(typeof(EEquipmentType)).Length];
        ItemSlotBackgroundArray = new Image[System.Enum.GetValues(typeof(EEquipmentType)).Length];
    }

    void Start()
    {
        EquipmentInitialize();

        EquipmentLoadData();                       

        StatManager.Instance.statHandler.AddStatModifier(equipmentStat);

        player = GameManager.Instance.player;

        //RefreshData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("전체 슬롯 강화");

            for (int i = 0; i < currentEquipment.Count; i++)
            {
                currentEquipment[(EEquipmentType)i].SlotLevelUp();                
            }
            EquipStatRefresh();
        }

        EquipStatRefresh();
    }


    public void Equip()
    {        
        if (currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO !=  null)
        {
            //Debug.Log("A");
            //switchingItem = currentEquipment[equipItem.itemSO.equipmentType].EquipItem;
            // 추후 분해 로직? 인벤 이동 로직?

            CurrencyManager.Instance.CurrencyDict[ECurrencyType.UpgradeStone].Add(currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.upgradeStoneAmount);

            //Debug.Log($"장비템({currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.itemName})이 분해 되었습니다. 획득한 재화 ({currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.upgradeStoneAmount}) ");

            currentEquipment[switchingItem.itemSO.equipmentType].EquipItem = switchingItem;

            

            switchingItem = new();

        }
        else
        {
            currentEquipment[switchingItem.itemSO.equipmentType].EquipItem = switchingItem;
            // switchingItem = null;
            Debug.Log("B");
            switchingItem = new();
           
        }
        //EquipItem tempItem = switchingItem;
        //currentEquipment[switchingItem.itemSO.equipmentType].EquipItem = switchingItem;

        EquipStatRefresh();

        /*if (switchingItem != null)
        {            
            ItemGrind();
        }    */    

        isGachaPossible = true;
        equipItemPanel.SetActive(false);
    }

    public void ItemGrind ()
    {
        // 기존 아이템 장착 유지

        // 새 아이템 분해
        CurrencyManager.Instance.CurrencyDict[ECurrencyType.UpgradeStone].Add(switchingItem.itemSO.upgradeStoneAmount);

        //Debug.Log($"장비템({switchingItem.itemSO.itemName})이 분해 되었습니다. 획득한 재화 ({switchingItem.itemSO.upgradeStoneAmount}) ");



        switchingItem = new();

        isGachaPossible = true;
        equipItemPanel.SetActive(false);
        //player.StatHandler.RemoveStatModifier(equipItem.itemSO.PassiveStat);
    }

    public void EquipStatRefresh()
    {
        InitEquipStat();

        for ( int i =  0; i < currentEquipment.Count;i++)
        {
            if (currentEquipment[(EEquipmentType)i].EquipItem.itemSO != null)
            {
                EquipItem item = currentEquipment[(EEquipmentType)i].EquipItem;
                ItemInfo so = item.itemSO;
                EquipItemSlot slot = currentEquipment[(EEquipmentType)i];

                equipmentStat.Atk += so.PassiveStat.Atk + item.GradeStatModifier.Atk + (slot.grade * so.GradeStatModifier.Atk) ;
                equipmentStat.Health += so.PassiveStat.Health + item.GradeStatModifier.Health + (slot.grade * so.GradeStatModifier.Health);
                equipmentStat.Defense += so.PassiveStat.Defense + item.GradeStatModifier.Defense + (slot.grade * so.GradeStatModifier.Defense);
                equipmentStat.AttackSpeed += so.PassiveStat.AttackSpeed + item.GradeStatModifier.AttackSpeed + (slot.grade * so.GradeStatModifier.AttackSpeed);
                equipmentStat.CritRate += so.PassiveStat.CritRate + item.GradeStatModifier.CritRate + (slot.grade * so.GradeStatModifier.CritRate);
                equipmentStat.CritMultiplier += so.PassiveStat.CritMultiplier + item.GradeStatModifier.CritMultiplier + (slot.grade * so.GradeStatModifier.CritMultiplier);
                equipmentStat.SkillMultiplier += so.PassiveStat.SkillMultiplier + item.GradeStatModifier.SkillMultiplier + (slot.grade * so.GradeStatModifier.SkillMultiplier);
                equipmentStat.DamageMultiplier += so.PassiveStat.DamageMultiplier + item.GradeStatModifier.DamageMultiplier + (slot.grade * so.GradeStatModifier.DamageMultiplier);
                equipmentStat.HealMultiplier += so.PassiveStat.HealMultiplier + item.GradeStatModifier.HealMultiplier + (slot.grade * so.GradeStatModifier.HealMultiplier);
            }
        }

        for (int i = 0; i < currentEquipment.Count; i++)
        {    
            if (currentEquipment[(EEquipmentType)i].EquipItem.itemSO.isEmpty == false)
            {
                ItemSlotIconArray[i].enabled = true;
                ItemSlotBackgroundArray[i].enabled = true;

                ItemSlotIconArray[i].sprite = currentEquipment[(EEquipmentType)i].EquipItem.itemSO.icon;

                ItemRarityBG(currentEquipment[(EEquipmentType)i].EquipItem, ItemSlotBackgroundArray[i]);
                                
            }
            else
            {
                ItemSlotIconArray[i].enabled = false;
                ItemSlotBackgroundArray[i].enabled = false;
            }
        }

        RefreshData();
    }

    public void InitEquipStat()
    {
        equipmentStat.Atk = 0;
        equipmentStat.Health = 0;
        equipmentStat.Defense = 0;
        equipmentStat.AttackSpeed = 0;

        equipmentStat.CritRate = 0;
        equipmentStat.CritMultiplier = 0;
        equipmentStat.SkillMultiplier = 0;
        equipmentStat.DamageMultiplier = 0;
        equipmentStat.HealMultiplier = 0;
    }

    public void OpenPopUP()
    {
        // 팝업창 액티브 하고
        //OnButtonClick();

        equipItemPanel.SetActive(true);

        // 이미지 셋팅
        switchingItemSprite.sprite = switchingItem.itemSO.icon;

        ItemRarityBG(switchingItem, switchingItemBG);
        
        switchingItemName.text = switchingItem.itemSO.itemName;

        SetItemDescription(switchingItemDescription, switchingItem);
                
        // 첫 장착시 교체 창이 보이지 않아야 함...
        if (currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.isEmpty == false)
        {
            currentItemPanel.SetActive(true);

            EquipItem currentEquipItem = currentEquipment[switchingItem.itemSO.equipmentType].EquipItem;

            equipItemSprite.sprite = currentEquipItem.itemSO.icon;
            ItemRarityBG(currentEquipItem, equipItemBG);

            equipItemName.text = currentEquipItem.itemSO.itemName;

            //SetItemDescription(equipItemDescription, currentEquipItem);

            // 나중에 꼭 손볼 것

            switchingItemDescription.text = string.Empty;
            equipItemDescription.text = string.Empty;

            StringBuilder switchingString = new();
            StringBuilder currentString = new();

            float switchingAtk = switchingItem.itemSO.PassiveStat.Atk + switchingItem.GradeStatModifier.Atk;
            float currentAtk = currentEquipItem.itemSO.PassiveStat.Atk + currentEquipItem.GradeStatModifier.Atk;

            float switchingHealth = switchingItem.itemSO.PassiveStat.Health + switchingItem.GradeStatModifier.Health;
            float currentHealth = currentEquipItem.itemSO.PassiveStat.Health + currentEquipItem.GradeStatModifier.Health;

            float switchingDefense = switchingItem.itemSO.PassiveStat.Defense + switchingItem.GradeStatModifier.Defense;
            float currentDefense = currentEquipItem.itemSO.PassiveStat.Defense + currentEquipItem.GradeStatModifier.Defense;

            float switchingAttackSpeed = switchingItem.itemSO.PassiveStat.AttackSpeed + switchingItem.GradeStatModifier.AttackSpeed;
            float currentAttackSpeed = currentEquipItem.itemSO.PassiveStat.AttackSpeed + currentEquipItem.GradeStatModifier.AttackSpeed;

            float switchingCritRate = switchingItem.itemSO.PassiveStat.CritRate + switchingItem.GradeStatModifier.CritRate;
            float currentCritRate = currentEquipItem.itemSO.PassiveStat.CritRate + currentEquipItem.GradeStatModifier.CritRate;

            Test(switchingAtk, currentAtk, switchingString, currentString, "공격력");
            Test(switchingHealth, currentHealth, switchingString, currentString, "생명력");
            Test(switchingDefense, currentDefense, switchingString, currentString, "방어력");
            Test(switchingAttackSpeed, currentAttackSpeed, switchingString, currentString, "공격 속도");
            Test(switchingCritRate, currentCritRate, switchingString, currentString, "치명타 확률");

            switchingItemDescription.text = switchingString.ToString();
            equipItemDescription.text = currentString.ToString();
        }
        else
        {
            currentItemPanel.SetActive(false);
        }

        
    }

    private void SetItemDescription(Text ItemDescription, EquipItem item)
    {
        StringBuilder stringBuilder = new ();

        if (item.itemSO.PassiveStat.Atk != 0)
        {
            stringBuilder.Append($"공격력 : {item.itemSO.PassiveStat.Atk + item.GradeStatModifier.Atk}\n");
        }

        if (item.itemSO.PassiveStat.Health != 0)
        {
            stringBuilder.Append($"생명력 : {item.itemSO.PassiveStat.Health + item.GradeStatModifier.Health}\n");
        }

        if (item.itemSO.PassiveStat.Defense != 0)
        {
            stringBuilder.Append($"방어력 : {item.itemSO.PassiveStat.Defense + item.GradeStatModifier.Defense}\n");
        }

        if (item.itemSO.PassiveStat.AttackSpeed != 0)
        {
            stringBuilder.Append($"공격 속도 : {item.itemSO.PassiveStat.AttackSpeed + item.GradeStatModifier.AttackSpeed}\n");
        }

        if (item.itemSO.PassiveStat.CritRate != 0)
        {
            stringBuilder.Append($"치명타 확률 : {item.itemSO.PassiveStat.CritRate + item.GradeStatModifier.CritRate}\n");
        }

        ItemDescription.text = stringBuilder.ToString();


    }        

    private void EquipmentLoadData()
    {
        //Debug.Log($"<color=red> 장비 로드 데이터 </color>");
        EquipmentSaveData loadData = DataManager.Instance.LoadData<EquipmentSaveData>(ESaveType.EQUIPMENT);

         if (loadData == null)
         {
             //Debug.Log($"<color=red> 장비 이니셜라이즈 </color>");
         }
         else
         {
            //Debug.Log($"<color=red> 장비 로드 </color>");

            LoadEquipmentData(loadData, currentEquipment, ref switchingItem);

        }
    }

    private void EquipmentInitialize()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(EEquipmentType)).Length; i++)
        {
            EquipItemSlot equipItemSlot = new();

            /*GameObject go = new GameObject($"{i} object");
            go.transform.parent = this.transform;
            equipItemSlot.StatHandlerObject = go;
            equipItemSlot.StatHandler = equipItemSlot.StatHandlerObject.AddComponent<StatHandler>();*/

            currentEquipment.Add((EEquipmentType)i, equipItemSlot);
        }
    }

    private void SaveEquipmentData()
    {       
        for (int i = 0; i < System.Enum.GetValues(typeof(EEquipmentType)).Length; i++)
        {            
            SlotInfomation slotInfo = new SlotInfomation();

            slotInfo.slotLevel = currentEquipment[(EEquipmentType)i].slotLevel;
            //slotInfo.StatHandler = currentEquipment[(EEquipmentType)i].StatHandler;
            slotInfo.slotGrade = currentEquipment[(EEquipmentType)i].grade;


            slotInfo.itemInfo.ItemRandomStat = currentEquipment[(EEquipmentType)i].EquipItem.GradeStatModifier;
            ItemInfo so = currentEquipment[(EEquipmentType)i].EquipItem.itemSO;            

            if (so.isEmpty)
            {
                slotInfo.isEmpty = true;
                continue;
            }

            slotInfo.isEmpty = false;
            slotInfo.itemInfo.equipmentType = so.equipmentType;
            slotInfo.itemInfo.rarity = so.rarity;
            slotInfo.itemInfo.icon =  DataManager.Instance.ImageToString(so.icon);
            slotInfo.itemInfo.itemName = so.itemName;
            slotInfo.itemInfo.upgradeStoneAmount = so.upgradeStoneAmount;
            slotInfo.itemInfo.PassiveStat = so.PassiveStat;
            slotInfo.itemInfo.GradeStatModifier = so.GradeStatModifier;



            EquipmentSaveData.slotIndex[i] = i;
            EquipmentSaveData.slotInfo[i] = slotInfo;
        }

        ItemSaveData switchItem = new ItemSaveData();

        if (switchingItem.itemSO.isEmpty == false)
        {
            switchItem.equipmentType = switchingItem.itemSO.equipmentType;
            switchItem.rarity = switchingItem.itemSO.rarity;
            switchItem.icon = DataManager.Instance.ImageToString(switchingItem.itemSO.icon);
            switchItem.itemName = switchingItem.itemSO.itemName;
            switchItem.upgradeStoneAmount = switchingItem.itemSO.upgradeStoneAmount;
            switchItem.PassiveStat = switchingItem.itemSO.PassiveStat;
            switchItem.GradeStatModifier = switchingItem.itemSO.GradeStatModifier;
            switchItem.ItemRandomStat = switchingItem.GradeStatModifier;

            switchItem.isEmpty = false;
        }

        //dictionaryConvert.switchItem

        EquipmentSaveData.switchItem = switchItem;

        EquipmentSaveData.isGachaPossible = isGachaPossible;

    }

    

    public void RefreshData()
    {
        StatManager.Instance.statHandler.UpdateStatModifier();
        player.StatHandler.UpdateStatModifier();
        GameManager.Instance.HeroUpdate();
        SaveEquipmentData();
    }

    private void LoadEquipmentData(EquipmentSaveData loadData , Dictionary<EEquipmentType, EquipItemSlot> currentEquipment, ref EquipItem switchingItem)
    {
        EquipmentSaveData = loadData;

        for (int i = 0; i < System.Enum.GetValues(typeof(EEquipmentType)).Length; i++)
        {
            EquipItemSlot slot = currentEquipment[(EEquipmentType)i];

            slot.slotLevel = loadData.slotInfo[i].slotLevel;
            slot.grade = loadData.slotInfo[i].slotGrade;

            slot.EquipItem.GradeStatModifier = loadData.slotInfo[i].itemInfo.ItemRandomStat;            

            if (loadData.slotInfo[i].isEmpty)
            {                
                slot.EquipItem.itemSO.isEmpty = true;
                continue;
            }

            slot.EquipItem.itemSO = new();

            

            if (loadData.slotInfo[i].isEmpty == false)
            {
                slot.EquipItem.itemSO.equipmentType = loadData.slotInfo[i].itemInfo.equipmentType;
                slot.EquipItem.itemSO.rarity = loadData.slotInfo[i].itemInfo.rarity;
                slot.EquipItem.itemSO.icon = DataManager.Instance.StringToImage(loadData.slotInfo[i].itemInfo.icon);
                slot.EquipItem.itemSO.itemName = loadData.slotInfo[i].itemInfo.itemName;
                slot.EquipItem.itemSO.upgradeStoneAmount = loadData.slotInfo[i].itemInfo.upgradeStoneAmount;
                slot.EquipItem.itemSO.PassiveStat = loadData.slotInfo[i].itemInfo.PassiveStat;
                slot.EquipItem.itemSO.GradeStatModifier = loadData.slotInfo[i].itemInfo.GradeStatModifier;

                slot.EquipItem.itemSO.isEmpty = loadData.slotInfo[i].isEmpty = false;
            }

            

            //ItemSO so = currentEquipment[(EEquipmentType)i].EquipItem.itemSO;

            /* if (so == null)
             {
                 continue;
             }*/

            /*slotInfo.equipmentType = so.equipmentType;
            slotInfo.rarity = so.rarity;
            slotInfo.icon = ImageToString(so.icon);
            slotInfo.itemName = so.itemName;
            slotInfo.upgradeStoneAmount = so.upgradeStoneAmount;
            slotInfo.PassiveStat = so.PassiveStat;
            slotInfo.GradeStatModifier = so.GradeStatModifier;

            dictionaryConvert.slotIndex[i] = i;
            dictionaryConvert.slotInfo[i] = slotInfo;*/

            

            //currentEquipment[(EEquipmentType)i] = slot;
        }

        if (loadData.switchItem.isEmpty == false)
        {
            switchingItem.itemSO.isEmpty = false;

            switchingItem.itemSO.equipmentType = loadData.switchItem.equipmentType;
            switchingItem.itemSO.rarity = loadData.switchItem.rarity;
            switchingItem.itemSO.icon = DataManager.Instance.StringToImage(loadData.switchItem.icon);
            switchingItem.itemSO.itemName = loadData.switchItem.itemName;
            switchingItem.itemSO.upgradeStoneAmount = loadData.switchItem.upgradeStoneAmount;
            switchingItem.itemSO.PassiveStat = loadData.switchItem.PassiveStat;
            switchingItem.itemSO.GradeStatModifier = loadData.switchItem.GradeStatModifier;

            switchingItem.GradeStatModifier = loadData.switchItem.ItemRandomStat;
        }

        isGachaPossible = loadData.isGachaPossible;

    }

    public void Test(float switchingStat, float currentgStat, StringBuilder switchingString, StringBuilder currentString, string StatName)
    {       
        if (switchingStat == 0 && currentgStat== 0)
        {
            return;
        }

        //switchingItemDescription, equipItemDescription
        if (switchingStat == 0)
        {
            currentString.Append($"<color=green>{StatName} : {currentgStat}</color>\n");
            return;
        }

        if (currentgStat == 0)
        {
            switchingString.Append($"<color=green>{StatName} : {switchingStat}</color>\n");
            return;
        }



        if (switchingStat > currentgStat)
        {
            switchingString.Append($"<color=green>{StatName} : {switchingStat}</color>\n");
            currentString.Append($"<color=red>{StatName} : {currentgStat}</color>\n");
        }
        else if (switchingStat == currentgStat)
        {
            switchingString.Append($"<color=gray>{StatName} : {switchingStat}</color>\n");
            currentString.Append($"<color=gray>{StatName} : {currentgStat}</color>\n");
        }
        else if (switchingStat < currentgStat)
        {
            switchingString.Append($"<color=red>{StatName} : {switchingStat}</color>\n");
            currentString.Append($"<color=green>{StatName} : {currentgStat}</color>\n");
        }
        return;
    }

    public void ItemRarityBG(EquipItem item, Image background)
    {
        switch (item.itemSO.rarity)
        {
            case ERarityType.COMMON:
                background.sprite = sprites[0];
                break;
            case ERarityType.RARE:
                background.sprite = sprites[1];
                break;
            case ERarityType.EPIC:
                background.sprite = sprites[2];
                break;
            case ERarityType.LEGEND:
                background.sprite = sprites[3];
                break;
            default:
                break;
        }
    }

    public int TotalUpgradeNum()
    {
        int total = 0;
        foreach (var equip in currentEquipment)
        {
            total += equip.Value.slotLevel;
        }
        return total;
    }
}

