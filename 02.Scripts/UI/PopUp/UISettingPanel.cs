using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettingPanel : MonoBehaviour, IUIPopUp
{
    public Slider MasterVol;
    public Slider BGMVol;
    public Slider SFXVol;    
    public Text ExitTxt;
    private AudioManager audioMixer;
    private GameManager manager;
    public GameObject AudioBtn;
    public GameObject ExitBtn;
    public GameObject AudioPanel;

    public void Hide()
    {   
        gameObject.SetActive(false);
    }

    public void Show()
    {
        UIManager.Instance.PushPopUp(this);
        gameObject.SetActive(true);
        audioMixer = AudioManager.Instance;
        manager = GameManager.Instance;
        AudioBtn.SetActive(true);
        ExitBtn.SetActive(true);
        AudioPanel.SetActive(false);
        UpdateUI();
    }

    public void UpdateUI()
    {
        float volume;
        audioMixer.GetAudioMixerVolume(EAudioMixerType.MASTER, out volume);
        MasterVol.value = volume;
        audioMixer.GetAudioMixerVolume(EAudioMixerType.BGM, out volume);
        BGMVol.value = volume;
        audioMixer.GetAudioMixerVolume(EAudioMixerType.SFX, out volume);
        SFXVol.value = volume;
        switch(manager.battleType)
        {
            case EBattleType.STAGE:
                ExitTxt.text = "종료하기";
                break;
            case EBattleType.GOLDDUNGEON:
                ExitTxt.text = "나가기";
                break;
            default:
                break;
        }
    }

    public void OnExitBtnClick()
    {
        switch (manager.battleType)
        {
            case EBattleType.STAGE:
                DataManager.Instance.SaveData();
                Application.Quit();
                break;
            case EBattleType.GOLDDUNGEON:
                GameManager.Instance.GoldDungeon.CombatObject.GetComponent<GoldDungeonCombat>().Defeat();
                UIManager.Instance.CloseUI();
                break;
            default:
                break;
        }
    }

    public void OnChangedVolume(int num)
    {
        EAudioMixerType type = (EAudioMixerType)num;
        switch (type)
        {
            case EAudioMixerType.MASTER:
                audioMixer.SetAudioMixerVolume(type, MasterVol.value); 
                break;
            case EAudioMixerType.BGM:
                audioMixer.SetAudioMixerVolume(type, BGMVol.value);
                break;
            case EAudioMixerType.SFX:
                audioMixer.SetAudioMixerVolume(type, SFXVol.value);
                break;
        }
    }
}
