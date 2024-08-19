using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipTooltipWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private GameObject slotUpgradePanel; // SlotUpgradPanel 오브젝트를 추가

    private Button slotUpgradeButton;

    public int index;

    public SlotUpgradePanelController SlotUpgradePanelController;

    private void Awake()
    {
        HideTooltip();

        // 자식의 자식 중 SlotUpgradeBtn이라는 이름의 버튼을 찾음
        slotUpgradeButton = transform.Find("SlotUpgradeBtn").GetComponent<Button>();

        if (slotUpgradeButton != null)
        {
            slotUpgradeButton.onClick.AddListener(OnSlotButtonClick);
        }

        SlotUpgradePanelController = slotUpgradePanel.GetComponent<SlotUpgradePanelController>();
    }

    public void ShowTooltip(string header, string description)
    {
        headerText.text = header;
        descriptionText.text = description;
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void OnSlotButtonClick()
    {
        //Debug.Log($"인덱스 확인 {index}");

        // 디버그 로그 추가
        //Debug.Log("SlotButton clicked");

        // 툴팁을 토글
        if (gameObject.activeSelf)
        {
            HideTooltip();
            //Debug.Log("Tooltip hidden");
        }
        else
        {
            ShowTooltip(headerText.text, descriptionText.text); // 필요한 경우 header와 description을 업데이트
            //Debug.Log("Tooltip shown");
        }

        // SlotUpgradPanel 오브젝트를 활성화
        if (slotUpgradePanel != null)
        {
            slotUpgradePanel.SetActive(true);

            SlotUpgradePanelController.index = index;
            SlotUpgradePanelController.RefreshUI();
            Debug.Log("slotUpgradePanel activated");
        }
        else
        {
            Debug.LogError("slotUpgradePanel이 설정되지 않았습니다.");
        }
    }
}
