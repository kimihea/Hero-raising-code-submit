using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIPartyMenu : MonoBehaviour
{
    public Button EntryChangeBtn;
    public MainMenu Menu;
    public UIPartyEntry Entry;

    private void OnEnable()
    {
        //if (!GameManager.isInit) return;

        HeroManager.Instance.heroEntry = GameManager.Instance.GetHeroEntry();

        Menu.PartyRefresh();
    }

    public void OnEntryChangeBtnClick()
    {
        bool isEqual = Enumerable.SequenceEqual(GameManager.Instance.GetHeroEntry(), HeroManager.Instance.heroEntry);
        if (isEqual) return;
        QuestManager.Instance.AddProgress(EQuestType.HEROENTRY, 1);
        GameManager.Instance.ChangeEntry();
        Entry.UpdateUI();
    }
}
