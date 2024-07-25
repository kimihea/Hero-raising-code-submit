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


    [SerializeField] private bool isGachaPossible = true;

    void Start()
    {
        InitializeSlots(slotsX1);
        InitializeSlots(slotsX10);

        // 버튼 클릭 이벤트 코루틴 할당
        recruitX10Btn.onClick.AddListener(() =>
        {   
            if (isGachaPossible)
            {
                InitializeSlots(slotsX1);   // 10회 모집 버튼 활성화 시 1회 모집 슬롯 초기화
                StartCoroutine(RecruitSlots(slotsX10, 10));
            }
            
        });
        recruitX1Btn.onClick.AddListener(() =>
        {
            if (isGachaPossible)
            {
                InitializeSlots(slotsX10);  // 1회 모집 버튼 활성화 시 10회 모집 슬롯 초기화
                StartCoroutine(RecruitSlots(slotsX1, 1));
            }
        });
    }

    IEnumerator RecruitSlots(GameObject[] slots,int count)
    {
        isGachaPossible = false;
        InitializeSlots(slots);

        GameObject[] viewslots = count == 1 ? slotsX1 : slotsX10;

        for ( int i = 0; i < count; i++ )
        {
            GameManager.Instance.heroGacha.DoGacha();

            HeroSO so = GameManager.Instance.heroGacha.heroSO;

            viewslots[i].GetComponent<Image>().sprite = so.icon;
        }


        yield return StartCoroutine(CoroutineManager.Instance.ShowSlotsWithDelay(slots, delay, count));

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
    }
}
