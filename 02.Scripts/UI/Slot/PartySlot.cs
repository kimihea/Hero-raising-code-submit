using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySlot : MonoBehaviour
{
    public Image[] synergyIcons;

    [Header("영웅 목록")]
    [SerializeField] GameObject SlotLayoutGroup;
    [SerializeField]  Button[] heroSlots;

    [Header("영웅 팝업창")]
    [SerializeField] GameObject partyChoice;
    [SerializeField] Image heroImage;
    [SerializeField] Image synergyImage;
    [SerializeField] Text heroStat;
    [SerializeField] Image heroSkillImage;
    [SerializeField] Text heroSkillDesc;



    private void Awake()
    {
        SlotLayoutGroup = GetComponentsInChildren<GridLayoutGroup>()[1].gameObject;

        heroSlots = SlotLayoutGroup.GetComponentsInChildren<Button>();

        partyChoice = transform.GetChild(2).gameObject;
        heroImage = partyChoice.GetComponentsInChildren<Image>()[1];
        synergyImage = partyChoice.GetComponentsInChildren<Image>()[2];
        heroStat = partyChoice.GetComponentsInChildren<Image>()[3].gameObject.GetComponent<Text>();
        heroSkillImage = partyChoice.GetComponentsInChildren<Image>()[4];
        heroSkillDesc = partyChoice.GetComponentsInChildren<Image>()[5].gameObject.GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for ( int i = 0; i < heroSlots.Length; i++ )
        {
            HeroManager.Instance.heroSlot[i] = heroSlots[i].gameObject.GetComponent<Image>();
        }

        this.gameObject.SetActive( false );
    }

    public void OnClickHeroSlot(int index)
    {
        partyChoice.SetActive(true);

        int heroKey = HeroManager.Instance.hidList[index];
        heroImage.sprite = HeroManager.Instance.heroDict[heroKey].data.icon;

    }
}
