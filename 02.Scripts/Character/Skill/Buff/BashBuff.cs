using UnityEngine;
using UnityEngine.UIElements;
public class BashBuff : BuffSkillController
{
    public Transform SpawnPos;
    public GameObject AnimObj;
    public Character character;

    public override void SetBuffStat()
    {
        BuffStat.AtkMultiplier = skill.DamagePerGradge();
    }
    protected override void ExecuteSkill()
    {
        if(character.Target != null) 
        {
              if (character.Target.position.x < character.transform.position.x)
              {
                  SpawnPos.localPosition = Vector3.left; 
                  AnimObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180f));
              }
              else
              {
                  SpawnPos.localPosition = Vector3.right;
                AnimObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
              }
        }
        AnimObj.transform.position = SpawnPos.position;
        IHandleBuff buff = GetComponentInParent<IHandleBuff>();
        buff?.ActiveBuff(BuffStat, skill.Duration, Type); 
    }
}
