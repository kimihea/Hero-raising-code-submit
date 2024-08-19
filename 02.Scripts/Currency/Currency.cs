using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Currency
{
    public BigInteger Amount { get; private set; }
    public event Action OnChangedEvent;

    public Currency(BigInteger initialAmount = default)
    {
        Amount = initialAmount;
    }

    public void ChangeAmount(BigInteger amount)
    {
        Amount = amount;
        OnChangedEvent?.Invoke();
    }

    public bool TrySpend(BigInteger amount)
    {
        if (Amount >= amount)
        {
            ChangeAmount(Amount - amount);
            return true;
        }
        return false;
    }

    public void Add(BigInteger amount)
    {
        ChangeAmount(Amount + amount);
    }
}
