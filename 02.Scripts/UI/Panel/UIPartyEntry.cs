using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPartyEntry : MonoBehaviour
{
    public List<Image> EntrySlotList;
    public Sprite DefaultImage;

    public void UpdateUI()
    {
        for(int i = 0; i < EntrySlotList.Count; i++)
        {
            if(HeroManager.Instance.heroEntry.Count > i)
            {
                EntrySlotList[i].sprite = HeroManager.Instance.heroEntry[i].data.icon;
            }
            else
            {
                EntrySlotList[i].sprite = DefaultImage;
            }
        }
    }
}
