using UnityEngine;

public class CharacterIdleState : CharacterBaseState 
{
    public CharacterIdleState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        StartAnimation(stateMachine.Character.DataAnim.IdleParameterHash);
        base.Enter();        
    }
    public override void Update()
    {
        base.Update();
        switch (GameManager.Instance.CombatConditionType)
        {
            case ECombatConditionType.START:
                stateMachine.ChangeState(stateMachine.Pursuit);
                break;
            case ECombatConditionType.END:
                if (GameManager.Instance.CheckHeroReady())
                {
                    GameManager.Instance.Stage.MoveMap();
                    //stateMachine.ChangeState(stateMachine.Pursuit);
                }
                break;
            case ECombatConditionType.READY:
                if(stateMachine.Character is Monster)
                {
                    stateMachine.ChangeState(stateMachine.Pursuit);
                }
                break;
        }
        //if(stateMachine.Character.Target != null || stateMachine.Character.transform.position != stateMachine.Character.DefalutPos)
        //{
        //    stateMachine.ChangeState(stateMachine.Pursuit);
        //}       
    }
    public override void Exit() 
    { 
        base.Exit();
        StopAnimation(stateMachine.Character.DataAnim.IdleParameterHash);
    }

}
