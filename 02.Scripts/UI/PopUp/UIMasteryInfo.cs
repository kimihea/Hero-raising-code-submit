using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMasteryInfo : MonoBehaviour, IUIPopUp
{
    private MasteryInfo info;
    private TimeSpan timeSpan;

    public Image Icon;
    public TextMeshProUGUI DescTxt;
    public TextMeshProUGUI TimeTxt;
    public GameObject Lock;
    public Button ExitBtn;
    public Button ResearchBtn;
    public Text ResearchBtnTxt;

    public void Hide()
    {
        Destroy(gameObject);
    }

    public void Show()
    {
        UIManager.Instance.PushPopUp(this);
    }

    public void InitUI(MasteryInfo info, Sprite sprite)
    {
        this.info = info;
        Icon.sprite = sprite;
        DescTxt.text = info.Description;
        TimeTxt.text = IdleTime.GetStringFromSeconds(info.TotalResearchTime.GetSeconds());       
        switch(info.Condition)
        {
            case EMasteryCondition.LOCK:
                Lock.SetActive(true);
                ResearchBtn.interactable = false;
                break;
            case EMasteryCondition.CANRESEARCHING:
                break;
            case EMasteryCondition.ISRESEARCHING:
                ResearchBtn.interactable = false;
                ResearchBtnTxt.text = "연구중";
                break;
            case EMasteryCondition.DONE:
                ResearchBtn.gameObject.SetActive(false);
                break;
        }
    }

    public void OnResearcingButtonClick()
    {        
        info.Condition = EMasteryCondition.ISRESEARCHING;
        DataManager.Instance.SaveData();
        UIManager.Instance.CloseUI();
        QuestManager.Instance.AddProgress(EQuestType.MASTERY, 1);
    }

    public void OnEXitButtonClick()
    {
        UIManager.Instance.CloseUI();
    }
}
