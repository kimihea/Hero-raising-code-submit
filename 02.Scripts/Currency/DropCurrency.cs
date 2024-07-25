using Cinemachine.Utility;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCurrency : MonoBehaviour
{
    [field: SerializeField] public ECurrencyType CurrencyType { get; private set; }
    [field: SerializeField] public int Amount { get; private set; }
    [SerializeField] public RectTransform TargetPos;

    public void InitDropReward(ECurrencyType type, int amount)
    {
        CurrencyType = type;
        Amount = amount;
    }

    public void GetRewards()
    {
        StartCoroutine(GetRewardsCoroutine());
    }

    private IEnumerator GetRewardsCoroutine()
    {
        float duration = 1f;
        //float curTime = 0f;
        //Vector3 targetPos = Camera.main.ScreenToWorldPoint(TargetPos.position);        
        yield return new WaitForSeconds(1f);
        //Vector3 curPos = transform.position;
        //DOTween.To(() => curPos, x => curPos = x, TargetPos.position, duration).SetEase(Ease.InCirc);
        transform.DOMoveX(TargetPos.position.x, duration).SetEase(Ease.OutCirc);
        transform.DOMoveY(TargetPos.position.y, duration).SetEase(Ease.InCirc);
        //do
        //{
        //    //curTime += Time.deltaTime;
        //    //transform.position = Vector2.Lerp(curPos, TargetPos.position, curTime / duration);

        //    yield return null;
        //} while (duration - curTime > 0);   
        yield return new WaitForSeconds(duration);
        CurrencyManager.Instance.AddCurrency(CurrencyType, Amount);

        gameObject.SetActive(false);
    }
}
