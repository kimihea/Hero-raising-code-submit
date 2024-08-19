using System.Collections;
using UnityEngine;

public class CharacterHealed : MonoBehaviour
{
    private CharacterController _characterController;
    WaitForSeconds waitHeal = new WaitForSeconds(0.12f);//힐이펙트0.12초
    public Transform body;
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }
    private void OnEnable()
    {
        _characterController.OnHeal += HealEffect;
        _characterController.OnHeal += ChangeHealth;
        //임시
    }
    private void OnDisable()
    {
        _characterController.OnHeal -= HealEffect;
        _characterController.OnHeal -= ChangeHealth;
    }
    void ChangeHealth(int amount)
    {
        if(_characterController.healthSystem.TakeHeal(amount))
        {
            //힐이펙트적용
            //Debug.Log("힐적용");
            //Debug.Log(amount + "치료");
        }
    }
    void HealEffect(int amonut)
    {
        StartCoroutine(EHealEffect());
    }
    IEnumerator EHealEffect()
    {
        GameObject obj = PoolManager.Instance.SpawnFromPool("FX00000");
        obj.transform.position = body.position + Vector3.up*0.5f;
        yield return waitHeal;
        obj.SetActive(false);
    }
}
