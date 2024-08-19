using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EMasteryCondition
{
    LOCK,
    CANRESEARCHING,
    ISRESEARCHING,
    DONE
}

[Serializable]
public class MasteryInfo
{
    public EMasteryCondition Condition;
    public float CurResearchTime;
    public IdleTime TotalResearchTime;
    //public IdleTime LastUpdateTime;
    public string ImageRcode;
    public string Description;

    public float GetResearchRate()
    {
        return Mathf.Min(CurResearchTime / TotalResearchTime.GetSeconds(), 1f);
    }
}

public abstract class BaseMastery : MonoBehaviour
{
    [SerializeField] public MasteryInfo Info;
    [SerializeField] public List<BaseMastery> NextMasterys;

    public void Init()
    {
        if(Info.Condition == EMasteryCondition.ISRESEARCHING)
        {
            TimeSpan timeSpan = DateTime.Now - DataManager.Instance.UserLoadData.LastUpdateTime.GetDateTime();
            float elapsedTime = (float)timeSpan.TotalSeconds;
            Info.CurResearchTime += elapsedTime;
            ResearchDone();
        }        
    }

    private void Update()
    {
        if (Info.Condition == EMasteryCondition.ISRESEARCHING)
        {
            Info.CurResearchTime += Time.deltaTime;
            //Info.LastUpdateTime.ConvertFromDateTime(DateTime.Now);
            ResearchDone();
        }
    }

    private void ResearchDone()
    {
        if (Info.CurResearchTime >= Info.TotalResearchTime.GetSeconds())
        {
            Info.Condition = EMasteryCondition.DONE;
            ApplyMastery();
            GameManager.Instance.Mastery.UpdateNextNodeUI(Info);
            foreach (var node in NextMasterys)
            {
                node.Info.Condition = EMasteryCondition.CANRESEARCHING;
                GameManager.Instance.Mastery.UpdateNextNodeUI(node.Info);
            }            
        }
    }

    public string GetResearchTimeTxt()
    {
        float remainTime = Info.TotalResearchTime.GetSeconds() - Info.CurResearchTime;
        TimeSpan time = TimeSpan.FromSeconds(remainTime);
        if(time.Days > 0) return string.Format("{0}d", time.Days);
        if (time.Hours > 0) return string.Format("{0}h", time.Hours);
        if (time.Minutes > 0) return string.Format("{0}m", time.Minutes);
        return string.Format("{0}s", time.Seconds);     
    }

    public abstract void ApplyMastery();
}
