using System.Collections;
using Unity.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;
public class CharacterRangeAttack : MonoBehaviour
{
    private WaitForSeconds AttackReady;
    public Transform SpawnPos;
    public bool IsFlash;
    public string rcode;
    public float ReadyMotionTime;
    [SerializeField] private CharacterController _CharacterController;
    public float timeSinceLastAttack;
    float CurAs => _CharacterController.character.StatHandler.curStat.AttackSpeed;

    /*����ü ������*/
    Vector2 direction;
    GameObject obj;
    float angle;
    ProjectileController pjc;
    IDamagable damagable;
    Coroutine MyCoroutine;
    public void Awake()
    {
        _CharacterController = GetComponentInParent<CharacterController>();
        
        timeSinceLastAttack = 0f;
        AttackReady = new WaitForSeconds(ReadyMotionTime);
    }
    public void OnEnable()
    {
        _CharacterController.OnAttack += OnAttack;
    }
    public void OnDisable()
    {
        _CharacterController.OnAttack -= OnAttack;
        if(obj != null)obj.SetActive(false);
    }
    public void Update()
    {
        if (timeSinceLastAttack > 1 / CurAs)
        {
            _CharacterController.isAttacking = false;
        }
        timeSinceLastAttack += Time.deltaTime;
    }
    private void OnAttack()
    {
        if (_CharacterController.character.Animator.GetBool(_CharacterController.character.DataAnim.Attack01ParameterHash))
        {
            timeSinceLastAttack = 0f;
            _CharacterController.isAttacking = true;
            MyCoroutine=StartCoroutine(RangeAttack());
        }
    }
    IEnumerator RangeAttack()
    {
        yield return AttackReady;
        if (_CharacterController.character.Target == null) yield break;
        obj = PoolManager.Instance.SpawnFromPool(rcode);
        obj.transform.position = !IsFlash ? transform.position : _CharacterController.character.Target.position;
        obj.SetActive(true);
        if (IsFlash)
        {
            damagable=_CharacterController.character.Target.gameObject.GetComponent<IDamagable>();
            var (damage, isCritical) = _CharacterController.CalculateDamage(_CharacterController.character.StatHandler.curStat.GetCurAtk());
            damagable?.TakeDamage(damage, isCritical);
            yield return AttackReady;
            obj.SetActive(false);
        }
        else
        {
            if (_CharacterController.character.Target != null)
            {
                direction = _CharacterController.character.Target.position - transform.position;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                //obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
            else
            {
                direction = Vector2.right;
                angle = 0f;
            }
            pjc = obj.GetComponent<ProjectileController>();
            var (damage, isCritical) = _CharacterController.CalculateDamage(_CharacterController.character.StatHandler.curStat.GetCurAtk());
            pjc.Initialize(direction.normalized, angle, damage,isCritical);
        }  
        
    }
}
