using UnityEngine.TextCore.Text;
using UnityEngine;
using System.Collections;

public class BuffSkillController : SkillObjectController
{
    public CharacterStat BuffStat;
    protected override void ExecuteSkill()
    {
        StartCoroutine(Buff());
    }

    protected override void MoveSkill()
    {
        //null
    }
    public void OnDisable()
    {
        //
    }
    IEnumerator Buff()//다음 웨이브로 넘어가면 코루틴이 중지되는 문제 발견, 일단 버프 줄 때 새 버프 주는 방식으로 변경
    {
        foreach (Character hero in GameManager.Instance.EntryList)
        {
            hero.StatHandler.RemoveStatModifier(BuffStat);
            hero.StatHandler.AddStatModifier(BuffStat);
        }
        yield return new WaitForSeconds(skill.Duration);
        foreach (Character hero in GameManager.Instance.EntryList)
        {
            hero.StatHandler.RemoveStatModifier(BuffStat);
        }
    }
}
