using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PartyGachaPanelController : MonoBehaviour
{
    public GameObject[] slotsX1;    // 1회 모집 슬롯 배열
    public GameObject[] slotsX10;   // 10회 모집 슬롯 배열
    public Button recruitX10Btn;
    public Button recruitX1Btn;
    public float delay = 0.5f;      // 슬롯이 나타나는 딜레이 시간

    private const int CostX1 = 300;     // X1 모집 다이아몬드 소모량
    private const int CostX10 = 3000;   // X10 모집 다이아몬드 소모량

    [SerializeField] private bool isGachaPossible = true;

    IEnumerator co;

    void Start()
    {
        InitializeSlots(slotsX1);
        InitializeSlots(slotsX10);



        // 버튼 클릭 이벤트 코루틴 할당
        recruitX10Btn.onClick.AddListener(() =>
        {   
            if (isGachaPossible && CurrencyManager.Instance.UseCurrency(ECurrencyType.Diamond, CostX10))
            {       
                if ( co != null )
                {                    
                    StopCoroutine(co);
                }

                HeroGacha(10, slotsX10);

                InitializeSlots(slotsX1);   // 10회 모집 버튼 활성화 시 1회 모집 슬롯 초기화
                StartCoroutine(RecruitSlots(slotsX10, 10));
            }
            else
            {
                //Debug.Log("다이아몬드가 부족합니다.");
            }

        });
        recruitX1Btn.onClick.AddListener(() =>
        {
            if (isGachaPossible && CurrencyManager.Instance.UseCurrency(ECurrencyType.Diamond, CostX1))
            {
                if (co != null)
                {
                    StopCoroutine(co);
                }

                HeroGacha(1, slotsX1);

                InitializeSlots(slotsX10);  // 1회 모집 버튼 활성화 시 10회 모집 슬롯 초기화
                StartCoroutine(RecruitSlots(slotsX1, 1));
            }
            else
            {
                //Debug.Log("다이아몬드가 부족합니다.");
            }
        });
    }

    IEnumerator RecruitSlots(GameObject[] slots,int count)
    {        
        InitializeSlots(slots);

        co = CoroutineManager.Instance.ShowSlotsWithDelay(slots, delay, count);

        //yield return StartCoroutine(CoroutineManager.Instance.ShowSlotsWithDelay(slots, delay, count));
        yield return StartCoroutine(co);

        isGachaPossible = true;
    }

    void InitializeSlots(GameObject[] slots)
    {
        foreach (GameObject slot in slots)
        {
            slot.SetActive(false);
        }
    }

    // 판넬이 비활성화될 때 슬롯 초기화
    private void OnDisable()
    {
        InitializeSlots(slotsX1);
        InitializeSlots(slotsX10);

        isGachaPossible = true;
    }

    void HeroGacha(int count, GameObject[] viewslots)
    {
        isGachaPossible = false;

        for (int i = 0; i < count; i++)
        {
            GameManager.Instance.heroGacha.DoGacha();

            HeroSO so = GameManager.Instance.heroGacha.heroSO;

            QuestManager.Instance.AddProgress(EQuestType.HEROGACHA, 1);

            viewslots[i].GetComponent<Image>().sprite = so.icon;
        }
    }
}
