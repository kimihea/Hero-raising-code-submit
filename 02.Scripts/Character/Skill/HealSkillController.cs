using System;
using UnityEngine;

public class HealSkillController : SkillObjectController
{
    Character character;
    Healer heal;
    float interval;
    public float period;
    public LineRenderer lineRenderer;
    protected  override void Awake()
    {
        base.Awake();
        character = GetComponentInParent<Character>();
        heal = GetComponentInParent<Healer>();
        
    }
    protected override void Start()
    {
        base.Start();
        AnimationCurve widthCurve = new AnimationCurve();
        widthCurve.AddKey(0.0f, 1.0f); // 시작 부분 (시간, 너비)
        widthCurve.AddKey(1.0f, 0.0f); // 끝 부분 (시간, 너비)
        lineRenderer.widthCurve = widthCurve;
    }
    protected override void ExecuteSkill()
    {
        interval = float.MinValue;
    }

    protected override void MoveSkill()
    {
        interval -= Time.deltaTime;
        if (interval <= period / 2)
            lineRenderer.SetPosition(1, Vector3.zero);
        if (interval <= 0f)
        {
            //주기가 되었으면 힐하고
            LineToFriendly(heal.HealMethod(character.CurAtk));
            interval = period;
        }
    }

    /// <summary>
    /// 라인의 index를 조절해서 줄을 만듭니다.
    /// </summary>
    /// <param name="DeadLinePos"></param>
    private void LineToFriendly(Vector3 DeadLinePos)
    {
        if(DeadLinePos != Vector3.zero)
        {
            Vector3 distance = DeadLinePos - transform.position;
            lineRenderer.SetPosition(1, distance);
        }
    }
}
