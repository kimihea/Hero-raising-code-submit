using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class AlertPanel: MonoBehaviour
{
    public GameObject showAlertObject;  // 알림을 표시할 오브젝트
    public Text alertText;  // 알림 텍스트
    public Image alertBG;
    public float fadeInDuration = 2.0f;  // 텍스트가 서서히 나타나는 데 걸리는 시간
    public float visibleDuration = 1.0f;  // 텍스트가 유지되는 시간
    public float fadeOutDuration = 1.0f;  // 텍스트가 서서히 사라지는 데 걸리는 시간
    private Coroutine alertCoroutine;
    Color newColor = Color.white;
    Color lack = new Color(1.0f, 0.4f, 0.7f); // 분홍색 (핑크) - (RGB: 255, 102, 178)
    Color NotImplemented = new Color(0.53f, 0.81f, 0.92f); // 하늘색 (스카이 블루) - (RGB: 135, 206, 235)
    Color Success= new Color(0.2f, 0.8f, 0.2f); // 초록색 (라임 그린) - (RGB: 51, 204, 51)
    Color Chapter = new Color(0.66f, 0.66f, 0.66f); // 회색 (그레이) - (RGB: 169, 169, 169)
    // ShowAlert 함수를 호출하여 알림을 표시
    public void ShowAlert(string message,EAlertType type)
    {
        switch (type)
        {
            case EAlertType.LACK:
                newColor = lack;
                AudioManager.Instance.PlaySFX("LACK");
                break;

            case EAlertType.NOTIMPLEMENTED:
                newColor = NotImplemented;
                AudioManager.Instance.PlaySFX("NOTIMP");

                break;

            case EAlertType.SUCCESS:
                newColor = Success;
                AudioManager.Instance.PlaySFX("SUCCESS");
                break;

            case EAlertType.CHAPTER:
                newColor = Chapter;
                break;

            default:
                Debug.LogWarning("Unknown alert type.");
                break;
        }
        alertBG.color = newColor;
        alertText.text = message;
        showAlertObject.SetActive(true);
        if (alertCoroutine != null) StopCoroutine(alertCoroutine);
        alertCoroutine = StartCoroutine(ShowAlertCoroutine());
        
        
    }

    // 알림을 서서히 나타내고 유지한 후 사라지게 하는 코루틴
    private IEnumerator ShowAlertCoroutine()
    {
        // 텍스트의 색상 정보 가져오기
        Color color = alertText.color;
        Color color1 = alertBG.color;
        // 텍스트를 서서히 나타나게 함
        for (float t = 0.0f; t < fadeInDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0, 1, t / fadeInDuration);
            color1.a= Mathf.Lerp(0, 1, t / fadeInDuration);
            alertText.color = color;
            alertBG.color = color1;
            yield return null;
        }
        // 텍스트를 완전히 선명하게 표시
        color.a = 1;
        color1.a = 1;
        alertText.color = color;
        alertBG.color = color1;

        yield return new WaitForSeconds(visibleDuration);

        // 텍스트를 서서히 사라지게 함
        for (float t = 0.0f; t < fadeOutDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1, 0, t / fadeOutDuration);
            color1.a = Mathf.Lerp(1, 0, t / fadeOutDuration);
            alertText.color = color;
            alertBG.color = color1;
            yield return null;
        }
        // 텍스트를 완전히 사라지게 한 후 오브젝트 비활성화
        color.a = 0;
        color1.a = 0;
        alertText.color = color;
        alertBG.color = color1;
        showAlertObject.SetActive(false);
    }
}
