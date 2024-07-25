using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroGacha : BaseGacha
{
    [SerializeField] private HeroSO[] commonPool;
    [SerializeField] private HeroSO[] rarePool;
    [SerializeField] private HeroSO[] epicPool;
    [SerializeField] private HeroSO[] legendPool;

    public HeroSO heroSO;
    public override void DoGacha()
    {
        base.DoGacha();

        switch (rarity)
        {
            case ERarityType.COMMON:
                heroSO = HeroSet(commonPool); 
                break;
            case ERarityType.RARE:
                heroSO = HeroSet(rarePool);
                break;
            case ERarityType.EPIC:
                heroSO = HeroSet(epicPool);
                break;
            case ERarityType.LEGEND:
                heroSO = HeroSet(legendPool);
                break;
            default:
                break;
        }

        HeroManager.Instance.HasHeroCheck(heroSO);


    }

    protected override void Start()
    {
        base.Start();

        GameManager.Instance.heroGacha = this;
    }

    private HeroSO HeroSet(HeroSO[] itemPool)
    {
        // 해당 등급의 아이템풀에서 무작위 인덱스 값을 설정
        int randomIndex = Random.Range(0, itemPool.Length);

        // 아이템 풀에서 가져온 SO를 통해 새로운 아이템 생성
        return itemPool[randomIndex];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //DoGacha();           

            for (int i = 0; i < 1; i++)
            {
                DoGacha();
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            UpgradeProbability();
        }
    }

}
