using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroManager : Singleton<HeroManager>
{
    public List<int> hidList = new List<int>();
    public Dictionary<int,Hero> heroDict = new Dictionary<int, Hero>();

    public Image[] heroSlot;

    public List<Hero> heroEntry = new List<Hero>();
    public void HasHeroCheck(HeroSO heroSO)
    {
        Debug.Log(heroSO.name);

        if (heroDict.ContainsKey(heroSO.hid))
        {
            Debug.Log("해당 키 있음 . 각성 조각 추가 로직");
        }
        else
        {
            Debug.Log("해당 키 없음 . 신규 영웅 추가 로직");

            /* GameObject go = new GameObject("test");
             go.transform.parent = transform;
             go.AddComponent<Hero>();*/

            string rCdoe = heroSO.RCode;

            GameObject obj = ResourceManager.Instance.GetResource<GameObject>(rCdoe, EResourceType.PREFAB);
            Hero newHero = obj.GetComponent<Hero>();

            newHero.data = heroSO;
            heroDict.Add(heroSO.hid, newHero);
        }

        UpdateUI();
    }

    public void InEntry(int index)
    {
        int hid = hidList[index];

        // 게임 매니저 엔트리에 추가
        GameManager.Instance.EntryList.Add(heroDict[hid]);

        // 스테이지에 소환

        // 해당 씬 재시작
    }

    public void OutEntry(int index)
    {
        int hid = hidList[index];

        // 게임 매니저 엔트리에 제거
        GameManager.Instance.EntryList.Remove(heroDict[hid]);

        // 스테이지에 제거

        // 해당 씬 재시작
    }        

    public void UpdateUI()
    {
        for (int i = 0; i < hidList.Count; i++)
        {
            if (heroDict.ContainsKey(hidList[i]))
            {
                heroSlot[i].sprite = heroDict[(hidList[i])].data.icon;
            }
        }
    }

}