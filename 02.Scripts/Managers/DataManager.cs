using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ESaveType
{
    STAGE,
    CURRUNCY,
    SKILL,
    MASTERY,
    STAT,
    EQUIPMENT,
    HERO,
    MIMIC,
    GOLDDUNGEON,
    QUEST
}



[Serializable]
public class UserData
{
    public IdleTime LastUpdateTime;

    public StageSaveData StageSaveData;
    public List<MasteryInfo> MasteryInfos;
    public List<Stat> Stats;
    public EquipmentSaveData currentEquipment;
    public HeroSaveData HeroSaveData;
    public CurrencySaveData CurrencySaveData;
    public GoldDungeonSaveData GoldDungeonSaveData;

    public MimicSaveData MimicSaveData;

    public SkillSaveData SkillSaveData;

    public QuestSaveData QuestSaveData;

    public void RefreshData()
    {
        if(LastUpdateTime == null)
            LastUpdateTime = new IdleTime();
        LastUpdateTime.ConvertFromDateTime(DateTime.Now);

        StageSaveData = StageSaveData.SaveData;
        MasteryInfos = GameManager.Instance.Mastery.MasteryList;        

        Stats = StatManager.Instance.Stats;

        currentEquipment = StatManager.Instance.equipment.EquipmentSaveData;

        HeroManager.Instance.DataUpdate();
        HeroSaveData = HeroManager.Instance.HeroSaveData;

        CurrencyManager.Instance.SaveData();
        CurrencySaveData = CurrencyManager.Instance.CurrencySaveData;

        MimicManager.Instance.SaveData();
        MimicSaveData = MimicManager.Instance.MimicSaveData;

        SkillManager.Instance.SaveData();
        SkillSaveData = SkillManager.Instance.SkillSaveData;

        GoldDungeonSaveData = GoldDungeonSaveData.SaveData;

        QuestSaveData = QuestSaveData.SaveData;
    }
}

[Serializable]
public class IdleTime
{
    public int year;
    public int month;
    public int day;
    public int hour;
    public int minute;
    public int second;
    public int millisecond;

    public static string GetStringFromSeconds(float seconds)
    {
        string str = "";
        TimeSpan span = TimeSpan.FromSeconds(seconds);
        str = string.Format("{0}{1}{2}{3}",
            span.Days > 0 ? string.Format("{0}일 ", span.Days) : string.Empty,
            span.Hours > 0 ? string.Format("{0}시간 ", span.Hours) : string.Empty,
            span.Minutes > 0 ? string.Format("{0}분 ", span.Minutes) : string.Empty,
            span.Seconds > 0 ? string.Format("{0}초", span.Seconds) : string.Empty);

        return str;
    }

    public string GetString()
    {
        string str = "";
        TimeSpan span = new TimeSpan(day, hour, minute, second);
        str = string.Format("{0}{1}{2}{3}",
            span.Days > 0 ? string.Format("{0}일 ", span.Days) : string.Empty,
            span.Hours > 0 ? string.Format("{0}시간 ", span.Hours) : string.Empty,
            span.Minutes > 0 ? string.Format("{0}분 ", span.Minutes) : string.Empty,
            span.Seconds > 0 ? string.Format("{0}초", span.Seconds) : string.Empty);

        return str;
    }    

    public void ConvertFromDateTime(DateTime dt)
    {
        year = dt.Year;
        month = dt.Month;
        day = dt.Day;
        hour = dt.Hour;
        minute = dt.Minute;
        second = dt.Second;
        millisecond = dt.Millisecond;
    }

    public float GetSeconds()
    {
        TimeSpan span = new TimeSpan(day, hour, minute, second);
        return (float)span.TotalSeconds;
    }

    public DateTime GetDateTime()
    {
        DateTime time = new DateTime(year, month, day, hour, minute, second, millisecond);
        return time;
    }
}

public class DataManager : Singleton<DataManager>
{
    private float curTime = 0;
    public UserData UserSaveData = new UserData();
    public UserData UserLoadData;
    public List<QuestData> QuestList;


    // PlayerPrefs의 키 목록, keys index = ESaveType 매칭되어야 함
    private string[] keys =
    {
        "Stage",
        "Currency",
        "Skill",
        "Mastery",
        "Stat",
        "Equipment",
        "NewItem"
    };

    protected override void Awake()
    {
        base.Awake();
        LoadData();
        //PlayerPrefs.DeleteAll(); // 데이터 리셋 필요시 주석 제거
    }

    private void OnApplicationPause(bool pause)
    {        
        foreach(var type in Enum.GetValues(typeof(ESaveType)))
        {
            SaveData();
            //SaveData((ESaveType)type);
        }
        //PlayerPrefs.Save(); // 어플리케이션이 일시정지 될 때, 모든 PlayerPrefs 정보 저장
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if(curTime > 10f && SceneManager.sceneCount != 0)
        {
            SaveData();
            //foreach (var type in Enum.GetValues(typeof (ESaveType)))
            //{   
            //    SaveData((ESaveType)type);
            //}
            //PlayerPrefs.Save();
            curTime = 0;
        }
    }

    public void SaveData()
    {
        UserSaveData.RefreshData();

        string data = JsonUtility.ToJson(UserSaveData);
        string savePath = Path.Combine(Application.persistentDataPath, "UserData.json");        
        File.WriteAllText(savePath, data);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        //Debug.Log(data);
        //PlayerPrefs.SetString("User", data);        
    }

    //public void SaveData(ESaveType type)
    //{
    //    string data = null;
    //    switch (type)
    //    {
    //        case ESaveType.STAGE:
    //            data = JsonUtility.ToJson(StageSaveData.SaveData); 
    //             // SetString을 통해 Json string을 PlayerPref에 저장
    //            break;
    //        case ESaveType.CURRUNCY:
    //            break;
    //        case ESaveType.SKILL:
    //            data = JsonUtility.ToJson(SkillManager.Instance.SkillList);
    //            break;
    //        /*case ESaveType.MASTERY:
    //            var list = GameManager.Instance.Mastery.MasteryList;
    //            data = JsonUtility.ToJson(list);*/
    //            //break;
    //        /*case ESaveType.STAT:                
    //            data = JsonUtility.ToJson(StatManager.Instance.Stats);*/
                
    //            //break;
    //    }
    //    PlayerPrefs.SetString(keys[(int)type], data);
    //}

    private void LoadData()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "UserData.json");
        if (!File.Exists(savePath))
        {
            UserLoadData = null;
            return ;
        }
        var data = File.ReadAllText(savePath);
        UserLoadData = JsonUtility.FromJson<UserData>(data);
    }   

    public T LoadData<T>(ESaveType type) where T : class
    {
        if (UserLoadData == null) return null;
        //if (!PlayerPrefs.HasKey("User") || PlayerPrefs.GetString("User") == "")
        //{
        //    return null;
        //}
        //if (!PlayerPrefs.HasKey(keys[(int)type]))
        //{
        //    return null;
        //}
        //var data = PlayerPrefs.GetString(keys[(int)type]);
        //var data = PlayerPrefs.GetString("User");
        //Data = JsonUtility.FromJson<UserData>(data);
        switch (type)
        {
            case ESaveType.STAGE:
                return UserLoadData.StageSaveData as T;
            case ESaveType.CURRUNCY:
                return UserLoadData.CurrencySaveData as T;
            case ESaveType.SKILL:
                return UserLoadData.SkillSaveData as T; ;
            case ESaveType.MASTERY:
                return UserLoadData.MasteryInfos as T;
            case ESaveType.STAT:
                return UserLoadData.Stats as T;
            case ESaveType.EQUIPMENT:
                return UserLoadData.currentEquipment as T;
            case ESaveType.HERO:
                return UserLoadData.HeroSaveData as T;
            case ESaveType.MIMIC:
                return UserLoadData.MimicSaveData as T;
            case ESaveType.GOLDDUNGEON:
                return UserLoadData.GoldDungeonSaveData as T;
            case ESaveType.QUEST:
                return UserLoadData.QuestSaveData as T;
            default:
                return null;
        }
    }

    public string ImageToString(Sprite sprite)
    {
        byte[] imageBytes = sprite.texture.EncodeToPNG();

        return Convert.ToBase64String(imageBytes);
    }

    public Sprite StringToImage(string str)
    {
        byte[] imageBytes = Convert.FromBase64String(str);

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

        return sprite;

    }

    #region FileIO by Json
    //    private string[] path =
    //    {
    //        "StageData.json"
    //    };

    //    public void SaveData(ESaveType type)
    //    {
    //        string savePath;
    //        switch(type)
    //        {
    //            case ESaveType.STAGE:
    //                savePath = Path.Combine(Application.dataPath, path[(int)type]);
    //                var data = JsonUtility.ToJson(StageSaveData.SaveData);
    //                File.WriteAllText(savePath, data);
    //                break;
    //        }
    //#if UNITY_EDITOR
    //        AssetDatabase.Refresh();
    //#endif
    //    }

    //    public T LoadData<T>(ESaveType type) where T : class
    //    {
    //        string savePath = Path.Combine(Application.dataPath, path[(int)type]);
    //        if(!File.Exists(savePath))
    //        {
    //            return null;
    //        }
    //        switch (type)
    //        {
    //            case ESaveType.STAGE:
    //                var data = File.ReadAllText(savePath);                                
    //                return JsonUtility.FromJson<T>(data);
    //            default:
    //                return null;
    //        }
    //    }
    #endregion
}
