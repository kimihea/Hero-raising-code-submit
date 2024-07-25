using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICurrency : MonoBehaviour
{
    [SerializeField] private ECurrencyType type;
    [SerializeField] private int amount;
    [SerializeField] private Text text;

    private void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        amount = CurrencyManager.Instance.GetCurrencyAmount(type);
        text.text = amount.ToString();
    }
}
