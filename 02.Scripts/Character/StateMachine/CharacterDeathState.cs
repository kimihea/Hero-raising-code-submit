public class CharacterDeathState : CharacterBaseState
{
    public CharacterDeathState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        StartAnimation(stateMachine.Character.DataAnim.DeathParameterHash);
        base.Enter();
    }
    public override void Update()
    {
        base.Update();

    }
    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Character.DataAnim.DeathParameterHash);
    }
}
