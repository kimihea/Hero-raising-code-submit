using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStageBar : MonoBehaviour
{
    [SerializeField] private Image waveProgressBar;
    [SerializeField] private Button bossTryBtn;
    [SerializeField] private TextMeshProUGUI chapterTxt;
    [SerializeField] private TextMeshProUGUI stageTxt;

    private StageManager manager;

    private void Start()
    {
        manager = GameManager.Instance.Stage;
        UpdateUI();
    }

    private void OnEnable()
    {
        if (!GameManager.isInit) return;
        UpdateUI();
    }

    private void Update()
    {        
        UpdateUI();
    }

    private void UpdateUI()
    {
        waveProgressBar.fillAmount = manager.StageProgress;
        chapterTxt.text = manager.ChapterNum.ToString() + " - ";
        stageTxt.text = manager.StageNum.ToString();
        if (manager.WaveNum == manager.BOSS_WAVE_IDX) bossTryBtn.interactable = false;
        else bossTryBtn.interactable = true;
    }

    public void OnBossTryBtnClick()
    {
        manager.StartBossWave();
        QuestManager.Instance.AddProgress(EQuestType.BOSSBUTTON, 1);
    }
}
