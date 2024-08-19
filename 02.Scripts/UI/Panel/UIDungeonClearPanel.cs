using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonClearPanel : MonoBehaviour
{
    public GoldDungeonManager GoldDungeon;
    public Button NextBtn;
    public Button ExitBtn;
    public TextMeshProUGUI RewardTxt;

    private void Start()
    {
        if(GoldDungeon.SaveData.TicketNum == 0 || GoldDungeon.DungeonNum == GoldDungeon.MAX_DUNGEON_COUNT)
        {
            NextBtn.gameObject.SetActive(false);
        }
    }

    public void OnNextBtnClick()
    {
        GoldDungeon.StopSpawn();
        UILoading.Instance.StartFade(GoldDungeon.InitNextDungeon, GoldDungeon.StartDungeon);
        Destroy(gameObject);
    }

    public void OnExitBtnClick()
    {
        GameManager.Instance.VictoryBattle();
        //UIManager.Instance.CloseUI();
        Destroy(gameObject);
    }

    //public void Show()
    //{
    //    UIManager.Instance.PushPopUp(this);
    //}

    //public void Hide()
    //{
    //    Destroy(gameObject);
    //}
}
