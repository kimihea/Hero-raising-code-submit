using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownController : MonoBehaviour
{
    public Dropdown dropdown;
    public GameObject[] panels;

    // Start is called before the first frame update
    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
    }

    void OnDropdownValueChanged(int index)
    {
        foreach (var panel in panels)
        {
            panel.SetActive(true);
        }

        if (index < panels.Length)
        {
            panels[index].SetActive(true);
        }
    }
}
