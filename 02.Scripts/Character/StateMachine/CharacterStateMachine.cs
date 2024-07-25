using System;
using UnityEngine.PlayerLoop;

public class CharacterStateMachine : StateMachine
{
    public Character Character;
    public CharacterIdleState Idle;
    public CharacterDeathState Death;

    public CharacterPursuitState Pursuit;
    public CharacterAttack01State Attack01;
    public CharacterAttack02State Attack02;
    public CharacterAttack03State Attack03;


    public CharacterStateMachine(Character Character)
    {
        this.Character = Character;
        Idle = new CharacterIdleState(this);
        Pursuit = new CharacterPursuitState(this);
        Attack01 = new CharacterAttack01State(this);
        Attack02 = new CharacterAttack02State(this);
        Attack03 = new CharacterAttack03State(this);
        Death = new CharacterDeathState(this);
    }
    
    public void Initialize()
    {
        currentState = Idle;
    }
}
