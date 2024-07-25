using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatState : CharacterBaseState
{
    protected float range;
    protected Transform characterTransform;


    public CharacterCombatState(CharacterStateMachine stateMachine)  : base(stateMachine)
    {
        characterTransform = stateMachine.Character.transform;
    }

    public override void Enter()
    {
        StartAnimation(stateMachine.Character.DataAnim.CombatParameterHash);
        base.Enter();
        
    }
    public override void Update()
    {
        if (stateMachine.Character.Controller.isChanneling == false)
        {
            if (stateMachine.Character.Target == null) stateMachine.ChangeState(stateMachine.Pursuit);
            else
            {
                if ((characterTransform.position - stateMachine.Character.Target.position).magnitude > stateMachine.Character.StatHandler.curStat.AttackRange)
                {
                    stateMachine.ChangeState(stateMachine.Pursuit);
                }
            }
        }
        
        
        base.Update();
    }
    public override void Exit()
    {
        StopAnimation(stateMachine.Character.DataAnim.CombatParameterHash);

        base.Exit();
    }

}