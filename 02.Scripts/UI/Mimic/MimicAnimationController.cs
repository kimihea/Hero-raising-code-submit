using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicAnimationController : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnHitButtonPressed()
    {
        if ( StatManager.Instance.equipment.isGachaPossible)
        {
            animator.SetTrigger("TriggerHit");
        }  
    }
}
