using System;
using UnityEngine;

[Serializable]
public class CharacterAnimationData
{
    [Header("Works Anywhere, Trigger")]
    [SerializeField] private string hurtParameterName = "Hurt";
    [SerializeField] private string deathParameterName = "Death";
        
    [Header("Sub-StateMachine")]
    [SerializeField] private string combatParameterName = "Combat";


    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string walkParameterName = "Walk";
    [SerializeField] private string attack01ParameterName = "Attack01";
    [SerializeField] private string attack02ParameterName = "Attack02";
    [SerializeField] private string attack03ParameterName = "Attack03";

    [SerializeField] private string CurMotionTimeParameterName = "CurMotionTime";


    public int IdleParameterHash { get; private set; }
    public int WalkParameterHash { get; private set; }


    public int Attack01ParameterHash { get; private set; }
    public int Attack02ParameterHash { get; private set; }
    public int Attack03ParameterHash { get; private set; }

    public int DeathParameterHash { get; private set; }
    public int HurtParameterHash { get; private set; }
    public int CombatParameterHash { get; private set; }
    public int CurMotionTimeParameterHash { get; private set; }

    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        WalkParameterHash = Animator.StringToHash(walkParameterName);

        Attack01ParameterHash = Animator.StringToHash(attack01ParameterName);
        Attack02ParameterHash = Animator.StringToHash(attack02ParameterName);
        Attack03ParameterHash = Animator.StringToHash(attack03ParameterName);

        DeathParameterHash = Animator.StringToHash(deathParameterName);
        HurtParameterHash = Animator.StringToHash(hurtParameterName);
        CombatParameterHash = Animator.StringToHash(combatParameterName);
        CurMotionTimeParameterHash = Animator.StringToHash(CurMotionTimeParameterName);
    }
}