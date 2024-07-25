using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterCloseAttack : MonoBehaviour
{
    public WaitForSeconds AttackReady;
    public Transform meleePos;
    public Vector2 boxSize;
    [SerializeField]private CharacterController _CharacterController;
    public float timeSinceLastAttack;
    float CurAtk => _CharacterController.character.StatHandler.curStat.Atk;
    float CurAs => _CharacterController.character.StatHandler.curStat.AttackSpeed;
    public void Awake()
    {
        _CharacterController  = GetComponentInParent<CharacterController>();
        _CharacterController.OnAttack += OnAttack;
        timeSinceLastAttack = 0f;
        
        AttackReady = new WaitForSeconds(0.85f);
    }
    public void OnDestroy()
    {
        _CharacterController.OnAttack -= OnAttack;

    }
    public void Start()
    {
     
    }
    public void Update()
    {
        if(timeSinceLastAttack > 1 / CurAs)
        {
            _CharacterController.isAttacking = false;
        }
        timeSinceLastAttack += Time.deltaTime;
    }
    public  void FixedUpdate()
    {
        
    }
    private void OnAttack()
    {
        if(_CharacterController.character.Animator.GetBool(_CharacterController.character.DataAnim.Attack01ParameterHash))
        {
            timeSinceLastAttack = 0f;
            _CharacterController.isAttacking = true;
            StartCoroutine(CloseAttack());
           /*공격 동작(준비/공격/마무리)은 시작했는데,준비동작에서 데미지를 입히면 이상하니까 
            공격동작에서 데미지를 입히게끔 코루틴을 사용하였음*/
        }  
    }
    private IEnumerator CloseAttack()
    {
        yield return AttackReady;
        if (!_CharacterController.isDead)
        {
            Collider2D[] coliders2Ds = Physics2D.OverlapBoxAll(meleePos.position, boxSize,0, _CharacterController.character.LayerMask);
            foreach (Collider2D colider in coliders2Ds)
            {
                if (colider.GetComponent<Character>().EntityType == _CharacterController.character.TargetType)
                {
                    colider.SendMessage("CallDamage", CurAtk);
                }
            }
        }
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(meleePos.position, boxSize);
    }
}
