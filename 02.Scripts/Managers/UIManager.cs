using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{
    private Stack<IUIPopUp> PopUpStack = new Stack<IUIPopUp>();

    public UISettingPanel SettingPanel;

    public void PushPopUp(IUIPopUp obj)
    {
        PopUpStack.Push(obj);
    }


    public void OnCancel(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            CloseUI();
        }
    }

    [ContextMenu("CloseTopUI")]
    public void CloseUI()
    {
        if(PopUpStack.Count > 0)
        {
            IUIPopUp obj = PopUpStack.Pop();
            obj.Hide();
        }        
        else
        {
            SettingPanel.Show();
        }
    }
}
