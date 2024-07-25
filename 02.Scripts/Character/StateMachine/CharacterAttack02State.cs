
public class CharacterAttack02State : CharacterCombatState
{ 
    public CharacterAttack02State(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        StartAnimation(stateMachine.Character.DataAnim.Attack02ParameterHash);
        base.Enter();

    }
    public override void Update()
    {

        stateMachine.Character.Controller.CallAttack();
        base.Update();
    }
    public override void Exit()
    {
        StopAnimation(stateMachine.Character.DataAnim.Attack02ParameterHash);
        base.Exit();
    }

}
