using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UILoading : Singleton<UILoading>
{
    [SerializeField] private TextMeshProUGUI DescTxt;
    [SerializeField] private TextMeshProUGUI FadeTxt;
    [SerializeField] private Image Background;
    [SerializeField] private Image FadeIn;
    [SerializeField] private Image FadeOut;
    [SerializeField] private GameObject UI;
    public bool IsFadeDone = true;

    protected override void Awake()
    {
        base.Awake();
        UI.SetActive(false);
    }

    public static void Show(Sprite bg = null)
    {
        Instance.SetBG(bg);
        Instance.UI.SetActive(true);        
    }

    public void SetBG(Sprite bg = null)
    {
        if (bg != null)
            this.Background.sprite = bg;
    }

    public static void Hide()
    {
        Instance.UI.SetActive(false);
    }

    public void SetProgress(float progress, string desc = "")
    {
        this.DescTxt.text = desc;
        //slider.value = progress;
    }

    public void SetProgress(AsyncOperation op, string desc = "")
    {
        this.DescTxt.text = desc;
        StartCoroutine(Progress(op));
    }

    public IEnumerator Progress(AsyncOperation op)
    {
        while (op.isDone)
        {
            //slider.value = op.progress;
            yield return new WaitForEndOfFrame();
        }
        //slider.value = 1;
    }

    public void SetProgress(AsyncOperationHandle op, string desc = "")
    {
        this.DescTxt.text = desc;
        StartCoroutine(Progress(op));
    }

    public IEnumerator Progress(AsyncOperationHandle op)
    {
        while (op.IsDone)
        {
            //slider.value = op.GetDownloadStatus().Percent;
            yield return new WaitForEndOfFrame();
        }
        //slider.value = 1;
    }

    public void StartFade(Func<IEnumerator> callback, Action action, params object[] args)
    {
        IsFadeDone = false;
        StartCoroutine(StartFadeCoroutine(callback, action, args));
    }

    public void StartFade(Func<object[], IEnumerator> callback, Action action, params object[] args)
    {
        IsFadeDone = false;     
        StartCoroutine(StartFadeCoroutine(callback, action, args));
    }

    private IEnumerator StartFadeCoroutine(Func<IEnumerator> callback, Action action, params object[] args)
    {
        // Fade Out
        FadeOut.gameObject.SetActive(true);
        FadeOut.rectTransform.localScale = new Vector3(0f, 1f, 1f);
        FadeOut.rectTransform.DOScaleX(1f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        // Wait For Progress
        FadeTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        //StartCoroutine(callback());
        yield return callback.Invoke();
        // Fade In
        FadeOut.gameObject.SetActive(false);
        FadeTxt.gameObject.SetActive(false);
        FadeIn.gameObject.SetActive(true);
        FadeIn.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        FadeIn.rectTransform.DOScaleX(0f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        IsFadeDone = true;
        action.Invoke();
    }

    private IEnumerator StartFadeCoroutine(Func<object[], IEnumerator> callback, Action action, params object[] args)
    {
        // Fade Out
        FadeOut.gameObject.SetActive(true);
        FadeOut.rectTransform.localScale = new Vector3(0f, 1f, 1f);
        FadeOut.rectTransform.DOScaleX(1f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        // Wait For Progress
        FadeTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f); 
        //StartCoroutine(callback());
        yield return callback.Invoke(args); 
        // Fade In
        FadeOut.gameObject.SetActive(false);
        FadeTxt.gameObject.SetActive(false);
        FadeIn.gameObject.SetActive(true);
        FadeIn.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        FadeIn.rectTransform.DOScaleX(0f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        IsFadeDone = true;
        action.Invoke();
    }
}
