using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class ProjectileController : MonoBehaviour
{
    Vector3 dir;
    int projectileDamage;
    float speedModifier;
    public Animator animator;
    WaitForSeconds ff;
    [SerializeField] protected LayerMask TargetCollisionLayer;
    float duration;
    private  void Awake()
    {
        duration = 0f;
        ff = new WaitForSeconds(0.2f);
    }
    
    private void Update()
    {
        transform.position += dir * speedModifier;
        if (duration < 3f)
            duration += Time.deltaTime;
        else
        {
            duration = 0f;
            gameObject.SetActive(false);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsLayerMatched(TargetCollisionLayer.value, collision.gameObject.layer))
        {
            //SoundManager.PlayFx(SoundFx.SkillHit);
            IDamagable damagable = collision.GetComponent<IDamagable>();
            damagable?.TakeDamage(projectileDamage);
            speedModifier = 0f;
            animator.SetTrigger("OnHit");
            StartCoroutine(ActiveFalse());
        }
    }

    public void Initialize(Vector3 dir,float angle,int damage)
    {
        this.dir = dir;
        projectileDamage = damage;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        speedModifier = 0.05f;
    }
    public bool IsLayerMatched(int layerMask, int objectLayer)
    {
        return layerMask == (layerMask | (1 << objectLayer));
    }
    IEnumerator ActiveFalse()
    {
        yield return ff;
        gameObject.SetActive(false);
    }
}