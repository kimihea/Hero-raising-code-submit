using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanelController : MonoBehaviour
{
    public Image[] itemSlotIconArray;

    public Image[] itemSlotBackgroundArray;
    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {      
        for ( int i = 0; i < gameObject.transform.childCount; i++ )
        {            
            StatManager.Instance.equipment.ItemSlotIconArray[i] = itemSlotIconArray[i];
            StatManager.Instance.equipment.ItemSlotBackgroundArray[i] = itemSlotBackgroundArray[i];
        }
    }
}
