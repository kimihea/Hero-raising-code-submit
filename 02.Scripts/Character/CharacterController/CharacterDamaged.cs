using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class CharacterDamaged : MonoBehaviour
{
    public GameObject damageTextPrefab;//<-poolmanager사용하면서 사용안함.
    public Transform damageTextPos;
    private CharacterController _characterController;
    private float yOffset = 0.2f; // 고정된 Y축 오프셋
    private Queue<BigInteger> damageQueue = new Queue<BigInteger>(); // 데미지 큐
    private bool isProcessing = false; // 현재 데미지 텍스트 처리 중인지 여부
    private Coroutine damageCoroutine; // 데미지 코루틴
    private List<GameObject> activeDamageTexts = new List<GameObject>(); // 활성화된 데미지 텍스트 리스트
    public float CurrentYOffset;
    public bool Enemy;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        _characterController.OnDamage += EnqueueDamage;
        _characterController.OnDamage += ChangeHealth;
        _characterController.OnDeath += ResetDamageTexts; // 몬스터가 죽었을 때 초기화
    }

    private void OnDisable()
    {
        _characterController.OnDamage -= EnqueueDamage;
        _characterController.OnDamage -= ChangeHealth;
        _characterController.OnDeath -= ResetDamageTexts;
        ResetLogic();
    }

    void ChangeHealth(int damage, bool isCritic)
    {
        BigInteger bigDamage = new BigInteger(damage);  // int를 BigInteger로 변환
        if (_characterController.healthSystem.TakeDamage(damage))
        {
            _characterController.isDead = true;
            _characterController.CallDeath();
        }
    }

    private void EnqueueDamage(int damage,bool isCritic)
    {
        BigInteger bigDamage = new BigInteger(damage);  // int를 BigInteger로 변환
        damageQueue.Enqueue(damage);
            damageCoroutine = StartCoroutine(ProcessDamageQueue(isCritic));
        if (!isProcessing)
        {
        }
    }

    private IEnumerator ProcessDamageQueue(bool isCritic)
    {
        isProcessing = true;
        float currentYOffset = CurrentYOffset!=0 ? CurrentYOffset: 0f;

        while (damageQueue.Count > 0)
        {
            BigInteger damage = damageQueue.Dequeue();
            ShowDamage(damage, damageTextPos, currentYOffset,isCritic);
            currentYOffset += yOffset; // 각 텍스트의 Y 오프셋을 증가시킴
            yield return new WaitForSeconds(0.5f); // 텍스트가 순차적으로 나타나도록 대기
        }

        isProcessing = false;
    }

    public void ShowDamage(BigInteger damage, Transform transform, float additionalYOffset,bool isCritic)
    {
        GameObject instance = PoolManager.Instance.SpawnFromPool("DMT00001");
        if (instance != null)
        {
            //instance.transform.SetParent(this.transform);
            instance.transform.position = transform.position + GetInitialOffset(additionalYOffset);
            TextMeshPro textMesh = instance.GetComponent<TextMeshPro>();
            textMesh.text = damage.ToAbbreviatedString(); // 축약된 형식으로 표시

            textMesh.color = isCritic ? Color.yellow : Color.white;
            if (!Enemy) textMesh.color = Color.red;
            activeDamageTexts.Add(instance); // 활성화된 텍스트 리스트에 추가
            StartCoroutine(MoveTextUpwards(instance));
            StartCoroutine(FadeOutText(instance, textMesh, 1f)); // 페이드아웃 코루틴 호출
            StartCoroutine(DeactivateAfterDelay(instance, 1f));
        }
    }

    private IEnumerator DeactivateAfterDelay(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }

    private IEnumerator MoveTextUpwards(GameObject instance)
    {
        float elapsedTime = 0f;
        float moveDuration = 0.5f;
        UnityEngine.Vector3 initialPosition = instance.transform.position;

        while (elapsedTime < moveDuration)
        {
            instance.transform.position = initialPosition + new UnityEngine.Vector3(0, elapsedTime, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // 페이드아웃을 위한 코루틴
    private IEnumerator FadeOutText(GameObject instance, TextMeshPro textMesh, float duration)
    {
        float elapsedTime = 0f;
        Color initialColor = textMesh.color;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            textMesh.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textMesh.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
    }

    // 고정된 Y축 오프셋을 설정하여 텍스트가 겹치지 않게 함
    private UnityEngine.Vector3 GetInitialOffset(float additionalYOffset)
    {
        return new  UnityEngine.Vector3(0, additionalYOffset, 0);
    }

    // 몬스터가 죽었을 때 데미지 텍스트 초기화
    private void ResetDamageTexts()
    {
        StartCoroutine(ResetDamageTextsCoroutine());
    }
    private IEnumerator ResetDamageTextsCoroutine()
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 대기, 사망후 0.8초후에 게임오브젝트 비활성화함.
        ResetLogic();
    }
    private void ResetLogic()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null; // 현재 실행 중인 코루틴을 초기화
        }
        isProcessing = false;
        damageQueue.Clear(); // 남은 데미지 큐를 초기화

        foreach (var text in activeDamageTexts)
        {
            if (text != null)
            {
                text.SetActive(false); // 활성화된 텍스트 비활성화
            }
        }
        activeDamageTexts.Clear(); // 활성화된 텍스트 리스트 초기화
    }
}