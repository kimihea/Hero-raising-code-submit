using System.Collections;
using UnityEngine;

public class  SlashSkillController : SkillObjectController
{
    private Character character;
    public Transform pivot;
    private float duration;
    public float targetZRotation;
    public GameObject Effect;
    Quaternion initialRotation;
    Quaternion targetRotation;
    protected override void Awake()
    {
        base.Awake();
        duration = skill.Duration;
        character = GetComponentInParent<Character>();
        initialRotation = pivot.rotation;
        targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, targetZRotation);

    }
    protected override void ExecuteSkill()
    {
        StartCoroutine(RotateOverTime());
    }
    protected override void MoveSkill()
    {
        //
    }
    IEnumerator RotateOverTime()
    {
        Effect.SetActive(false);

        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            float t = Mathf.Pow(elapsedTime / duration, 6);
            pivot.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
            if(pivot.rotation == targetRotation) //돌아가고 난 후에 이펙트 발동
            {
                Effect.SetActive(true);
                gameObject.SetActive(false);
            }
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
            var (damage, isCritical) = character.Controller.CalculateDamage(SkillDamage(character,skill));
            damagable?.TakeDamage(damage, isCritical);
        }
    }
}
