using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRaisMenu : MonoBehaviour
{
    public List<Toggle> ToggleList;
    public List<GameObject> PanelList;

    public void OnToggleValueChanged()
    {
        for(int i = 0; i < ToggleList.Count; i++)
        {
            if (ToggleList[i].isOn)
            {
                PanelList[i].SetActive(true);
            }
            else
            {
                PanelList[i].SetActive(false);
            }
        }
    }
}
