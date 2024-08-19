using System.Collections;
using UnityEngine;

public class BossController : CharacterController
{
    private MaterialPropertyBlock mpb;
    protected void Start()
    {
        mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Color", Color.red);
        spriteRenderer.SetPropertyBlock(mpb);
    }
    public override IEnumerator PlayHurtAnimationAndIdleCoroutine()
    {
        //float elapsedTime = 0f;
        //while (elapsedTime < 0.2f)
        //{
        //    elapsedTime += Time.deltaTime;
        //    yield return new WaitForEndOfFrame();
        //    //spriteRenderer.color = Color.red;
        //}
        mpb.SetFloat("_FlipX", spriteRenderer.flipX ? 1.0f : 0.0f);
        spriteRenderer.SetPropertyBlock(mpb);
        yield return hurtAnimLength;
        spriteRenderer.color = originalColor;
    }

}

