using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static UnityEditor.VersionControl.Asset;
using static UnityEngine.UI.Image;

[Serializable]
public class HeroSaveData
{
    public List<HeroInfomation> heroInfo = new List<HeroInfomation>();

    public List<int> EntryHidList = new(); 
}

[Serializable]
public class HeroInfomation
{    
    public int hid;
    public ERoleType roleType;
    public string icon;
    public string heroName;
    public string heroDescription;
    public string RCode; 
    public CharacterStat multipleStat; // 정보가 변하지 않는 상수 데이터
    public CharacterStat PassiveStat;
    public CharacterStat gradeStatModifier;
    public CharacterStat starsStatModifier;

    public ERarityType rarityType;
    public int GradeLevel;
    public int StarsLevel;
}

public class HeroManager : Singleton<HeroManager>
{
    public List<int> hidList = new List<int>();
    public Dictionary<int,Hero> heroDict = new Dictionary<int, Hero>();

    public Image[] heroSlot;

    public List<Hero> heroEntry = new List<Hero>();

    public StatHandler statHandler;

    public List<Sprite> heroSilhouetteArray;

    public HeroSaveData HeroSaveData = new();

    public UIPartyEntry PartyEntry;

    private void Start()
    {
        statHandler = new StatHandler();

        statHandler.ChangeCharacterStat();

        StatManager.Instance.statHandler.AddStatModifier(statHandler.curStat);

        /*StartCoroutine(InitialHero());*/

        HeroLoadData();

        Invoke("HeroEntryAdd", 1f  ) ;
    }

    public async Task HasHeroCheckAsync(HeroData heroData)
    {       
        string rCdoe = heroData.RCode;
        GameObject obj = await ResourceManager.Instance.GetResource<GameObject>(rCdoe, EAddressableType.PREFAB) ;

        if (heroDict.ContainsKey(heroData.hid))
        {
            //Debug.Log("해당 키 있음 . 각성 조각 추가 로직");
            CurrencyManager.instance.HeroFragmentDict[heroData.hid].Add(1);
        }
        else
        {
            //Debug.Log("해당 키 없음 . 신규 영웅 추가 로직");

            GameObject go = Instantiate(obj);
            go.name = heroData.RCode;           
            go.transform.parent = this.transform;
            go.SetActive(false);

            Hero newHero = go.GetComponent<Hero>();

            newHero.data = heroData;            
            heroDict.Add(heroData.hid, newHero);

            Currency currency = new Currency();
            CurrencyManager.instance.HeroFragmentDict.Add(heroData.hid, currency);

            StatHandler sh = go.GetComponent<StatHandler>();

            //Debug.Log(heroData);

            sh.AddStatModifier(StatManager.Instance.statHandler.curStat);            
            sh.AddStatModifier(heroData.multipleStat);
            sh.AddStatModifier(heroData.gradeStatModifier);
            sh.AddStatModifier(heroData.starsStatModifier);
            sh.UpdateStatModifier();

            statHandler.AddStatModifier(heroData.PassiveStat);
        }

        DataUpdate();
        GameManager.Instance.HeroUpdate();

        //SaveHeroData();
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
                heroSlot[i].color = Color.white;
            }
            else
            {
                heroSlot[i].sprite = heroSilhouetteArray[i];
                heroSlot[i].color = Color.black;
            }
        }
    }

    public void DataUpdate()
    {
        SaveHeroData();
        UpdateUI();
    }

    public void SaveHeroData()
    {      
        HeroSaveData.heroInfo = new List<HeroInfomation>();

        for (int i = 0; i < hidList.Count; i++)
        {
            if (heroDict.ContainsKey(hidList[i]))
            {
                Hero SaveHeroInfo = heroDict[hidList[i]];
                
                HeroInfomation heroInfomation = new HeroInfomation();
                
                heroInfomation.hid = SaveHeroInfo.data.hid;
                                

                heroInfomation.roleType = SaveHeroInfo.data.roleType;
                
                heroInfomation.icon = DataManager.Instance.ImageToString(SaveHeroInfo.data.icon);

                heroInfomation.heroName = SaveHeroInfo.data.heroName;
                heroInfomation.heroDescription = SaveHeroInfo.data.heroDescription;
                heroInfomation.RCode = SaveHeroInfo.data.RCode;
                heroInfomation.multipleStat = SaveHeroInfo.data.multipleStat;
                heroInfomation.PassiveStat = SaveHeroInfo.data.PassiveStat;
                heroInfomation.gradeStatModifier = SaveHeroInfo.data.gradeStatModifier;
                heroInfomation.starsStatModifier = SaveHeroInfo.data.starsStatModifier;


                heroInfomation.rarityType = SaveHeroInfo.rarityType;
                heroInfomation.GradeLevel = SaveHeroInfo.GradeLevel;
                heroInfomation.StarsLevel = SaveHeroInfo.StarsLevel;
                
                HeroSaveData.heroInfo.Add(heroInfomation);
            }
        }
        HeroSaveData.EntryHidList.Clear();

        for ( int i = 0; i < heroEntry.Count; i++)
        {
            HeroSaveData.EntryHidList.Add(heroEntry[i].data.hid);
        }
    }

    private void HeroLoadData()
    {
        //Debug.Log($"<color=red> 파티 로드 데이터 </color>");
        HeroSaveData loadData = DataManager.Instance.LoadData<HeroSaveData>(ESaveType.HERO);

        if (loadData == null)
        {
            //Debug.Log($"<color=red> 파티 이니셜라이즈 </color>");
        }
        else
        {
            //Debug.Log($"<color=red> 파티 로드 </color>");

            //HeroSaveData = loadData;
            //LoadDataSync();

            StartCoroutine(InitialHero());
            heroEntry.Clear();

            /*for (int i = 0; i < HeroSaveData.EntryHidList.Count; i++)
            {
                int key = HeroSaveData.EntryHidList[i];


                heroEntry.Add(heroDict[key]);
            }*/

            //SaveHeroData();
        }
    }

   /* public async void LoadDataSync()
    {
        HeroSaveData loadData = DataManager.Instance.LoadData<HeroSaveData>(ESaveType.HERO);

        Debug.Log(loadData);
        HeroSaveData = loadData;

        Debug.Log(HeroSaveData.heroInfo.Count);

        for ( int i = 0; i <  HeroSaveData.heroInfo.Count;  i++ )
        {
            Debug.Log(HeroSaveData.heroInfo.Count);

            string rCdoe = HeroSaveData.heroInfo[i].RCode;
            Debug.Log(HeroSaveData.heroInfo.Count);
            GameObject obj = await ResourceManager.Instance.GetResource<GameObject>(rCdoe, EAddressableType.PREFAB);
            Debug.Log(HeroSaveData.heroInfo.Count);
            GameObject go = Instantiate(obj);
            Debug.Log(HeroSaveData.heroInfo.Count);
            go.transform.parent = this.transform;
            Debug.Log(HeroSaveData.heroInfo.Count);
            go.SetActive(false);
            Debug.Log(HeroSaveData.heroInfo.Count);
            Hero newHero = go.GetComponent<Hero>();
            Debug.Log(HeroSaveData.heroInfo.Count);
            newHero.data.hid = HeroSaveData.heroInfo[i].hid;
            newHero.data.roleType = HeroSaveData.heroInfo[i].roleType;
            Debug.Log("456");
            newHero.data.icon = DataManager.Instance.StringToImage(HeroSaveData.heroInfo[i].icon);
            newHero.data.heroName = HeroSaveData.heroInfo[i].heroName;
            Debug.Log("789");
            newHero.data.heroDescription = HeroSaveData.heroInfo[i].heroDescription;
            Debug.Log("111");
            newHero.data.RCode = HeroSaveData.heroInfo[i].RCode;
            
            newHero.data.PassiveStat = HeroSaveData.heroInfo[i].PassiveStat;
            newHero.data.gradeStatModifier = HeroSaveData.heroInfo[i].gradeStatModifier;
            newHero.data.starsStatModifier = HeroSaveData.heroInfo[i].starsStatModifier;

            newHero.rarityType = HeroSaveData.heroInfo[i].rarityType;
            newHero.GradeLevel = HeroSaveData.heroInfo[i].GradeLevel;
            newHero.StarsLevel = HeroSaveData.heroInfo[i].StarsLevel;

            heroDict.Add(HeroSaveData.heroInfo[i].hid, newHero);

            statHandler.AddStatModifier(HeroSaveData.heroInfo[i].PassiveStat);


            Debug.Log("하나 처리");

            Debug.Log(heroDict.Count);
        }

        

        //DataUpdate();
    }*/

    public IEnumerator InitialHero()
    {
        HeroSaveData loadData = DataManager.Instance.LoadData<HeroSaveData>(ESaveType.HERO);

        //HeroSaveData = loadData;

        for (int i = 0; i < loadData.heroInfo.Count; i++)
        {      
            string rCdoe = loadData.heroInfo[i].RCode;

            _ = CreateHero(loadData.heroInfo[i]);
        }
        yield return null;

        
    }

    public async Task CreateHero(HeroInfomation data)
    {
        string rCdoe = data.RCode;

        GameObject obj = await ResourceManager.Instance.GetResource<GameObject>(rCdoe, EAddressableType.PREFAB);

        GameObject go = Instantiate(obj);
        go.transform.parent = this.transform;
        go.SetActive(false);

        Hero newHero = go.GetComponent<Hero>();

        newHero.data.hid = data.hid;
        newHero.data.roleType = data.roleType;
        
        newHero.data.icon = DataManager.Instance.StringToImage(data.icon);
        newHero.data.heroName = data.heroName;
        
        newHero.data.heroDescription = data.heroDescription;
        
        newHero.data.RCode = data.RCode;

        newHero.data.multipleStat = data.multipleStat;
        newHero.data.PassiveStat = data.PassiveStat;
        newHero.data.gradeStatModifier = data.gradeStatModifier;
        newHero.data.starsStatModifier = data.starsStatModifier;


        newHero.rarityType = data.rarityType;
        newHero.GradeLevel = data.GradeLevel;
        newHero.StarsLevel = data.StarsLevel;

        //Debug.Log(data.GradeLevel);
        
        heroDict.Add(data.hid, newHero);
        
        
        
        StatHandler sh = go.GetComponent<StatHandler>();


        StatAdd(sh, data);

        /*sh.AddStatModifier(StatManager.Instance.statHandler.curStat);
        sh.AddStatModifier(newHero.data.multipleStat);
        sh.AddStatModifier(newHero.data.gradeStatModifier);
        sh.AddStatModifier(newHero.data.starsStatModifier);*/
        sh.UpdateStatModifier();

        newHero.data.PassiveStat = data.PassiveStat;
        statHandler.AddStatModifier(data.PassiveStat);
        
        
        //DataUpdate();

        DictAdd(data.hid, newHero);
    }

    public void StatAdd(StatHandler sh, HeroInfomation hd)
    {
        //HeroSaveData loadData = DataManager.Instance.LoadData<HeroSaveData>(ESaveType.HERO);
        //HeroSaveData = loadData;

        sh.AddStatModifier(StatManager.Instance.statHandler.curStat);

        sh.AddStatModifier(hd.multipleStat);

        sh.AddStatModifier(hd.gradeStatModifier);

        sh.AddStatModifier(hd.starsStatModifier);

        sh.grade = hd.GradeLevel;

        sh.stars = hd.StarsLevel;
    }

    public void DictAdd(int key, Hero hero)
    {       
        
        heroDict.Add(key, hero);
    }
   
    private void HeroEntryAdd()
    {
        HeroSaveData loadData = DataManager.Instance.LoadData<HeroSaveData>(ESaveType.HERO);

        if (loadData != null)
        {
            //HeroSaveData = loadData;

            heroEntry.Clear();

            //Debug.Log(loadData.EntryHidList.Count);

            for ( int i = 0; i < loadData.EntryHidList.Count; i++)
            {
                int key = loadData.EntryHidList[i];

                heroEntry.Add(heroDict[key]);

                GameManager.instance.EntryList.Add(heroDict[key]);
            }
            PartyEntry.UpdateUI();

            DataUpdate();

        }
        

        
    }
}

