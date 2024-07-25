using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanelController : MonoBehaviour
{
    Image[] itemSlots;

    private void Awake()
    {
        itemSlots = gameObject.GetComponentsInChildren<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {      
        for ( int i = 0; i < gameObject.transform.childCount; i++ )
        {            
            StatManager.Instance.equipment.ItemSlot[i] = gameObject.transform.GetChild(i).GetComponent<Image>();
        }
    }
}
