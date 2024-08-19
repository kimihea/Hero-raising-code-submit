using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFragment : MonoBehaviour
{
    // ������ ID
    public int HeroId { get; set; }

    // ���� ������ ���� ����
    public int Amount { get; private set; }

    // ������ ����� �� �߻��ϴ� �̺�Ʈ
    public Action OnChangedEvent;

    // ������ �����ϴ� �޼ҵ�
    public void ChangeAmount(int amount)
    {
        Amount = amount;
        OnChangedEvent?.Invoke();
    }
}
