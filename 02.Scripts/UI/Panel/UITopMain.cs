using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITopMain : MonoBehaviour
{
    private void Start()
    {
        // Rect stretch 수직 수평 둘다
        RectTransform rect = GetComponent<RectTransform>();
        Rect safeArea = Screen.safeArea;

        // SafeArea Vector2 범위 Min(하단) Max(상단)
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        // 수평
        anchorMin.x = rect.anchorMin.x;
        anchorMax.x = rect.anchorMax.x;

        // 수직 > SafeArea / 스크린 높이 앵커 비율 설정
        anchorMin.y /= Screen.height;
        anchorMax.y /= Screen.height;

        // 앵커에 위에 계산한 Min, Max 비율 대입
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
    }
}
