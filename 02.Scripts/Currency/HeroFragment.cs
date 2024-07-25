using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFragment : MonoBehaviour
{
    // 영웅의 ID
    public int HeroId { get; set; }

    // 영웅 조각의 현재 수량
    public int Amount { get; private set; }

    // 수량이 변경될 때 발생하는 이벤트
    public Action OnChangedEvent;

    // 수량을 변경하는 메소드
    public void ChangeAmount(int amount)
    {
        Amount = amount;
        OnChangedEvent?.Invoke();
    }
}
