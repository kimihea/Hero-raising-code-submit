using UnityEngine.TextCore.Text;
using System.Collections;
public enum EBuffType
{
    SPECIAL, //"The special skill's animation will be shown by its animator."
    ATK,
    DEF,
    
}
public interface IHandleBuff
{
    void ActiveBuff(CharacterStat buffStat, float time, EBuffType Type);
}
public abstract class BuffSkillController : SkillObjectController
{
    public CharacterStat BuffStat;
    public EBuffType Type;

    protected override void Awake()
    {
        base.Awake();
        SetBuffStat();
    }
    protected override void ExecuteSkill()
    {
        foreach (Character hero in GameManager.Instance.EntryList)
        {
            if (hero.isActiveAndEnabled)
            {
                IHandleBuff buff = hero.GetComponent<IHandleBuff>();
                buff?.ActiveBuff(BuffStat, skill.Duration, Type);
            }
            
        }
    }
    protected override void TerminateSkill()
    {
        base.TerminateSkill();
    }
    protected override void MoveSkill()
    {
        //null
    }
    
    public abstract void SetBuffStat();
}
