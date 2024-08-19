using System.Collections;
using UnityEngine;

public class ErruptionSkillController : SkillObjectController
{
    public Transform SpawnPos;
    public Character character;
    public BoxCollider2D AnimObjColider;
    private WaitForSeconds waitInit = new WaitForSeconds(0.1f);
    private WaitForSeconds waitTemp = new WaitForSeconds(0.2f);
    public SpriteRenderer spr;
    public float XAdd;
    public float YAdd;
    //스킬이 점점 퍼져 나가면서 데미지를 입히는
    protected override void ExecuteSkill()
    {
        AnimObjColider.enabled = false;
        StartCoroutine(CoColiderOff());
        if (character.Target != null)
        {
            if (character.Target.position.x < character.transform.position.x)
            {
                SpawnPos.localPosition = Vector3.left;
                spr.flipX = true;
                transform.position = SpawnPos.position + new Vector3(-XAdd, YAdd, 0);

            }
            else
            {
                spr.flipX = false;
                SpawnPos.localPosition = Vector3.right;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                transform.position = SpawnPos.position + new Vector3(XAdd, YAdd, 0);
            }
        }
        
    }

    protected override void MoveSkill()
    {
        
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
            damagable?.TakeDamage(damage, isCritical);
        }
    }
    private IEnumerator CoColiderOff()
    {
        yield return waitInit;
        AnimObjColider.enabled = true;
        yield return waitTemp;
        AnimObjColider.enabled = false;
    }

}
