using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGoldDungeonBar : MonoBehaviour
{
    private GoldDungeonManager manager;
    public Image ClockFill;
    public Image ProgressFill;
    public TextMeshProUGUI TimeTxt;
    public TextMeshProUGUI ProgressTxt;

    private bool isFade;


    private void Start()
    {
        manager = GameManager.Instance.GoldDungeon;
        UpdateUI();
    }


    private void OnEnable()
    {
        if (!GameManager.isInit) return;
        isFade = false;
        UpdateUI();
    }    

    private void Update()
    {
        UpdateUI();
        if (!isFade && manager.RemainTime <= 15f && manager.TotalClearPoint > 0)
        {
            //TimeTxt.DOFade(0.0f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            TimeTxt.DOColor(Color.red, 0.5f).SetLoops(-1, LoopType.Yoyo);
            isFade = true;
        }
    }

    private void UpdateUI()
    {
        TimeTxt.text = Mathf.Max(manager.RemainTime, 0).ToString("F0");
        ClockFill.fillAmount = manager.RemainTime / manager.TIME_LIMIT;
        float progress = 0f;
        if (manager.TotalClearPoint != 0) progress = (float)manager.CurClearPoint / manager.TotalClearPoint;        
        ProgressFill.fillAmount = progress;
        ProgressTxt.text = Mathf.Min((progress * 100), 100).ToString("F0") + "%";
    }
}
