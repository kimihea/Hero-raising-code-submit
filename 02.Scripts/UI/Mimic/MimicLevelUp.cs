using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MimicLevelUp : MonoBehaviour
{
    public Slider levelSlider;
    public Text levelText;
    public Image mimicImage;

    private int currentLevel = 1;
    private int maxLevel = 10;
    private int currentXP = 0;
    private int xpForNextLevel = 100;
    private int equipmentFragments = 0;

    void Start()
    {
        levelSlider.maxValue = xpForNextLevel;
        UpdateUI();
    }

    void UpdateUI()
    {
        levelSlider.value = currentXP;
        levelText.text = "Lv: " + currentLevel;
    }

    public void AddFragments(int amount)
    {
        CurrencyManager.Instance.AddCurrency(ECurrencyType.ManaStone, amount);
        UseFragments();
        UpdateUI();
    }

    void UseFragments()
    {
        while (CurrencyManager.Instance.GetCurrencyAmount(ECurrencyType.ManaStone) > 0 && currentLevel < maxLevel)
        {
            if (CurrencyManager.Instance.UseCurrency(ECurrencyType.ManaStone, 1)) // 업그레이드 스톤 1개당 10 XP
            {
                currentXP += 10;
                equipmentFragments--;
                if (currentXP >= xpForNextLevel)
                {
                    currentXP -= xpForNextLevel;
                    LevelUp();
                }
            }
            else
            {
                break;
            }
        }
    }

    void LevelUp()
    {
        currentLevel++;
        xpForNextLevel += 50;
        levelSlider.maxValue = xpForNextLevel;

        if (currentLevel > maxLevel)
        {
            currentLevel = maxLevel;
            currentXP = 0;
        }
    }
}
