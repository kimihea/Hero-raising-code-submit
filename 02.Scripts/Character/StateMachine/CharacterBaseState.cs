using UnityEditor;
using UnityEngine;
using static StatManager;

public class CharacterBaseState : IState
{
    protected CharacterStateMachine stateMachine;
    protected GameManager gameManager;
    protected AnimatorStateInfo animatorStateInfo;
    float currentMotionTime;
    //protected AnimatorControllerParameter curMotionTime;
    protected CharacterBaseState(CharacterStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        gameManager = GameManager.Instance;
        //curMotionTime = stateMachine.Character.Animator.GetParameter(stateMachine.Character.DataAnim.CurMotionTimeParameterHash);
    }
    public virtual void Enter()
    {
        if(stateMachine.currentState !=stateMachine.previousState)
            stateMachine.Character.Animator.SetFloat(stateMachine.Character.DataAnim.CurMotionTimeParameterHash, 0);
        //Debug.Log(stateMachine.currentState != stateMachine.previousState);
        //Debug.Log(stateMachine.currentState+","+stateMachine.previousState);
    }

    public virtual void Exit()
    {
        
    }
    public virtual void Update()
    {
        animatorStateInfo = stateMachine.Character.Animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("Hurt")) return;
        currentMotionTime += Time.deltaTime;
        stateMachine.Character.Animator.SetFloat(stateMachine.Character.DataAnim.CurMotionTimeParameterHash, currentMotionTime);
    }


    protected void StartAnimation(int animationHash)
    {
       stateMachine.Character.Animator.SetBool(animationHash, true);
    }
    protected void StopAnimation(int animationHash)
    {
        stateMachine.Character.Animator.SetBool(animationHash, false);
    }
}
