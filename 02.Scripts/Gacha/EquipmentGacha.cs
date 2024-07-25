using System.Collections;
using UnityEngine;
using DG.Tweening;

public class EquipmentGacha : BaseGacha
{
    [System.Serializable]
    public class tierItemPool
    {
        [SerializeField] public ItemSO[] commonPool;
        [SerializeField] public ItemSO[] rarePool;
        [SerializeField] public ItemSO[] epicPool;
        [SerializeField] public ItemSO[] legendPool;
    }
    /*
        [SerializeField] private ItemSO[] commonPool;
        [SerializeField] private ItemSO[] rarePool;
        [SerializeField] private ItemSO[] epicPool;
        [SerializeField] private ItemSO[] legendPool;*/

    public tierItemPool tier1Pool;
    public tierItemPool tier2Pool;
    public tierItemPool tier3Pool;

    private tierItemPool selectedItemPool;

    [System.Serializable]
    public class itemTierTable
    {
        public int tableLevel;

        public float tier1Probability;
        public float tier2Probability;
        public float tier3Probability;     
    }

    public itemTierTable tierTable;

    public int itemTier;

    public float delay = 5.0f;      // 딜레이 시간

    public GameObject equipItemPanel;

    IEnumerator doGacha;

    Equipment equipment;

    protected override void Start()
    {
        base.Start();

        equipment = StatManager.Instance.equipment;
    }

    public override void DoGacha()
    {
        SetItemTier();

        base.DoGacha();

        EquipItem equipItem = new EquipItem();

        switch (rarity)
        {
            case ERarityType.COMMON:
                equipItem.itemSO = ItemSet(selectedItemPool.commonPool);
                /*Debug.Log(equipItem.itemSO.equipmentType);
                Debug.Log($"<color=white>{equipItem.itemSO.name} </color>");*/
                
                break;
            case ERarityType.RARE:
                equipItem.itemSO = ItemSet(selectedItemPool.rarePool);
                /*Debug.Log(equipItem.itemSO.equipmentType);
                Debug.Log($"<color=#00C3FF>{equipItem.itemSO.name} </color>");*/
                
                break;
            case ERarityType.EPIC:
                equipItem.itemSO = ItemSet(selectedItemPool.epicPool);
                /*Debug.Log(equipItem.itemSO.equipmentType);
                Debug.Log($"<color=#BA55D3>{equipItem.itemSO.name} </color>");*/
                
                break;
            case ERarityType.LEGEND:
                equipItem.itemSO = ItemSet(selectedItemPool.legendPool);
                /*Debug.Log(equipItem.itemSO.equipmentType);
                Debug.Log($"<color=#FF9100>{equipItem.itemSO.name} </color>");*/                
                break;
            default:
                break;
        }

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
        tierTable.tableLevel++;   

        if (tierTable.tableLevel == 2)
        {
            tierTable.tier1Probability = 50;
            tierTable.tier2Probability = 50;
            tierTable.tier3Probability = 0;
        }
        else if (tierTable.tableLevel == 3)
        {
            tierTable.tier1Probability = 10;
            tierTable.tier2Probability = 50;
            tierTable.tier3Probability = 40;
        }
        else if (tierTable.tableLevel > 3)
        {
            tierTable.tier1Probability = 0;
            tierTable.tier2Probability = 40;
            tierTable.tier3Probability = 60;
        }
        else
        {
            Debug.Log("테이블 레벨 오류");
        }
    }

    protected override void InitTable()
    {
        drawProbability.tableLevel = 1;
        drawProbability.commonProbability = 70f;
        drawProbability.rareProbability = 20f;
        drawProbability.epicProbabilityf = 10f;
        drawProbability.legendProbability = 0f;

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

        if ( equipment.isGachaPossible )
        {
            equipment.isGachaPossible = false;

            doGacha = ShowEquipItemPanelWithDelay();
            StartCoroutine(doGacha);
        }
       /* else if (equipment.switchingItem.itemSO != null)
        {
            GameManager.Instance.player.equipment.OpenPopUP();
        }*/
        
    }
}
