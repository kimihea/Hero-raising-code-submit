using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMasterySlot : MonoBehaviour, IPointerClickHandler
{
    public BaseMastery Mastery;
    public Image CoolDownFill;
    public Image Icon;
    public Image LockIcon;
    public TextMeshProUGUI TimeTxt;

    private void OnEnable()
    {
        StartCoroutine(InitCoroutine());
    }

    public IEnumerator InitCoroutine()
    {
        // 수정할 부분
        yield return new WaitUntil(() => PoolManager.Instance.IsInit);
        yield return PoolManager.TaskAsIEnumerator(InitAsync());        
    }

    public async Task InitAsync()
    {
        Icon.sprite = await ResourceManager.Instance.GetResource<Sprite>(Mastery.Info.ImageRcode, EAddressableType.IMAGE);
        UpdateImage();
    }

    private void Update()
    {
        if(Mastery.Info.Condition == EMasteryCondition.ISRESEARCHING)
        {
            UpdateImage();
        }
    }

    public void UpdateImage()
    {
        switch (Mastery.Info.Condition)
        {
            case EMasteryCondition.LOCK:
                LockIcon.gameObject.SetActive(true);
                CoolDownFill.fillAmount = 1f;
                break;
            case EMasteryCondition.CANRESEARCHING:
                LockIcon.gameObject.SetActive(false);
                break;
            case EMasteryCondition.ISRESEARCHING:
                LockIcon.gameObject.SetActive(false);
                CoolDownFill.fillAmount = 1f - Mastery.Info.GetResearchRate();
                TimeTxt.gameObject.SetActive(true);
                TimeTxt.text = Mastery.GetResearchTimeTxt(); 
                break;
            case EMasteryCondition.DONE:
                LockIcon.gameObject.SetActive(false);
                TimeTxt.gameObject.SetActive(false); 
                CoolDownFill.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        GameObject obj = await ResourceManager.Instance.GetResource<GameObject>("MasteryInfo", EAddressableType.UI);
        UIMasteryInfo newObj = Instantiate(obj, UIManager.Instance.transform).GetComponent<UIMasteryInfo>();
        newObj.Show();
        newObj.InitUI(Mastery.Info, Icon.sprite);
    }
}
