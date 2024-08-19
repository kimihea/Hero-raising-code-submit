using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISweepPanel : MonoBehaviour, IUIPopUp
{
    private GoldDungeonManager Manager;

    public int Level;    
    public TextMeshProUGUI KeyCountTxt;
    public TextMeshProUGUI RewardTxt;
    public Text MultiSweepCountTxt;
    public Button SweepBtn;
    public Button MultiSweepBtn;        

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        UIManager.Instance.PushPopUp(this);
        gameObject.SetActive(true);
        UpdateUI();
    }    

    private void UpdateUI()
    {
        Manager = GameManager.Instance.GoldDungeon;
        RewardTxt.text = Manager.DataList[Level - 1].Rewards[0].amount.ToString();
        KeyCountTxt.text = Manager.SaveData.TicketNum.ToString() + " / 2";
        int ticketCount = Mathf.Min(Manager.SaveData.TicketNum, 10);        
        if(ticketCount <= 1)
        {
            MultiSweepBtn.gameObject.SetActive(false);
        }
        else
        {
            MultiSweepCountTxt.text = ticketCount.ToString() + "회 소탕";
            MultiSweepBtn.gameObject.SetActive(true);
        }
    }

    public void OnSweepBtnClick()
    {
        Manager.SweepDungeon(1, Level);
        UpdateUI();
    }

    public void OnMultiSweepBtnClick()
    {
        int ticketCount = Mathf.Min(Manager.SaveData.TicketNum, 10);
        Manager.SweepDungeon(ticketCount, Level);
        UpdateUI();
    }
}
