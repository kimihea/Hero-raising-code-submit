using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGoldDungeonMenu : MonoBehaviour
{
    public Text TicketCountTxt;
    public TextMeshProUGUI SelectedLevelTxt;
    public Text RewardTxt;
    public Button LeftBtn;
    public Button RightBtn;
    public Button ChallengeBtn;
    public Button SweepBtn;
    public GameObject Menu;
    public UISweepPanel SweepPanel;

    [SerializeField] private int SelectedLevel;
    private GoldDungeonManager DungeonManager;
    private bool isInit;

    private void OnEnable()
    {
        if (!isInit) return;
        UpdateUI();
    }

    private void Start()
    {        
        DungeonManager = GameManager.Instance.GoldDungeon;
        SelectedLevel = Mathf.Min(GameManager.Instance.GoldDungeon.SaveData.LevelNum + 1, DungeonManager.MAX_DUNGEON_COUNT);
        UpdateUI();
        isInit = true;
    }

    public void OnLeftBtnClick()
    {
        SelectedLevel -= 1;
        UpdateUI();
    }

    public void OnRightBtnClick()
    {
        SelectedLevel += 1;
        UpdateUI();
    }

    public void OnStartBtnClick()
    {
        Menu.SetActive(false);
        GameManager.Instance.GoldDungeonStart(SelectedLevel);
    }

    public void OnSweepBtnClick()
    {
        SweepPanel.Level = SelectedLevel;
        SweepPanel.Show();        
    }

    private void UpdateUI()
    {
        TicketCountTxt.text = DungeonManager.SaveData.TicketNum.ToString() + "/2";
        SelectedLevelTxt.text = SelectedLevel.ToString();
        // 보상 금액을 BigInteger로 변환 후 축약된 형식으로 표시
        BigInteger rewardAmount = new BigInteger(DungeonManager.DataList[SelectedLevel - 1].Rewards.ToList().Find(x => x.type == ECurrencyType.Gold).amount);
        RewardTxt.text = rewardAmount.ToAbbreviatedString();

        if (DungeonManager.SaveData.TicketNum == 0)
        {
            ChallengeBtn.gameObject.SetActive(false);
        }
        else
        {
            ChallengeBtn.gameObject.SetActive(true);
        }
        if(SelectedLevel > DungeonManager.SaveData.LevelNum)
        {
            SweepBtn.gameObject.SetActive(false);
        }
        else
        {
            SweepBtn.gameObject.SetActive(true);
        }

        if(SelectedLevel == 1)
        {
            LeftBtn.gameObject.SetActive(false);
        }
        else
        {
            LeftBtn.gameObject.SetActive(true);
        }
        if(SelectedLevel == DungeonManager.MAX_DUNGEON_COUNT || SelectedLevel == DungeonManager.SaveData.LevelNum + 1)
        {
            RightBtn.gameObject.SetActive(false);
        }
        else
        {
            RightBtn.gameObject.SetActive(true);
        }
    }
}
