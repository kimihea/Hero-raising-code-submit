using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : Singleton<CoroutineManager>
{
    //private static CoroutineManager instance;

    //public static CoroutineManager Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            GameObject obj = new GameObject("CoroutineManager");
    //            instance = obj.AddComponent<CoroutineManager>();
    //            DontDestroyOnLoad(obj); // 씬이 바뀌어도 파괴되지 않도록 설정
    //        }
    //        return instance;
    //    }
    //}

    //public IEnumerator ShowEquipItemPanelWithDelay(GameObject equipItemPanel, float delay)
    //{
    //    yield return new WaitForSeconds(delay); // 설정된 시간만큼 기다립니다.
    //    equipItemPanel.SetActive(true); // EquipItemPanel을 활성화합니다.

    //    RectTransform rect = equipItemPanel.GetComponent<RectTransform>();
    //    rect.localScale = Vector3.one;
    //    rect.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce);

    //}

    // PartyGachaPanel 쪽 PartySlots 딜레이 설정
    public IEnumerator ShowSlotsWithDelay(GameObject[] slots, float delay, int count)
    {
        int shownSlots = 0;
        foreach (GameObject slot in slots)
        {
            if (shownSlots >= count)
            {
                break;
            }

            yield return new WaitForSeconds(delay);
            slot.SetActive(true);
            shownSlots++;

            // 슬롯의 RectTransform을 0에서 1로 Ease.OutBunce 효과로 키우는 애니메이션
            RectTransform rect = slot.GetComponent<RectTransform>();
            rect.localScale = Vector3.one * 0f;
            rect.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce);
        }
    }
}
