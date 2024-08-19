using System;
using UnityEngine;
using UnityEngine.UI;
using static StatManager;

public class HealthSystem : MonoBehaviour
{
    Character character;
    public Image HpBar;
    [field: SerializeField]public float CurHealth { get; private set; }
    [field: SerializeField]public float MaxHealth { get; private set; }
    private float preMaxHealth;
    //public StatHandler statHandler;

    //public void Start()
    //{
    //    statHandler.OnStatChanged += DetectChangeMaxHealth;
    //}
    //public void OnDisable()
    //{
    //    statHandler.OnStatChanged -= DetectChangeMaxHealth;
    //}

    void Awake()
    {
        character=GetComponent<Character>();
    }
    public void Update()
    {
        
        DetectChangeMaxHealth(character.StatHandler.curStat.GetCurHealth());
        ShowCurrentHpRate();
    }

    private void ShowCurrentHpRate()
    {
        HpBar.fillAmount =GetCurrentHpRate();
    }

    public void InitHealth(float health)
    {
        MaxHealth = health;
        CurHealth = health;
        preMaxHealth= health;
    }
    public void DetectChangeMaxHealth(float curHealthStat)
    {
        if (preMaxHealth == curHealthStat)
        {
            return;
        }
        float difference = curHealthStat - preMaxHealth; 
        //실행되기전에 최대체력이 변했는지 검사
        if (preMaxHealth < curHealthStat) { 
            MaxHealth += difference;
            CurHealth += difference;
        }
        else 
        {
            MaxHealth += difference;
            {
            // 현재 체력은 변동 없음
            // 감소한 최대 체력이 현재 체력보다 낮으면 현재 체력 보정,비율로 계산을 하면은 체력이 높아진 상황에서 회복될경우 디메리트가 된다.
            if (CurHealth > MaxHealth)
                CurHealth = MaxHealth;
            }
        }
        preMaxHealth = curHealthStat;
    }

    public bool TakeDamage(int damage, bool isCrit = false)
    {
        BaseStat stat = character.StatHandler.curStat;
        CurHealth = Mathf.Clamp(CurHealth - damage , 0, CurHealth); 
        if (CurHealth == 0) return true;
        return false;
    }

    public bool TakeHeal(float amount)
    {
        if(CurHealth == MaxHealth) return false;
        CurHealth = Mathf.Min(CurHealth + amount, MaxHealth);
        return true;
    }
    public float GetCurrentHpRate()
    {
        return CurHealth / MaxHealth;
    }
}
