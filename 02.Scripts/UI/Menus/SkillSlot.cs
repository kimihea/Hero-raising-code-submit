using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot :MonoBehaviour
{
    public bool IsLock;
    public SkillMenu skillMenu;
    public Image icon;
    public Button btn;
    private Button lockBtn;
    public Text countText;
    private string chapterInfo;
    private int maxCount =20;
    private int Count;


    public GameObject IsEquip;
    public GameObject Lock;
    

    [Header("객체별로 다르게설정")]
    public int index;
    private (int,int) OpenLevel;
    public int OpenChap;
    public int OpenStage;
    public void Awake()
    {
        countText = GetComponentInChildren<Text>();
        lockBtn = Lock.GetComponent<Button>();
    }
    private void Start()
    {
        OpenLevel = (OpenChap, OpenStage);
        lockBtn.onClick.RemoveListener(LearnStageInfo);
        lockBtn.onClick.AddListener(LearnStageInfo);
        btn.onClick.RemoveListener(OnClickButton);
        btn.onClick.AddListener(OnClickButton);
        if (IsLock) 
        { 
            btn.enabled = false;
            Lock.SetActive(true);
        }
        icon.sprite = SkillManager.Instance.HeroIdToList(0, skill => skill.Data.Icon,icon.sprite)[index];
    }
    public void LearnSkill()
    {
        IsLock = false;
        Lock.SetActive(false);
        btn.enabled = true;
    }
    public void LearnStageInfo()
    {
        var currentStage = (
    ChapterNum: GameManager.Instance.Stage.ChapterNum,
    StageNum: GameManager.Instance.Stage.StageNum);
        if (OpenLevel.CompareTo(currentStage) > 0)
        {
            chapterInfo = string.Format("{0}-{1}에 해제",OpenLevel.Item1,OpenLevel.Item2); //
            GameManager.Instance.ShowAlert(chapterInfo, EAlertType.LACK);
        }

        else
        {
            /*각성던전으로 이동하고 각성던전에서 해금되는 로직*/
            //chapterInfo = "배우기 가능.";
            //skillMenu.MoveToAwakeDungeon(); /
            /*스테이지에 도달하면 바로 해금되는 로직*/
            chapterInfo = "스킬을 배웠습니다"; 
            LearnSkill();
            GameManager.Instance.ShowAlert(chapterInfo, EAlertType.SUCCESS);
        }
    }
    public void UpdateCount()
    {
        Count = SkillManager.Instance.HeroIdToList(0, skill => skill.Count, 0)[index];
        countText.text = $"{Count}/{maxCount}";
    }
    public void OnClickButton()
    {
        skillMenu.SelectSkill(index);
    }
    //bool IsLock을 저장
}
