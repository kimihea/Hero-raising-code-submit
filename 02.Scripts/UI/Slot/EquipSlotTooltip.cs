using System.Text;
using UnityEngine;
using UnityEngine.UI; // Button 컴포넌트를 사용하기 위해 포함

public class EquipSlotTooltip : MonoBehaviour
{
    [SerializeField] private EquipTooltipWindow equipTooltipWindowPrefab; // 프리팹으로 툴팁 창을 설정
    [SerializeField] private string header;
    [SerializeField] private string description;
    [SerializeField] private Button button; // Button 컴포넌트를 사용

    private EquipTooltipWindow equipTooltipWindowInstance; // 툴팁 창 인스턴스
    private static EquipTooltipWindow currentActiveTooltip; // 현재 활성화된 툴팁
    private static EquipSlotTooltip currentActiveSlot; // 현재 활성화된 슬롯

    [SerializeField] private int slotIndex;

    private void Start()
    {
        if (equipTooltipWindowPrefab == null)
        {
            Debug.LogError("equipTooltipWindowPrefab가 설정되지 않았습니다.");
        }

        if (button == null)
        {
            Debug.LogError("Button 컴포넌트가 설정되지 않았습니다.");
        }
        else
        {
            button.onClick.AddListener(OnButtonClick); // OnClick 이벤트에 OnButtonClick 메서드를 등록
        }

        // 툴팁 창 인스턴스를 생성하고 초기 상태를 설정
        equipTooltipWindowInstance = Instantiate(equipTooltipWindowPrefab, transform);
        equipTooltipWindowInstance.gameObject.SetActive(false);
    }

    private void OnButtonClick()
    {
        if (currentActiveSlot == this)
        {
            // 현재 활성화된 슬롯이 자신인 경우 툴팁을 토글
            ToggleTooltip();
        }
        else
        {
            // 다른 슬롯이 활성화된 경우 현재 활성화된 툴팁을 숨기고 새로운 툴팁을 표시
            if (currentActiveSlot != null)
            {
                currentActiveSlot.HideTooltip();
            }
            ShowTooltip();
            currentActiveSlot = this;
        }
    }

    public void ToggleTooltip()
    {
        if (isTooltipVisible())
        {
            HideTooltip();
            currentActiveSlot = null;
        }
        else
        {
            ShowTooltip();
            currentActiveSlot = this;
        }
    }

    private bool isTooltipVisible()
    {
        return equipTooltipWindowInstance.gameObject.activeSelf;
    }

    private void ShowTooltip()
    {
        EquipItem item = StatManager.Instance.equipment.currentEquipment[(EEquipmentType)slotIndex].EquipItem;
        ItemInfo itemInfo = item.itemSO;
        int slotLevel = StatManager.Instance.equipment.currentEquipment[(EEquipmentType)slotIndex].slotLevel;


        if (itemInfo.isEmpty == false)
        {
            header = $"{itemInfo.itemName}  +{slotLevel}";

            StringBuilder stringBuilder = new();

            if (itemInfo.PassiveStat.Atk != 0)
            {
                stringBuilder.Append($"공격력 : {itemInfo.PassiveStat.Atk + item.GradeStatModifier.Atk + (itemInfo.GradeStatModifier.Atk * slotLevel)}\n");
            }

            if (itemInfo.PassiveStat.Health != 0)
            {
                stringBuilder.Append($"생명력 : {itemInfo.PassiveStat.Health + item.GradeStatModifier.Health + (itemInfo.GradeStatModifier.Health * slotLevel)}\n");
            }

            if (itemInfo.PassiveStat.Defense != 0)
            {
                stringBuilder.Append($"방어력 : {itemInfo.PassiveStat.Defense + item.GradeStatModifier.Defense + (itemInfo.GradeStatModifier.Defense * slotLevel)}\n");
            }

            if (itemInfo.PassiveStat.AttackSpeed != 0)
            {
                stringBuilder.Append($"공격 속도 : {itemInfo.PassiveStat.AttackSpeed + item.GradeStatModifier.AttackSpeed + (itemInfo.GradeStatModifier.AttackSpeed * slotLevel)}\n");
            }

            if (itemInfo.PassiveStat.CritRate != 0)
            {
                stringBuilder.Append($"치명타 확률 : {itemInfo.PassiveStat.CritRate + item.GradeStatModifier.CritRate + (itemInfo.GradeStatModifier.CritRate * slotLevel)}\n");
            }
            description = stringBuilder.ToString();
        }

        // 현재 활성화된 툴팁이 있으면 비활성화
        if (currentActiveTooltip != null)
        {
            currentActiveTooltip.HideTooltip();
        }

        if (equipTooltipWindowInstance != null)
        {
            equipTooltipWindowInstance.ShowTooltip(header, description);

            equipTooltipWindowInstance.gameObject.SetActive(true);
            equipTooltipWindowInstance.index = slotIndex;

            currentActiveTooltip = equipTooltipWindowInstance; // 현재 활성화된 툴팁 갱신
        }
    }

    private void HideTooltip()
    {
        if (equipTooltipWindowInstance != null)
        {
            equipTooltipWindowInstance.HideTooltip();

            // 현재 활성화된 툴팁이 자신인 경우 null로 설정
            if (currentActiveTooltip == equipTooltipWindowInstance)
            {
                currentActiveTooltip = null;
            }
        }
    }
}
