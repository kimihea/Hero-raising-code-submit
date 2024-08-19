using System.Collections;
using UnityEngine;
using DG.Tweening;

public class EquipmentGacha : BaseGacha
{
    [System.Serializable]
    public class TierItemPool
    {
        [SerializeField] public ItemSO[] commonPool;
        [SerializeField] public ItemSO[] rarePool;
        [SerializeField] public ItemSO[] epicPool;
        [SerializeField] public ItemSO[] legendPool;
    }

    public TierItemPool tier1Pool;
    public TierItemPool tier2Pool;
    public TierItemPool tier3Pool;

    private TierItemPool selectedItemPool;

    [System.Serializable]
    public class ItemTierTable
    {
        public int tableLevel;

        public float tier1Probability;
        public float tier2Probability;
        public float tier3Probability;     
    }

    public ItemTierTable tierTable;

    public int itemTier;

    public float delay = 5.0f;      // 딜레이 시간

    public GameObject equipItemPanel;

    IEnumerator doGacha;

    Equipment equipment;

    protected override void Start()
    {
        base.Start();

        equipment = StatManager.Instance.equipment;

        MimicManager.Instance.EquipmentGacha = this;
    }

    public override void DoGacha()
    {
        if (CurrencyManager.Instance.TryDrawEquipment())
        {
            SetItemTier();
            base.DoGacha();

            EquipItem equipItem = new();

            switch (rarity)
            {
                case ERarityType.COMMON:
                    equipItem.itemSO = new ItemInfo(ItemSet(selectedItemPool.commonPool));
                    break;
                case ERarityType.RARE:
                    equipItem.itemSO = new ItemInfo(ItemSet(selectedItemPool.rarePool));
                    break;
                case ERarityType.EPIC:
                    equipItem.itemSO = new ItemInfo(ItemSet(selectedItemPool.epicPool));
                    break;
                case ERarityType.LEGEND:
                    equipItem.itemSO = new ItemInfo(ItemSet(selectedItemPool.legendPool));
                    break;
                default:
                    break;
            }


            ItemRandomStatSetting(equipItem);

            equipment.switchingItem = equipItem;
            equipment.OpenPopUP();

            // 애니메이션 설정
            Transform gameObjectTransform = equipItemPanel.transform.Find("GameObject");
            if (gameObjectTransform == null)
            {
                return;
            }

            Transform newItemBG = gameObjectTransform.Find("NewItemBG");
            Transform currentItemBG = gameObjectTransform.Find("CurrentItemBG");

            if (newItemBG == null || currentItemBG == null)
            {
                return;
            }

            // RectTransform 애니메이션 설정
            RectTransform newRect = newItemBG.GetComponent<RectTransform>();
            RectTransform currentRect = currentItemBG.GetComponent<RectTransform>();
            newRect.localScale = Vector3.one * 0f;
            currentRect.localScale = Vector3.one * 0f;

            // 애니메이션 실행
            newRect.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce);
            currentRect.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce);

            StopCoroutine(doGacha);
        }
        else
        {
            Equipment.isGachaPossible = true;   // 초기화
        }
    }

    private ItemSO ItemSet(ItemSO[] itemPool)
    {
        // 해당 등급의 아이템풀에서 무작위 인덱스 값을 설정
        int randomIndex = Random.Range(0, itemPool.Length);

        // 아이템 풀에서 가져온 SO를 통해 새로운 아이템 생성
        return itemPool[randomIndex];
    }

    private void Update()
    {        
        if (Input.GetKeyDown(KeyCode.R))
        {
            UpgradeProbability();
        }
    }

    private void SetItemTier()
    {
        float random = Random.Range(0f, 100f);

        float probability = 0f;

        probability += tierTable.tier3Probability;
        if (random < probability)
        {
            selectedItemPool = tier3Pool;
            return;
        }

        probability += tierTable.tier2Probability;
        if (random < probability)
        {
            selectedItemPool = tier2Pool;
            return;
        }

        probability += tierTable.tier1Probability;
        if (random < probability)
        {
            selectedItemPool = tier1Pool;
            return;
        }
        else
        {
            Debug.Log("무언가 잘못 됨");
        }
    }

    protected override void UpgradeProbability()
    {
        //tierTable.tableLevel++;   

        if (tierTable.tableLevel == 2)
        {
            tierTable.tier1Probability = 90;
            tierTable.tier2Probability = 10;
            tierTable.tier3Probability = 0;
        }
        else if (tierTable.tableLevel == 3)
        {
            tierTable.tier1Probability = 50;
            tierTable.tier2Probability = 50;
            tierTable.tier3Probability = 0;
        }
        else if (tierTable.tableLevel == 4)
        {
            tierTable.tier1Probability = 0;
            tierTable.tier2Probability = 90;
            tierTable.tier3Probability = 10;
        }
        else if (tierTable.tableLevel > 4)
        {
            tierTable.tier1Probability = 0;
            tierTable.tier2Probability = 0;
            tierTable.tier3Probability = 100;
        }
        else
        {
            Debug.Log("테이블 레벨 오류");
        }
    }

    protected override void InitTable()
    {
        drawProbability.tableLevel = 1;
        drawProbability.commonProbability = 68;
        drawProbability.rareProbability = 20;
        drawProbability.epicProbabilityf = 10f;
        drawProbability.legendProbability = 2f;

        tierTable.tableLevel = 1;
        tierTable.tier1Probability = 100;
        tierTable.tier2Probability = 0;
        tierTable.tier3Probability = 0;
    }


    public IEnumerator ShowEquipItemPanelWithDelay()
    {
        yield return new WaitForSeconds(delay); // 설정된 시간만큼 기다립니다.        
        //equipItemPanel.SetActive(true); // EquipItemPanel을 활성화합니다.

        DoGacha();
    }

    public void StartGacha()
    {
        if ( Equipment.isGachaPossible )
        {
            Equipment.isGachaPossible = false;

            doGacha = ShowEquipItemPanelWithDelay();
            StartCoroutine(doGacha);
        }      
    }

    public void ItemRandomStatSetting(EquipItem equipItem)
    {
        equipItem.GradeStatModifier.Atk = ItemRandomValue(equipItem.itemSO.PassiveStat.Atk);
        equipItem.GradeStatModifier.Health = ItemRandomValue(equipItem.itemSO.PassiveStat.Health);
        equipItem.GradeStatModifier.Defense = ItemRandomValue(equipItem.itemSO.PassiveStat.Defense);
        equipItem.GradeStatModifier.AttackSpeed = ItemRandomValue(equipItem.itemSO.PassiveStat.AttackSpeed);

        equipItem.GradeStatModifier.CritRate = ItemRandomValue(equipItem.itemSO.PassiveStat.CritRate);
        equipItem.GradeStatModifier.CritMultiplier = ItemRandomValue(equipItem.itemSO.PassiveStat.CritMultiplier);
        equipItem.GradeStatModifier.SkillMultiplier = ItemRandomValue(equipItem.itemSO.PassiveStat.SkillMultiplier);
        equipItem.GradeStatModifier.DamageMultiplier = ItemRandomValue(equipItem.itemSO.PassiveStat.DamageMultiplier);
        equipItem.GradeStatModifier.HealMultiplier = ItemRandomValue(equipItem.itemSO.PassiveStat.HealMultiplier);
    }

    private float ItemRandomValue(float passiveStat)
    {
        int rangeValue = (int)(passiveStat / 10);

        return Random.Range( -rangeValue, rangeValue + 1 );
    }

    public void UpdateProbability()
    {
        switch(tierTable.tableLevel)
        {
            case 1:
                tierTable.tier1Probability = 100;
                tierTable.tier2Probability = 0;
                tierTable.tier3Probability = 0;
                break;
            case 2:
                tierTable.tier1Probability = 90;
                tierTable.tier2Probability = 10;
                tierTable.tier3Probability = 0;
                break;
            case 3:
                tierTable.tier1Probability = 50;
                tierTable.tier2Probability = 50;
                tierTable.tier3Probability = 0;
                break;
            case 4:
                tierTable.tier1Probability = 0;
                tierTable.tier2Probability = 90;
                tierTable.tier3Probability = 10;
                break;
            case 5:
                tierTable.tier1Probability = 0;
                tierTable.tier2Probability = 0;
                tierTable.tier3Probability = 100;
                break;
            default:
                Debug.Log("예외 처리");
                break;

        }
    }
}
