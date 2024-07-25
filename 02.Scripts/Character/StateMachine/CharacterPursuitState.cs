using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
//using static UnityEngine.RuleTile.TilingRuleOutput;

public class CharacterPursuitState : CharacterBaseState
{
    private float speedModifier;
    private float range;
    private float curTime = -1f;
    private Transform characterTransform;
    private Vector3 startPos;
    private Vector3 currentPosition;
    private SpriteRenderer spriteRenderer;
    //private Transform goBack;
    public CharacterPursuitState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
        speedModifier = stateMachine.Character.StatHandler.baseStat.MoveSpeed;
        range = stateMachine.Character.StatHandler.baseStat.AttackRange;
        characterTransform = stateMachine.Character.transform;
        spriteRenderer = stateMachine.Character.GetComponentInChildren<SpriteRenderer>();
    }

    public override void Enter()
    {
        StartAnimation(stateMachine.Character.DataAnim.WalkParameterHash);
        curTime = -1f;
        base.Enter();
    }
    public override void Update()
    {
        base.Update();
        NewAgrro();
        switch (GameManager.Instance.CombatConditionType)
        {
            case ECombatConditionType.START:
                if (stateMachine.Character.Target != null)
                {
                    MoveTowardsTarget(stateMachine.Character.Target.position);

                    FlipCharacter(stateMachine.Character.Target.position);
                    if (DistanceToTarget(stateMachine.Character.Target) < range)
                    {
                        stateMachine.ChangeState(stateMachine.Attack01);
                    }
                }
                break;            
            case ECombatConditionType.END:                
                stateMachine.ChangeState(stateMachine.Idle);            
                break;
            case ECombatConditionType.READY:
                if (stateMachine.Character is Monster)
                {
                    if (stateMachine.Character.Target != null)
                    {
                        MoveTowardsTarget(stateMachine.Character.Target.position);

                        FlipCharacter(stateMachine.Character.Target.position);
                        if (DistanceToTarget(stateMachine.Character.Target) < range)
                        {
                            stateMachine.ChangeState(stateMachine.Attack01);
                        }
                    }
                }
                else
                {
                    if (stateMachine.Character.EntityType == EEntityType.MONSTER) return;
                    if (curTime < 0f)
                    {
                        curTime = 0f;
                        startPos = characterTransform.position;
                    }
                    MoveTowardsDefalutPos(stateMachine.Character.DefalutPos);
                    if (characterTransform.position == stateMachine.Character.DefalutPos)
                    {
                        //GameManager.Instance.ReadyCount--;
                        curTime = -1f;
                        stateMachine.ChangeState(stateMachine.Idle);
                    }
                }
                break;
        }
        
        //if (stateMachine.Character.Target != null)
        //{
        //    MoveTowardsTarget(stateMachine.Character.Target.position);
             
        //    FlipCharacter(stateMachine.Character.Target.position);
        //    if (DistanceToTarget(stateMachine.Character.Target) < range)
        //    {
        //        stateMachine.ChangeState(stateMachine.Attack01);
        //    }
        //}
        //else
        //{
        //    if (stateMachine.Character.EntityType == EEntityType.MONSTER) return;
        //    if (curTime < 0f)
        //    {
        //        curTime = 0f;
        //        startPos = characterTransform.position;
        //    }            
        //    MoveTowardsDefalutPos(stateMachine.Character.DefalutPos);
        //    if (characterTransform.position == stateMachine.Character.DefalutPos)
        //    {
        //        GameManager.Instance.ReadyCount++;
        //        curTime = -1f;
        //        stateMachine.ChangeState(stateMachine.Idle);
        //    }
        //}
    }
    public float DistanceToTarget(Transform Target)
    {
        return (characterTransform.position - Target.position).magnitude;
    }
    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Character.DataAnim.WalkParameterHash);
    }
    private void MoveTowardsTarget(Vector3 target)
    {
        if (Mathf.Abs(characterTransform.position.x - target.x) <= 0.7f)
        {
            currentPosition.x = characterTransform.position.x;
            currentPosition.y = Vector2.MoveTowards(characterTransform.position, target, speedModifier * Time.deltaTime).y;
            characterTransform.position = new Vector2(currentPosition.x, currentPosition.y);
        }
        else
        {
            characterTransform.position = Vector2.MoveTowards(characterTransform.position, target, speedModifier * Time.deltaTime);
            currentPosition = characterTransform.position;  
        }

    }
    private void FlipCharacter(Vector3 target)
    {
        if (characterTransform.position.x <= target.x) spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;
    }
    private void MoveTowardsDefalutPos(Vector3 target)
    {
        //GameManager.Instance.Stage.MoveMap();
        curTime += Time.deltaTime;
        characterTransform.position = Vector2.Lerp(startPos, target, curTime / 2f);
    }
    private void NewAgrro()
    {
        Collider2D colider2D = Physics2D.OverlapCircle(characterTransform.position, range * 3,stateMachine.Character.LayerMask.value);
        if(colider2D != null)
        {
            if (colider2D.GetComponent<Character>().EntityType == stateMachine.Character.TargetType)
            {
                if (stateMachine.Character.Target == null || colider2D == null) return;
                if ((characterTransform.position - stateMachine.Character.Target.position).magnitude > (characterTransform.position - colider2D.gameObject.transform.position).magnitude)
                { stateMachine.Character.Target = colider2D.gameObject.transform; }
            }
        }
        


    }
}
