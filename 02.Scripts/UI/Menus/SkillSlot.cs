using UnityEngine;
using UnityEngine.UI;

public class SkillSlot :MonoBehaviour
{
    public SkillMenu skillMenu;

    public Image icon;
    public Button btn;
    public int index;

    public void Awake()
    {
        //skillMenu= GetComponentInParent<SkillMenu>();
        //icon의 이름을 index로 변경후 할당하는 로직
    }
    private void Start()
    {
        btn.onClick.AddListener(OnClickButton);
        //btn = GetComponent<Button>();
    }

    public void OnClickButton()
    {
        skillMenu.SelectSkill(index);
    }
}
