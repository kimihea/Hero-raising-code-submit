using System;
using UnityEngine;
using static StatManager;

public class HealthSystem : MonoBehaviour
{
    [field: SerializeField]public float CurHealth { get; private set; }
    [field: SerializeField]public float MaxHealth { get; private set; }

    public event Action OnChangeHealthEvent;

    private StatHandler statHandler;

    private void Awake()
    {
        statHandler = GetComponent<StatHandler>();
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        //if curstate is updated, ChanageMaxHealth();
    }
    private void OnEnable()
    {
        
    }

    public void InitHealth(float health)
    {
        MaxHealth = health;
        CurHealth = health;
    }
    public void ChangeMaxHealth(float amount)
    {
        MaxHealth += amount;
        if (amount > 0) // 최대 체력 증가 시
        {
            CurHealth += amount;
        }
        else // 최대 체력 감소시 
        {
            // 현재 체력은 변동 없음
            // 감소한 최대 체력이 현재 체력보다 낮으면 현재 체력 보정
            if (CurHealth > MaxHealth)
            {
                CurHealth = MaxHealth;
            }
        }

        OnChangeHealthEvent?.Invoke();
    }

    public bool TakeDamage(int damage, bool isCrit = false)
    {
        CurHealth = Mathf.Max(CurHealth - damage, 0);
        if (CurHealth == 0) return true;
        return false;
    }

    //public void Heal(float amount)
    //{
    //    CurHealth = Mathf.Min(CurHealth + amount, MaxHealth);
    //    OnHealEvent?.Invoke();
    //}
}
