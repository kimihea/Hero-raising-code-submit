using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [SerializeField] private Button StartBtn;
    [SerializeField] private Image Background;

    private bool isInit = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        UILoading.Instance.SetBG(Background.sprite);
        yield return new WaitForSeconds(2);
        OnInit();
        yield return new WaitUntil(() => isInit);
        SceneManager.LoadSceneAsync("MainScene");
        //GameManager.Instance.StartGame();
    }

    public void OnInit()
    {
        StartCoroutine(OnManagerInit());
    }

    public IEnumerator OnManagerInit()
    {
        UILoading.Show();
        ResourceManager.Instance.Init();
        yield return new WaitUntil(() => ResourceManager.Instance.isInit);
        isInit = true;
    }
}