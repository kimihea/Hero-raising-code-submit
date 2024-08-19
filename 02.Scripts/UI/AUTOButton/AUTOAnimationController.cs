using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AUTOAnimationController : MonoBehaviour
{
    public Button autoButton;
    public Animator animator;
    private bool isPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        autoButton.onClick.AddListener(ToggleAnimation);
    }

    void ToggleAnimation()
    {
        if (isPlaying)
        {
            animator.SetTrigger("Idle");
            isPlaying = false;
        }
        else
        {
            animator.SetTrigger("Play");
            isPlaying = true;
        }
    }
}
