using UnityEngine;
using UnityEngine.UI;
using System.Numerics;

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

    [SerializeField] Text heroSkillName;
    [SerializeField] Text heroSkillDesc;

    [SerializeField] Text heroEntryText;

    [SerializeField] Text heroUpgradeText;
    public Image heroUpGradetBar;
    [SerializeField] Text heroUpstarText;
    public Image heroUpStartBar;
    [Header("배치 목록")]
    public Image[] EntrySlots;
    public Image EntryDefaultImage;

    [Header("파티 스킬 배치 목록")]
    public Image[] AdditionalEntrySlots;

    public int selectIndex = -1;


    public GameObject ItemPopupOpenBtn;

    private void Awake()
    {        
        heroSlots = SlotLayoutGroup.GetComponentsInChildren<Button>();

    }

    // Start is called before the first frame update
    void Start()
    {
        PartyRefresh();
    }

    private void Update()
    {

        //Debug.Log(StatManager.Instance.equipment.switchingItem.itemSO.isEmpty);
        ItemPopupOpenBtn.SetActive(!StatManager.Instance.equipment.switchingItem.itemSO.isEmpty);

    }

    public void OnClickHeroSlot(int index)
    {
        selectIndex = index;

        partyChoice.SetActive(true);

        int heroKey = HeroManager.Instance.hidList[selectIndex];

        Hero selectedHero = HeroManager.Instance.heroDict[heroKey];
        selectedHero.StatHandler.UpdateStatModifier();


        heroImage.sprite = selectedHero.data.icon;

        heroName.text = $"{selectedHero.data.heroName}";
        heroDesc.text = $"{selectedHero.data.heroDescription}";

        Skill heroSkill = selectedHero.gameObject.GetComponentInChildren<Skill>();

        heroSkillImage.sprite = heroSkill.Data.Icon;
        heroSkillName.text = heroSkill.Data.Name;
        heroSkillDesc.text = heroSkill.GetSkillDescription(1);

        heroEntryText.text = HeroManager.Instance.heroEntry.Contains(selectedHero) ? "배치 해제" : "영웅 배치";

        // UpgradeCost를 BigInteger로 변환하여 축약된 형식으로 표시
        BigInteger upgradeCost = new BigInteger(selectedHero.UpgradeDefaultCost + (selectedHero.GradeLevel * selectedHero.UpgradeIncreaseCost));
        BigInteger currentHeroEssence = CurrencyManager.Instance.GetCurrencyAmount(ECurrencyType.HeroEssence);

        heroUpgradeText.text = $"{CurrencyManager.Instance.GetCurrencyAmount(ECurrencyType.HeroEssence)} / {upgradeCost}";
        heroUpGradetBar.color = 1f >= (float)currentHeroEssence / (float)upgradeCost ? Color.grey : Color.green;

        int upstarCost = selectedHero.UpstarDefaultCost + (selectedHero.StarsLevel * selectedHero.UpstarIncreaseCost);
        heroUpStartBar.fillAmount = (float)CurrencyManager.Instance.HeroFragmentDict[heroKey]?.Amount / upstarCost;
        heroUpstarText.text = $" {CurrencyManager.Instance.HeroFragmentDict[heroKey]?.Amount ?? 0} / {upstarCost}";
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
            partyChoice.SetActive(false);
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
        if (GameManager.Instance.battleType == EBattleType.GOLDDUNGEON) return;
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

        for (int i = 0; i < EntrySlots.Length; i++)
        {
            if (HeroManager.Instance.heroEntry.Count <= i)
            {
                EntrySlots[i].sprite = EntryDefaultImage.sprite;
                //if (i < AdditionalEntrySlots.Length)
                //{
                //    AdditionalEntrySlots[i].sprite = EntryDefaultImage.sprite;
                //}
                continue;
            }

            EntrySlots[i].sprite = HeroManager.Instance.heroEntry[i].data.icon;
            //if (i < AdditionalEntrySlots.Length)
            //{
            //    AdditionalEntrySlots[i].sprite = HeroManager.Instance.heroEntry[i].data.icon;
            //}
        }

        HeroManager.Instance.DataUpdate();
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
                //Debug.Log("엔트리 자리가 다 참");
                return;
            }

            HeroManager.Instance.heroEntry.Add(selectedHero);
        }

        //heroEntryText.text = HeroManager.Instance.heroEntry.Contains(selectedHero) ? "배치 해제" : "영웅 배치";

        //Debug.Log(selectedHero.data.heroName);

        PartyRefresh();
    }

    public void HeroUpgrade()
    {
        int heroKey = HeroManager.Instance.hidList[selectIndex];
        Hero selectedHero = HeroManager.Instance.heroDict[heroKey];

        // UpgradeCost를 계산하고 BigInteger로 변환
        BigInteger upgradeCost = new BigInteger(selectedHero.UpgradeDefaultCost + (selectedHero.GradeLevel * selectedHero.UpgradeIncreaseCost));

        // 현재 HeroEssence도 BigInteger로 가져오기
        BigInteger currentHeroEssence = CurrencyManager.Instance.GetCurrencyAmount(ECurrencyType.HeroEssence);

        if (CurrencyManager.Instance.CurrencyDict[ECurrencyType.HeroEssence].TrySpend((int)upgradeCost))
        {
            selectedHero.GradeLevel++;
            selectedHero.StatHandler.grade = selectedHero.GradeLevel;
            selectedHero.StatHandler.UpdateStatModifier();
            //selectedHero.StatHandler.AddStatModifier(selectedHero.data.gradeStatModifier);

            upgradeCost = new BigInteger(selectedHero.UpgradeDefaultCost + (selectedHero.GradeLevel * selectedHero.UpgradeIncreaseCost));
            heroUpgradeText.text = $"{CurrencyManager.Instance.GetCurrencyAmount(ECurrencyType.HeroEssence).ToAbbreviatedString()} / {upgradeCost.ToAbbreviatedString()}";
            heroUpGradetBar.color = 1f >= (float)CurrencyManager.Instance.GetCurrencyAmount(ECurrencyType.HeroEssence) / (float)upgradeCost ? Color.grey : Color.green;

            QuestManager.Instance.AddProgress(EQuestType.HEROUPGRADE, 1);

            PartyRefresh();
        }
    }

    public void HeroUpstar()
    {
        int heroKey = HeroManager.Instance.hidList[selectIndex];
        Hero selectedHero = HeroManager.Instance.heroDict[heroKey];

        int upstarCost = selectedHero.UpstarDefaultCost + (selectedHero.StarsLevel * selectedHero.UpstarIncreaseCost);

        if (CurrencyManager.Instance.HeroFragmentDict[heroKey].TrySpend(upstarCost))
        {
            selectedHero.StarsLevel++;
            selectedHero.StatHandler.stars = selectedHero.GradeLevel;
            selectedHero.StatHandler.UpdateStatModifier();

            //selectedHero.StatHandler.AddStatModifier(selectedHero.data.gradeStatModifier);
            //TODO : 
            HeroManager.Instance.statHandler.AddStatModifier(selectedHero.data.PassiveStat);

            upstarCost = selectedHero.UpstarDefaultCost + (selectedHero.StarsLevel * selectedHero.UpstarIncreaseCost);
            heroUpstarText.text = $"{CurrencyManager.Instance.HeroFragmentDict[heroKey]?.Amount ?? 0}/{upstarCost}";
            heroUpStartBar.fillAmount =(float)CurrencyManager.Instance.HeroFragmentDict[heroKey]?.Amount / upstarCost;

            QuestManager.Instance.AddProgress(EQuestType.HEROUPSTAR, 1);

            PartyRefresh();
        }
    }

    public void OnItemPopUp()
    {
        StatManager.Instance.equipment.OpenPopUP();
    }

}
