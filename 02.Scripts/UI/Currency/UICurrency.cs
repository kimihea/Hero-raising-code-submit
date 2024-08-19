using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class UICurrency : MonoBehaviour
{
    [SerializeField] private ECurrencyType type;
    [SerializeField] private BigInteger amount;
    [SerializeField] private Text text;

    private void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        amount = CurrencyManager.Instance.GetCurrencyAmount(type);
        text.text = amount.ToAbbreviatedString();
    }
}

// BigInteger 값을 축약된 형식으로 표시하기 위한 확장 메서드
public static class BigIntegerExtensions
{
    public static string ToAbbreviatedString(this BigInteger value)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");

        var suffixes = new[] { "", "A", "B", "C", "D", "E", "F" };
        var threshold = 1_000;

        int suffixIndex = 0;
        decimal abbreviatedValue = (decimal)value;

        while (abbreviatedValue >= threshold && suffixIndex < suffixes.Length - 1)
        {
            abbreviatedValue /= threshold;
            suffixIndex++;
        }

        return suffixIndex == 0 ? value.ToString() : abbreviatedValue.ToString("0.0") + suffixes[suffixIndex];
    }
}
