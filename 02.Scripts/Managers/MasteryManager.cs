using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class MasteryManager : MonoBehaviour
{
    public List<MasteryInfo> MasteryList { get => UIMasterySlotList.Select(x => x.Mastery.Info).ToList(); }
    public List<UIMasterySlot> UIMasterySlotList;
    public List<BaseMastery> NodeList;

    private void Start()
    {
        GameManager.Instance.Mastery = this;
        NodeList = GetComponentsInChildren<BaseMastery>().ToList();
        for(int i = 0; i < NodeList.Count; i++)
        {
            UIMasterySlotList[i].Mastery = NodeList[i]; 
        }
        //UIMasterySlotList = GetComponentsInChildren<UIMasterySlot>().ToList();
        var data = DataManager.Instance.LoadData<List<MasteryInfo>>(ESaveType.MASTERY);
        if (data != null)
        {
            for(int i=0; i<data.Count; i++)
            {
                UIMasterySlotList[i].Mastery.Info = data[i];
            }
            for (int i = 0; i < data.Count; i++)
            {
                UIMasterySlotList[i].Mastery.Init();
            }
        }
    }

    private int GetIndex(MasteryInfo info)
    {
        var item = UIMasterySlotList.Find(x => x.Mastery.Info == info);

        return UIMasterySlotList.IndexOf(item);
    }

    public void UpdateNextNodeUI(MasteryInfo info)
    {
        int idx = GetIndex(info);
        UIMasterySlotList[idx].UpdateImage();
    }
}
