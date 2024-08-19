using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    List<Character> heroList;
    Hero healer;
    public float HealPeriod;
    public float TimeSinceLastHeal;
    float lowestHealthPercentage;
    float healthPercentage;
    Character lowestHealthHero;
    private void Start()
    {
        heroList = GameManager.Instance.EntryList;
        TimeSinceLastHeal = float.MinValue;
        healer= GetComponent<Hero>();
    }
    private void Update()
    {
        //heroList가 변경되면 List최신화
        //TimeSinceLastHeal -= Time.deltaTime;
        //if(TimeSinceLastHeal <= 0f)
        //{
        //    HealMethod(healer.CurAtk);
        //}
    }
    /// <summary>
    /// 체력비율이 가장 낮은 Hero를 탐색하여,한번 회복시킵니다.회복시킨 아군의 Vector3를 반환
    /// </summary>
    public Vector3 HealMethod(int value)
    {
        lowestHealthPercentage = float.MaxValue;
        //체력비율이 적은 아군 탐색
        foreach (Character character in heroList)
        {
            if (!character.Controller.isDead)
            {
                healthPercentage = character.Health.GetCurrentHpRate();
                if (healthPercentage < lowestHealthPercentage)
                {
                    lowestHealthPercentage = healthPercentage;
                    lowestHealthHero = character;
                }
            }    
        }
        //그새 안 죽었고 풀피가 아니면
        if (!lowestHealthHero.Controller.isDead && lowestHealthPercentage != 1f)
        {

            lowestHealthHero.Controller.CallHeal(value);
            TimeSinceLastHeal = HealPeriod;
            return lowestHealthHero.transform.position;
        }
        else return Vector3.zero;
    }
}
