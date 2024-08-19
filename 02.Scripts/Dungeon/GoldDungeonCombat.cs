using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDungeonCombat : MonoBehaviour
{
    private bool isDefeat = false;
    public bool IsWin;

    private void OnEnable()
    {
        isDefeat = false;
    }

    private void Update()
    {
        if (!GameManager.Instance.EntryList[0].gameObject.activeSelf && !isDefeat)
        {
            Defeat();            
            return;
        }
        if (GameManager.Instance.GoldDungeon.RemainTime > 0 && !isDefeat)
        {
            if (!IsWin) GameManager.Instance.GoldDungeon.RemainTime -= Time.deltaTime;
            if(GameManager.Instance.GoldDungeon.RemainTime <= 0)
            {
                Defeat();                
                return;
            }
        }
    }

    public void Defeat()
    {
        GameManager.Instance.DefeatBattle();
        GameManager.Instance.GoldDungeon.StopSpawn();
        isDefeat = true;
    }
}
