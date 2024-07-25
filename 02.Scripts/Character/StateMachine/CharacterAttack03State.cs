
public class CharacterAttack03State : CharacterCombatState
{
    public CharacterAttack03State(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        StartAnimation(stateMachine.Character.DataAnim.Attack03ParameterHash);
        base.Enter();

    }
    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        StopAnimation(stateMachine.Character.DataAnim.Attack03ParameterHash);
        base.Exit();
    }

}