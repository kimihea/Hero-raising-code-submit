using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterDamaged : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public Transform damageTextPos;
    private CharacterController _characterController;
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }
    private void OnEnable()
    {
        _characterController.OnDamage += GenerateText;
        _characterController.OnDamage += ChangeHealth;
        //임시
    }
    private void OnDisable()
    {
        _characterController.OnDamage -= GenerateText;
        _characterController.OnDamage -= ChangeHealth;
    }
    void ChangeHealth(int Damage)
    {
        if (_characterController.healthSystem.TakeDamage(Damage))//단순 수치 계산하고 만약 죽으면 True값 반환 
        {
            _characterController.isDead = true; 
            _characterController.CallDeath(); //OnDeath Action에 구독 되어 있는 함수들 실행
        }
    }
    private void GenerateText(int Damage)
    {
        ShowDamage(Damage, damageTextPos);
    }
    public void ShowDamage(int damage, Transform transform)
    {
        GameObject instance = Instantiate(damageTextPrefab, transform);
        instance.transform.SetParent(this.transform);
        instance.GetComponent<TextMeshPro>().text = damage.ToString();
        Destroy(instance, 0.2f);
        //풀링 시켜서 여러개 떠오르는 방식 추가예정
    }
}
