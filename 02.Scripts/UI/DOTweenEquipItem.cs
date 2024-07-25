using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DOTweenEquipItem : MonoBehaviour
{
    public RectTransform Mimic;         // 시작 지점
    public RectTransform NewEquip;      // 도착 지점
    public RectTransform CurrentEquip;  //     "

    void Start()
    {
        // Mimic 위치에서 NewEquip 위치로 이동하는 애니메이션
        Mimic.DOPath(new Vector3[] { Mimic.position, NewEquip.position }, 2f, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // 크기 변경 애니메이션
                Mimic.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1f).SetEase(Ease.InOutQuad);
            });

        // Mimic 위치에서 CurrentEquip 위치로 이동하는 애니매이션
        Mimic.DOPath(new Vector3[] { Mimic.position, CurrentEquip.position }, 2f, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad)
            .SetDelay(2.5f) // 첫 번째 애니메이션이 끝난 후 실행되도록 딜레이 설정
            .OnComplete(() =>
            {
                // 크기 변경 애니메이션
                Mimic.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1f).SetEase(Ease.InOutQuad);
            });
    }
}
