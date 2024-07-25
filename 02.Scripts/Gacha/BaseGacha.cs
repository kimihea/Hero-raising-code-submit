using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGacha : MonoBehaviour
{

    public RarityTable drawProbability;

    public ERarityType rarity;

    protected virtual void Start()
    {
        InitTable();
    }

    protected virtual void InitTable()
    {
        drawProbability.tableLevel = 1;

        drawProbability.commonProbability = 70f;
        drawProbability.rareProbability = 20f;
        drawProbability.epicProbabilityf = 10f;
        drawProbability.legendProbability = 0f;
    }


    public virtual void DoGacha()
    {
        ChooseRarity();

       // Debug.Log(rarity);
    }

    public void ChooseRarity()
    {
        // 가챠 시 실행되는 랜덤 값
        float random = Random.Range(0f, 100f);

        if (random < drawProbability.legendProbability)
        {
            rarity = ERarityType.LEGEND;
            return;
        }
        else if (random < drawProbability.legendProbability + drawProbability.epicProbabilityf )
        {
            rarity = ERarityType.EPIC;
            return;
        }
        else if (random < drawProbability.legendProbability + drawProbability.epicProbabilityf + drawProbability.rareProbability)
        {
            rarity = ERarityType.RARE;
            return;
        }
        else if (random < drawProbability.legendProbability + drawProbability.epicProbabilityf + drawProbability.rareProbability + drawProbability.commonProbability)
        {
            rarity = ERarityType.COMMON;
            return;
        }
        else
        {
            Debug.Log("무언가 잘못 됨");
        }
    }

    protected virtual void UpgradeProbability()
    {
        drawProbability.tableLevel++;

        drawProbability.commonProbability -= 3;
        drawProbability.rareProbability++;
        drawProbability.epicProbabilityf++;
        drawProbability.legendProbability++;

    }


}
