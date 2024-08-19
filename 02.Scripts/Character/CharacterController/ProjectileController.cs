using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class ProjectileController : MonoBehaviour
{
    Vector3 dir;
    int projectileDamage;
    bool isCritical;
    float speedModifier;
    public Animator animator;
    public float Time2Explose;
    WaitForSeconds ff;
    [SerializeField] protected LayerMask TargetCollisionLayer;
    [SerializeField] protected LayerMask WallCollisionLayer;

    float duration;
    BoxCollider2D box;
    private  void Awake()
    {
        ff = new WaitForSeconds(Time2Explose);
        box = GetComponent<BoxCollider2D>();
    }
    private void OnEnable()
    {
        duration = 0f;
        box.enabled = true;
    }

    private void Update()
    {
        transform.position += dir * speedModifier;
        if(GameManager.Instance.CombatConditionType == ECombatConditionType.END) gameObject.SetActive(false);
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
        if(IsLayerMatched(WallCollisionLayer.value, collision.gameObject.layer))
        {
            gameObject.SetActive(false);
        }
        if (IsLayerMatched(TargetCollisionLayer.value, collision.gameObject.layer))
        {
            //SoundManager.PlayFx(SoundFx.SkillHit);
            IDamagable damagable = collision.GetComponent<IDamagable>();
            damagable?.TakeDamage(projectileDamage,isCritical);
            speedModifier = 0f;
            animator?.SetTrigger("OnHit");
            StartCoroutine(ActiveFalse());
            box.enabled = false;//애니메이션 실행동안 다른 애들이 맞지 않게
        }

    }

    public void Initialize(Vector3 dir,float angle,int damage,bool isCritical)
    {
        this.dir = dir;
        projectileDamage = damage;
        this.isCritical = isCritical;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        speedModifier = 0.1f;
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