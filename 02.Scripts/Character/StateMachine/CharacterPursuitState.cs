using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
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
    //private Transform goBack;
    public CharacterPursuitState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
        speedModifier = stateMachine.Character.StatHandler.baseStat.MoveSpeed;
        range = stateMachine.Character.StatHandler.baseStat.AttackRange;
        characterTransform = stateMachine.Character.transform;
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
    public void MoveTowardsTarget(Vector3 target)
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
        stateMachine.Character.Controller.FlipCharacter(target);   
    }
    private void MoveTowardsDefalutPos(Vector3 target)
    {
        //GameManager.Instance.Stage.MoveMap();
        curTime += Time.deltaTime;
        characterTransform.position = Vector2.Lerp(startPos, target, curTime / 2f);
    }
    /// <summary>
    /// 새 어그로 탐색 범위는 AttackRange의 3배이다.
    /// </summary>
    private void NewAgrro()
    {
        StageTarget();
        if (GameManager.Instance.battleType == EBattleType.GOLDDUNGEON && stateMachine.Character.EntityType ==EEntityType.PLAYER)
        {
            DungeonTarget();
        }
    }
    private void DungeonTarget()
    {
        if (stateMachine.Character.Target==null) return;
        float MobToBaseDis = (stateMachine.Character.Target.position - GameManager.Instance.GoldDungeon.BasePos.position).magnitude;
        if (MobToBaseDis > 2.0f)
        {
            GameManager.Instance.GoldDungeon.CheckBase(2f, out List<Transform> Target);
            //Base근처에 몬스터들이 있는지 체크
            if (Target.Count>0)
            {
                Transform closestEnemy = null;
                float closestDistance = float.MaxValue;
                foreach (Transform t in Target)
                {
                    float distance = Vector2.Distance(t.position, stateMachine.Character.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = t.transform;
                    }
                }
                stateMachine.Character.Target = closestEnemy;
            }
        }
    }
    private void StageTarget()
    {
        Collider2D colider2D = Physics2D.OverlapCircle(characterTransform.position, range * 3, stateMachine.Character.LayerMask.value);
        if (colider2D != null)
        {
            if (colider2D.GetComponent<Character>().EntityType == stateMachine.Character.TargetType)
            {
                if (stateMachine.Character.Target == null || colider2D == null) return;

                float curTargetToCharacterDist = (characterTransform.position - stateMachine.Character.Target.position).magnitude;
                float newTargetToCharacterDist = (characterTransform.position - colider2D.gameObject.transform.position).magnitude;
                if (curTargetToCharacterDist > newTargetToCharacterDist)  //기존 타겟의 거리가 새로 잡힌 타겟보다 멀다면
                {
                    stateMachine.Character.Target = colider2D.gameObject.transform; //타겟을 바꾸는데
                    float playerToNewTargetDist = (GameManager.Instance.player.transform.position - stateMachine.Character.Target.position).magnitude;
                    if (playerToNewTargetDist > 4f && stateMachine.Character.EntityType != EEntityType.MONSTER)//현재 카메라의 horizontal length는 9
                    {//새로운 타겟과 플레이어와의 거리가 너무 멀다면 타겟을 플레이어와 링크한다. 즉 플레이어와 거리가 4f이하인 적들만 타겟으로 설정한다.
                        stateMachine.Character.Target = GameManager.Instance.player.Target;
                    }
                }

            }
        }
    }
}
