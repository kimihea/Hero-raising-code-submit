using UnityEngine;

public class BodyEffecter : MonoBehaviour
{
    //몬스터는 피격시 이펙트 나오고
    //PLAYER들은 스킬 사용시 이펙트 나오게 하기
    private SpriteRenderer bodySprite;
    private Animator effectAnimator;


    ///summary
    ///컴포넌트 할당
    ///summary
    public void Awake()
    {
        bodySprite = GetComponent<SpriteRenderer>();
        effectAnimator = GetComponent<Animator>();
    }
}

