using UnityEngine;
using UnityEngine.TextCore.Text;
public class ProjectileSkillController : SkillObjectController
{
    public Character character;
    public Transform SpawnPoint;
    Vector3 direction = Vector3.right;
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
            direction = character.Target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
    protected override void MoveSkill()
    {
        if(character.Target != null)
        {
            transform.position += direction.normalized * skill.Data.SkillMoveSpeed;
        }
        else
        {
            transform.position += direction.normalized * skill.Data.SkillMoveSpeed;
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
            var (damage, isCritical) = character.Controller.CalculateDamage(SkillDamage(character, skill));
            damagable?.TakeDamage(damage,isCritical);
        }
    }
}
