using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency
{
    public int Amount { get; private set; }
    public event Action OnChangedEvent;

    public Currency(int initialAmount = 0)
    {
        Amount = initialAmount;
    }

    public void ChangeAmount(int amount)
    {
        Amount = amount;
        OnChangedEvent?.Invoke();
    }

    public bool TrySpend(int amount)
    {
        if (Amount >= amount)
        {
            ChangeAmount(Amount - amount);
            return true;
        }
        return false;
    }

    public void Add(int amount)
    {
        ChangeAmount(Amount + amount);
    }
}
