using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestPanel : MonoBehaviour
{
    QuestManager manager;
    public Button Btn;
    public TextMeshProUGUI IndexTxt;
    public TextMeshProUGUI DescTxt;
    public TextMeshProUGUI ProgressTxt;
    public TextMeshProUGUI RewardTxt;
    public TextMeshProUGUI RewardConst;
    public Image RewardIcon;
    public Image RedDot;
    public ShinyEffectForUGUI ShinyEffect;

    public List<Sprite> Icons;

    private Tweener tweener;

    private IEnumerator Start()
    {
        manager = QuestManager.Instance;        
        manager.OnChangeQuestTargetEvnet += UpdateUI;
        yield return new WaitUntil(() => manager.IsInit);
        UpdateUI();
    }
   
    public void UpdateUI()
    {
        IndexTxt.text = "퀘스트 " + (manager.SaveData.Index + 1).ToString();
        if (manager.SaveData.Index >= manager.QuestList.Count)
        {
            Btn.interactable = false;
            DescTxt.text = "Cooming Soon";
            ProgressTxt.text = "";
            RewardTxt.text = "";
            RewardIcon.gameObject.SetActive(false);
            RewardConst.gameObject.SetActive(false);
        }
        else
        {
            DescTxt.text = manager.GetQuestDesc();
            ProgressTxt.text = string.Format("( {0} )", manager.GetQuestProgress());
            RewardTxt.text = manager.CurQuest.Amount.ToString();
            RewardIcon.sprite = Icons[(int)manager.CurQuest.RewardType];
            bool isComplete = manager.CheckCompleteQuest();
            RedDot.gameObject.SetActive(isComplete);
            if (isComplete)
            {
                if (!tweener.IsActive())
                {
                    tweener = DOTween.To(() => 0f, x => ShinyEffect.location = x, 1f, 2f).SetLoops(-1, LoopType.Restart);
                }
            }
            else
            {
                tweener.Kill();
                tweener = null;
                ShinyEffect.location = 0f;
            }
            ProgressTxt.color = isComplete ? Color.yellow : Color.gray;
        }
    }

    public void OnBtnClick()
    {
        if(manager.CheckCompleteQuest())
        {
            manager.CompleteQuest();
            UpdateUI();           
        }
    }

}
