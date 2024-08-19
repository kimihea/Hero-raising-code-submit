using System.Collections;
using UnityEngine;

public class BodyEffect : MonoBehaviour
{
    Animator animator;
    public EffectAnimationData Data;
    WaitForSeconds SkillWait = new WaitForSeconds(1f);
    
    private void Awake()
    {
        Data = new EffectAnimationData();
        Data.Initialize();
        animator = GetComponent<Animator>();
    }
    #region Trigger형식으로 할 경우
    public void StartEffect(int animationHash)
    {
        animator.SetTrigger(animationHash);
    }

    #endregion
    #region 버프 코루틴 지속시간동안 계속 재생할경우
    //public void StartAnim(int animationHash)
    //{
    //    animator.SetBool(animationHash, true);
    //}
    //public void StopAnim(int animationHash)
    //{
    //    animator.SetBool(animationHash, false);
    //}
    ///// <summary>
    ///// 버프지속시간동안 실행
    ///// </summary>
    ///// <param name = "animationHash" ></ param >
    ///// < param name="buffTime"></param>
    ///// <returns></returns>
    //public IEnumerator EffectOn(int animationHash, float buffTime)
    //{
    //    StartAnim(animationHash);
    //    yield return new WaitForSeconds(buffTime);
    //    StopAnim(animationHash);
    //}
    ///// <summary>
    ///// 스킬사용할때 1초동안만 실행
    ///// </summary>
    ///// <param name="animationHash"></param>
    ///// <returns></returns>
    //public IEnumerator EffectOn(int animationHash)
    //{
    //    StartAnim(animationHash);
    //    yield return SkillWait;
    //    StopAnim(animationHash);
    //}
    #endregion
}
