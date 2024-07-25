using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterRangeAttack : MonoBehaviour
{
    private WaitForSeconds AttackReady;
    public Transform SpawnPos;
    public string rcode;
    public float ReadyMotionTime;
    [SerializeField] private CharacterController _CharacterController;
    public float timeSinceLastAttack;
    float CurAs => _CharacterController.character.StatHandler.curStat.AttackSpeed;
    public void Awake()
    {
        _CharacterController = GetComponentInParent<CharacterController>();
        _CharacterController.OnAttack += OnAttack;
        timeSinceLastAttack = 0f;
        AttackReady = new WaitForSeconds(ReadyMotionTime);
    }
    public void OnDestroy()
    {
        _CharacterController.OnAttack -= OnAttack;

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
            StartCoroutine(RangeAttack());
        }
    }
    IEnumerator RangeAttack()
    {
        yield return AttackReady;
        GameObject obj = PoolManager.Instance.SpawnFromPool(rcode);
        obj.transform.position = transform.position;
        obj.SetActive(true);
        Vector2 direction;
        float angle;
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
        ProjectileController pjc = obj.GetComponent<ProjectileController>();
        pjc.Initialize(direction.normalized, angle, _CharacterController.character.CurAtk);
        
    }
}
