
public class CharacterAttack01State : CharacterCombatState
{
    public CharacterAttack01State(CharacterStateMachine stateMachine) : base(stateMachine)
    {
        
    }
    public override void Enter()
    {
        StartAnimation(stateMachine.Character.DataAnim.Attack01ParameterHash);
        base.Enter();

    }
    public override void Update()
    {

        stateMachine.Character.Controller.CallAttack();

        base.Update();
    }
    public override void Exit()
    {
        StopAnimation(stateMachine.Character.DataAnim.Attack01ParameterHash);
        base.Exit();
    }
    
}
