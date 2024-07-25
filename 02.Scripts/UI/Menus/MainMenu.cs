using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject RaisMenu;
    public GameObject PartyMenu;
    public GameObject PartyGachaMenu;
    public GameObject SkillMenu;
    public GameObject DungenMenu;

    public Image[] synergyIcons;

    [Header("영웅 목록")]
    [SerializeField] GameObject SlotLayoutGroup;
    [SerializeField] Button[] heroSlots;

    [Header("영웅 팝업창")]
    [SerializeField] GameObject partyChoice;
    [SerializeField] Image heroImage;
    [SerializeField] Image synergyImage;
    [SerializeField] Text heroName;
    [SerializeField] Text heroDesc;
    [SerializeField] Image heroSkillImage;
    [SerializeField] Text heroSkillDesc;

    [SerializeField] Text heroEntryText;

    [Header("배치 목록")]
    public Image[] EntrySlots;
    public Image EntryDefaultImage;

    public int selectIndex = -1;

    private void Awake()
    {        
        heroSlots = SlotLayoutGroup.GetComponentsInChildren<Button>();

    }

    // Start is called before the first frame update
    void Start()
    {
        PartyRefresh();
    }

    public void OnClickHeroSlot(int index)
    {
        selectIndex = index;

        partyChoice.SetActive(true);

        int heroKey = HeroManager.Instance.hidList[index];

        Debug.Log(heroKey);
        Hero selectedHero = HeroManager.Instance.heroDict[heroKey];

        heroImage.sprite = selectedHero.data.icon;

        heroName.text = $"{selectedHero.data.heroName}";
        heroDesc.text = $"{selectedHero.data.heroDescription}";

        heroEntryText.text = HeroManager.Instance.heroEntry.Contains(selectedHero) ? "배치 해제" : "영웅 배치";

        // 추후 시너지 이미지 추가 시 구현
    }    

    // 성장 메뉴
    public void OnClickRaisMenu()
    {
        if (RaisMenu.activeSelf)
        {
            RaisMenu.SetActive(false);
        }
        else
        {
            RaisMenu.SetActive(true);
        }
    }

    // 파티 메뉴
    public void OnClickPartyMenu()
    {
        if (PartyMenu.activeSelf)
        {
            PartyMenu.SetActive(false);
        }
        else
        {
            PartyRefresh();

            PartyMenu.SetActive(true);
        }
    }

    // 동료모집 메뉴
    public void OnClickGachaMenu()
    {
        if (PartyGachaMenu.activeSelf)
        {
            PartyGachaMenu.SetActive(false);
        }
        else
        {
            PartyGachaMenu.SetActive(true);
        }
    }


    // 스킬 메뉴
    public void OnClickSkillMenu()
    {
        if (SkillMenu.activeSelf)
        {
            SkillMenu.SetActive(false);
        }
        else
        {
            SkillMenu.SetActive(true);
        }
    }


    // 던전 메뉴
    public void OnClickDungenMenu()
    {
        if (DungenMenu.activeSelf)
        {
            DungenMenu.SetActive(false);
        }
        else
        {
            DungenMenu.SetActive(true);
        }
    }


    public void PartyRefresh()
    {
        for (int i = 0; i < heroSlots.Length; i++)
        {
            if (HeroManager.Instance.hidList.Count <= i )
            {
                heroSlots[i].interactable = false;
                continue;
            }

            HeroManager.Instance.heroSlot[i] = heroSlots[i].gameObject.GetComponent<Image>();

            int heroKey = HeroManager.Instance.hidList[i];

            heroSlots[i].interactable = HeroManager.Instance.heroDict.ContainsKey(heroKey);
            
        }

        for ( int i = 0; i < EntrySlots.Length; i++)
        {
            if (HeroManager.Instance.heroEntry.Count <= i)
            {
                EntrySlots[i].sprite = EntryDefaultImage.sprite;
                continue;
            }

            EntrySlots[i].sprite = HeroManager.Instance.heroEntry[i].data.icon;
        }
    }

    public void OnClickPartyPosition()
    {
        int heroKey = HeroManager.Instance.hidList[selectIndex];
        Hero selectedHero = HeroManager.Instance.heroDict[heroKey];

        if (HeroManager.Instance.heroEntry.Contains(selectedHero) )
        {
            HeroManager.Instance.heroEntry.Remove(selectedHero);
            //HeroManager.Instance.heroEntry.Add(selectedHero);
        }
        else
        {
            if (HeroManager.Instance.heroEntry.Count >= EntrySlots.Length)
            {
                Debug.Log("엔트리 자리가 다 참");
                return;
            }

            HeroManager.Instance.heroEntry.Add(selectedHero);
        }

        heroEntryText.text = HeroManager.Instance.heroEntry.Contains(selectedHero) ? "배치 해제" : "영웅 배치";

        Debug.Log(selectedHero.data.heroName);

        PartyRefresh();
    }
}
