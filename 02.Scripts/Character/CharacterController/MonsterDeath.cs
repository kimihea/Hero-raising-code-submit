using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDeath : MonoBehaviour
{
    private CharacterController _characterController;
    private void OnEnable()
    {
        _characterController = GetComponent<CharacterController>();
        _characterController.OnDeath += Death;
    }
    private void OnDisable()
    {
        _characterController.OnDeath -= Death;

    }
    void Death()
    {
        GameManager.Instance.MonsterDeath((Monster)_characterController.character);
        //CurrencyManager.Instance.AddCurrency(ECurrencyType.ManaStoneFragment, amount);
    }
}
