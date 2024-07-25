using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    [field: SerializeField] public BaseStat Stat { get; private set; }
    //List<Hero> heroList = new List<Hero> ();

    protected override void Awake()
    {
        base.Awake();    
    }
    protected override void Start()
    {
        base.Start();
    }
    //public void InitStat()
    //{
    //    // TODO : Stat ����
    //    //
    //    Health.InitHealth(StatHandler.curStat.Health);
    //}

    public override void FindTarget()
    {
        targetList.Clear();
        foreach (Character hero in GameManager.Instance.EntryList)
        {
            if(hero != null&&!hero.Controller.isDead)
            {
                targetList.Add(hero);
            }
        }
        SetTarget();
    }
    public override void SetTarget()
    {
        if(targetList.Count>0)
        {
            int randomIndex = UnityEngine.Random.Range(0, targetList.Count);
            Target = targetList[randomIndex].transform;
        }
        else
        {
            Target = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GameController"))
        {
            if (GameManager.Instance.CombatConditionType == ECombatConditionType.READY)
                GameManager.Instance.CombatConditionType = ECombatConditionType.START;
        }
    }
}