using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class Equipment : MonoBehaviour
{
    //[SerializeField] private EquipItemSlot[] currentEquipment;

    public Dictionary<EEquipmentType, EquipItemSlot> currentEquipment = new();

    public Image[] ItemSlot;

    private CharacterStat equipmentStat = new();

    Player player;

    public EquipItem switchingItem = new();

    public GameObject equipItemPanel;
    

    public Image switchingItemSprite;
    public Text switchingItemName;
    public Text switchingItemDescription;


    public GameObject currentItemPanel;
    public Image equipItemSprite;
    public Text equipItemName;
    public Text equipItemDescription;

    public bool isGachaPossible = true;
    private void Awake()
    {
        

        ItemSlot = new Image[System.Enum.GetValues(typeof(EEquipmentType)).Length];
    }

    // Start is called before the first frame update
    void Start()
    {     
        for ( int i = 0; i < System.Enum.GetValues(typeof(EEquipmentType)).Length; i++ )
        {
            EquipItemSlot equipItemSlot = new EquipItemSlot();

            currentEquipment.Add((EEquipmentType)i, equipItemSlot);
        }


        StatManager.Instance.statHandler.AddStatModifier(equipmentStat);

        player = GameManager.Instance.player;
    }

    private void Update()
    {
        /*for (int i = 0; i < currentEquipment.Count; i++)
        {
            if (currentEquipment[ (EEquipmentType) i].EquipItem.itemSO != null)
            {
                ItemSlot[i].sprite = currentEquipment[(EEquipmentType)i].EquipItem.itemSO.icon;
            }
        }*/


        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("전체 슬롯 강화");

            for (int i = 0; i < currentEquipment.Count; i++)
            {
                currentEquipment[(EEquipmentType)i].SlotLevelUp();                
            }
        }
    }


    public void Equip()
    {        
        if (currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO !=  null)
        {
            Debug.Log("이미 착용중인 장착 아이템이 있습니다.");
            //switchingItem = currentEquipment[equipItem.itemSO.equipmentType].EquipItem;
            // 추후 분해 로직? 인벤 이동 로직?

            EquipItem tempItem = currentEquipment[switchingItem.itemSO.equipmentType].EquipItem;

            currentEquipment[switchingItem.itemSO.equipmentType].EquipItem = switchingItem;

            switchingItem = tempItem;

        }
        else
        {
            currentEquipment[switchingItem.itemSO.equipmentType].EquipItem = switchingItem;
            switchingItem = null;
        }
        //EquipItem tempItem = switchingItem;
        //currentEquipment[switchingItem.itemSO.equipmentType].EquipItem = switchingItem;

        EquipStatRefresh();

        if (switchingItem != null)
        {
            //OpenPopUP();
            ItemGrind();
        }
        else
        {
            //equipItemPanel.SetActive(false);
        }

        isGachaPossible = true;
        equipItemPanel.SetActive(false);


    }

    public void SwitchingItemEquip()
    {

    }

    public void ItemGrind ()
    {
        // 기존 아이템 장착 유지

        // 새 아이템 분해
        CurrencyManager.Instance.CurrencyDict[ECurrencyType.UpgradeStone].Add(switchingItem.itemSO.upgradeStoneAmount);

        Debug.Log($"장비템이 분해 되었습니다. 획득한 재화 ({switchingItem.itemSO.name}) ");
        Debug.Log($"장비템이 분해 되었습니다. 획득한 재화 ({switchingItem.itemSO.upgradeStoneAmount}) ");

        switchingItem = null;


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
                ItemSO so = currentEquipment[(EEquipmentType)i].EquipItem.itemSO;
                StatHandler handler = currentEquipment[(EEquipmentType)i].StatHandler;

                equipmentStat.Atk += so.PassiveStat.Atk + (handler.grade * so.GradeStatModifier.Atk) ;
                equipmentStat.Health += so.PassiveStat.Health + (handler.grade * so.GradeStatModifier.Health);
                equipmentStat.Defense += so.PassiveStat.Defense + (handler.grade * so.GradeStatModifier.Defense);
                equipmentStat.AttackSpeed += so.PassiveStat.AttackSpeed + (handler.grade * so.GradeStatModifier.AttackSpeed);
                equipmentStat.CritRate += so.PassiveStat.CritRate + (handler.grade * so.GradeStatModifier.CritRate);
                equipmentStat.CritMultiplier += so.PassiveStat.CritMultiplier + (handler.grade * so.GradeStatModifier.CritMultiplier);
                equipmentStat.SkillMultiplier += so.PassiveStat.SkillMultiplier + (handler.grade * so.GradeStatModifier.SkillMultiplier);
                equipmentStat.DamageMultiplier += so.PassiveStat.DamageMultiplier + (handler.grade * so.GradeStatModifier.DamageMultiplier);
                equipmentStat.HealMultiplier += so.PassiveStat.HealMultiplier + (handler.grade * so.GradeStatModifier.HealMultiplier);
            }
        }

        for (int i = 0; i < currentEquipment.Count; i++)
        {
            if (currentEquipment[(EEquipmentType)i].EquipItem.itemSO != null)
            {
                ItemSlot[i].sprite = currentEquipment[(EEquipmentType)i].EquipItem.itemSO.icon;
            }
        }


        StatManager.Instance.statHandler.UpdateStatModifier();
        player.StatHandler.UpdateStatModifier();
        GameManager.Instance.HeroUpdate();
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
        switchingItemName.text = switchingItem.itemSO.itemName;
        switchingItemDescription.text = "";

        switchingItemDescription.text += (switchingItem.itemSO.PassiveStat.Atk != 0) ? $"공격력 : {switchingItem.itemSO.PassiveStat.Atk} " : "";
        switchingItemDescription.text += (switchingItem.itemSO.PassiveStat.Health != 0) ? $"생명력 : {switchingItem.itemSO.PassiveStat.Health} " : "";
        switchingItemDescription.text += (switchingItem.itemSO.PassiveStat.Defense != 0) ? $"방어력 : {switchingItem.itemSO.PassiveStat.Defense} " : "";
        switchingItemDescription.text += (switchingItem.itemSO.PassiveStat.AttackSpeed != 0) ? $"공속 속도 : {switchingItem.itemSO.PassiveStat.AttackSpeed} " : "";
        switchingItemDescription.text += (switchingItem.itemSO.PassiveStat.CritRate != 0) ? $"치명타 확률 : {switchingItem.itemSO.PassiveStat.CritRate} " : "";

        // 첫 장착시 교체 창이 보이지 않아야 함...
        if (currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO != null)
        {
            currentItemPanel.SetActive(true);

            equipItemSprite.sprite = currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.icon;
            equipItemName.text = currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.itemName;
            equipItemDescription.text = "";

            equipItemDescription.text += (currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.PassiveStat.Atk != 0) ? $"공격력 : {currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.PassiveStat.Atk} " : "";
            equipItemDescription.text += (currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.PassiveStat.Health != 0) ? $"생명력 : {currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.PassiveStat.Health} " : "";
            equipItemDescription.text += (currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.PassiveStat.Defense != 0) ? $"방어력 : {currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.PassiveStat.Defense} " : "";
            equipItemDescription.text += (currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.PassiveStat.AttackSpeed != 0) ? $"공속 속도 : {currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.PassiveStat.AttackSpeed} " : "";
            equipItemDescription.text += (currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.PassiveStat.CritRate != 0) ? $"치명타 확률 : {currentEquipment[switchingItem.itemSO.equipmentType].EquipItem.itemSO.PassiveStat.CritRate} " : "";
        }
        else
        {
            currentItemPanel.SetActive(false);
        }

        
    }


}
