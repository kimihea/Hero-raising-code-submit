using UnityEngine;
using UnityEngine.TextCore.Text;
public class ProjectileSkillController : SkillObjectController
{
    Character character;
    public Transform SpawnPoint;
    protected override void Awake()
    {
        base.Awake();
        character=GetComponentInParent<Character>();
    }
    protected override void ExecuteSkill()
    {
        transform.position = SpawnPoint.position;
        if (character.Target != null)
        {
            Vector2 direction = character.Target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
    protected override void MoveSkill()
    {
        if(character.Target != null)
        {
            transform.position += (character.Target.position - SpawnPoint.position).normalized * skill.Data.SkillMoveSpeed;
        }
        else
        {
            transform.position += Vector3.right * skill.Data.SkillMoveSpeed;
        }
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsLayerMatched(WallCollisionLayer.value, collision.gameObject.layer))
        {
            gameObject.SetActive(false);
        }
        else if (IsLayerMatched(TargetCollisionLayer.value, collision.gameObject.layer))
        {
            //SoundManager.PlayFx(SoundFx.SkillHit);
            IDamagable damagable = collision.GetComponent<IDamagable>();
            damagable?.TakeDamage((int)(character.CurAtk* skill.DamagePerGradge()));
        }
    }
}
