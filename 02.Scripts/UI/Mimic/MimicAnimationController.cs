using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MimicAnimationController : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // 특정 트리거 이름을 설정할 수 있는 메서드
    public void SetTrigger(int index)
    {
        if (animator != null)
        {
            if (!Equipment.isGachaPossible) return;

            // ManaStone의 0개인지 확인
            BigInteger manaStoneAmount = CurrencyManager.Instance.GetCurrencyAmount(ECurrencyType.ManaStone);
            if (manaStoneAmount <= 0)
            {
                //Debug.Log("ManaStone이 부족하여 Trigger가 발동되지 않습니다.");
                return;
            }

            string triggerName = $"Trigger{index}";
            animator.SetTrigger(triggerName);
        }
    }
}