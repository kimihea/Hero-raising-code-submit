using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class SkillSaveData
{
    public int[] PSkillIndex;

    public SkillData[] skillDataArray;

    public bool[] IsLockArray;
}

[System.Serializable]
public class SkillData
{
    public int Stars;
    public int Count;
}

public class SkillManager : Singleton<SkillManager>
{
    public SkillController PlayerSkillController;//controller에 접근해서 스킬리스트에 스킬을 추가/삭제하는 식으로 할 예정
    public int[] PSkillIndex = new int[3]{99,99,99}; //99는 없다는 뜻
    public int[] TempSkillIndex = new int[3] { 0, 0, 0 };
    public SkillMenu skillMenu; //Ui Update간편하게 하기 위해서 참조... 개선여부 O

    public List<Skill> SkillList = new List<Skill>();
    public Dictionary<int, List<Skill>> SkillDic = new();
    public SkillMenu SkillMenu;

    public GameObject alertDialog;
    public Button yesButton;

    public UsingSkill CurUsingSkill;
    public UsingSkill CurUsingSkillOnMain;
    public Sprite DefalutSprite;

    public SkillSaveData SkillSaveData;
    protected override void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        base.Awake();
        SkillLoadData();
        ConvertListToDict();
    }
    void Update()
    {
        //float fps = 1.0f / Time.deltaTime;
        //Debug.Log("Current FPS: " + fps);
    }
    private void Start()
    {
        GetCurrentSkillIndex();
        
        CurUsingSkill.UpdateImage();
        CurUsingSkillOnMain.UpdateImage();
    }
    void ConvertListToDict()
    {
        SkillDic.Clear();
        foreach (Skill skill in SkillList)
        {
            if (!SkillDic.TryGetValue(skill.Data.HeroId, out List<Skill> skills))
            {
                skills = new List<Skill>();
                SkillDic[skill.Data.HeroId] = skills;
            //Debug.Log($"Skill ID: {skill.Data.HeroId} added to the dictionary.");
            }
            skills.Add(skill);
        }
    }
    #region 정보 반환 메소드
    public Skill IndexToPlayerSkill(int index)
    {
        try
        {
            if (SkillDic.TryGetValue(0, out List<Skill> skills))
            {
                if (IsIndexValid(index, skills))
                {
                    return skills[index];
                }
                else
                {
                    Debug.LogError($"Index out of bounds: {index}. Valid range is 0 to {skills.Count - 1}.");
                    return null;
                }
            }
            else
            {
                Debug.LogError("Player skill list does not exist.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unhandled exception in IndexToPlayerSkill: {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }
    public List<string> HeroIdToSkill(int heroId)
    {
        List<string> strings = new List<string>();
        if (SkillDic.TryGetValue(heroId, out List<Skill> skills))
        {
            foreach (Skill skill in skills)
            {
                string str = skill.GetSkillDescription(777);
                strings.Add(str);
            }
        }
        return strings;
    }
    public List<Sprite> HeroIdToSprite(int heroId)
    {
        List<Sprite> result = new();
        if (SkillDic.TryGetValue(heroId, out List<Skill> skills))
        {
            foreach (Skill skill in skills)
            {
                Sprite selectedValue = skill.Data.Icon;
                result.Add(selectedValue);
            }
        }
        else
        {
            //Debug.Log("Sprite is dont exist");
        }
        return result;
    }
    public List<T> HeroIdToList<T>(int heroId, Func<Skill, T> skillSelector,T defaultValue)
    {
        List<T> result = new List<T>();
        if (SkillDic.TryGetValue(heroId, out List<Skill> skills))
        {
            foreach (Skill skill in skills)
            {
                T selectedValue = skillSelector(skill);
                result.Add(selectedValue);
            }
        }
        else
        {
            result.Add(defaultValue);
        }
        return result;
    }

    public SkillInfo GetPlayerSkillInfo(int index)
    {
        if (SkillDic.TryGetValue(0, out List<Skill> skills))
        {
        //    Debug.Log("Get Skill success"+index);s
        }
        //else Debug.Log("wrong index");
        Skill value = skills[index];
        SkillInfo returnSKill = new SkillInfo()
        {
            Stars = value.Stars,
            Count = value.Count,
            Name = value.Data.Name,
            Description = value.GetSkillDescription(1),
            CoolTime = value.CoolTime.ToString(),
            PassiveEffect = value.Data.Passive,
            Icon = value.Data.Icon,
        };
        return returnSKill;
    }
    public Skill GetFirstSkill(int index)
    {
        if (SkillDic.TryGetValue(0, out List<Skill> skills))
            return skills[0];
        else
        {
            Debug.Log(index +"없는 스킬입니다");
            return skills[0];
        }
    }
    #endregion

    #region SkillIndexBuffer관련
    public void OnSkillBufferStart() //스킬창 열릴시
    {
        for(int i = 0; i<3; i++)
        {
            TempSkillIndex[i] = PSkillIndex[i];
        }
        CurUsingSkill.UpdateImage();
    }
    public void OnSkillBufferEnd() //스킬 적용 버튼 누를시
    {
        for (int i = 0; i < 3; i++)
        {
            PSkillIndex[i] = TempSkillIndex[i];
        }
    }
    public void AddSkillBuffer(int index)
    {
        for (int i = 0; i < 3; i++)
        {
            if (TempSkillIndex[i] == 99)
            {
                TempSkillIndex[i] = index;
                return;
            }
        }
    }
    public void SubstractSkillBuffer(int index)
    {
        for (int i = 0; i < 3; i++)
        {
            if (TempSkillIndex[i] == index)
            {
                TempSkillIndex[i] = 99;
                return;
            }
        }
    }
    public void GetCurrentSkillIndex()
    {
        //스킬리스트의 무슨 스킬이 들어갔는지 가져오기
        for (int i = 0; i < 3; i++)
        {
            if (IsIndexValid(i, PlayerSkillController.SkillList))
                PSkillIndex[i] = PlayerSkillController.SkillList[i].Data.SkillId;
            else
                PSkillIndex[i] = 99;
        }
    }
    ///<summary>
    /// PSkillList의 길이를 확인하는 것
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <param name="myList"></param>
    /// <returns></returns>
    bool IsIndexValid<T>(int index,List<T> myList)
    {
        return index >= 0 && index < myList.Count;
    }
    #endregion

    #region 스킬 장착

    public bool EquipSkill(int index)// 스킬 바로적용
    {        
        if (SkillDic.TryGetValue(0, out List<Skill> skills))
        {
            if (PlayerSkillController.SkillList.Count < 3)
            {
                if (!PlayerSkillController.SkillList.Contains(skills[index]))
                {
                    PlayerSkillController.SkillList.Add(skills[index]);
                    PlayerSkillController.UpdateSkillList(PlayerSkillController.SkillList);
                    GetCurrentSkillIndex();
                    CurUsingSkill.UpdateImage();
                    CurUsingSkillOnMain.UpdateImage();
                    QuestManager.Instance.AddProgress(EQuestType.SKILLCAST, 1);
                    return true;
                }
            }
            else
            {
                if (!PlayerSkillController.SkillList.Contains(skills[index]))
                {
                    ShowChangeAlert(() =>
                   {
                       if (PlayerSkillController.SkillCoolTimeAmount(0) == 0)
                       {
                           skillMenu.UpdateEquipUI(PlayerSkillController.SkillList[0].Data.SkillId, false);
                           PlayerSkillController.SkillList.RemoveAt(0);
                           PlayerSkillController.SkillList.Add(skills[index]);
                           PlayerSkillController.UpdateSkillList(PlayerSkillController.SkillList);
                           GetCurrentSkillIndex();
                           CurUsingSkill.UpdateImage();
                           CurUsingSkillOnMain.UpdateImage();
                           QuestManager.Instance.AddProgress(EQuestType.SKILLCAST, 1);
                           skillMenu.UpdateEquipUI(index, true);
                       }
                       else 
                            GameManager.Instance.ShowAlert("재사용 대기중에는 해제 할 수 없습니다", EAlertType.LACK);
                   });
                }
            }
        }
        return false;
    }
    public void ShowChangeAlert(Action onYesAction)
    {
        // 경고 메시지 표시
        alertDialog.SetActive(true);

        // "Yes" 버튼에 클릭 이벤트 등록
        yesButton.onClick.RemoveAllListeners(); // 이전에 등록된 리스너 제거
        yesButton.onClick.AddListener(() =>
        {
            onYesAction?.Invoke(); // 전달된 로직 실행
            alertDialog.SetActive(false); // 경고창 닫기
        });
    }
        public bool EquipSkillOnBuffer(int index) //버퍼에 저장하고 일괄적용
        {
            if (SkillDic.TryGetValue(0, out List<Skill> skills))
            {
                if (TempSkillIndex.Contains(99))//index에 99값이 있다면 자리가 비었다는 뜻
                {
                    if (!TempSkillIndex.Contains(index))//TempIndex에 없다면
                    {
                        AddSkillBuffer(index);
                        CurUsingSkill.UpdateImage(true);
                        return true;
                    }
                    else
                    {
                        //Debug.Log("이미 장착함");
                        return false;
                    }
                }
                else
                {
                    //Debug.Log("남는 칸이 없습니다");
                    return false;
                }
            }
            Debug.LogError("Player skill list does not exist.");
            return false;
        }
    public bool UnEquipSkill(int index)// 스킬 바로적용
    {
        if (SkillDic.TryGetValue(0, out List<Skill> skills))
        {
            if (PlayerSkillController.SkillList.Contains(skills[index]))
            {
                int count=0;
                foreach(var indexx in PSkillIndex)
                {
                    if (index == indexx) 
                        break;
                    count++;
                }
                if (PlayerSkillController.SkillCoolTimeAmount(count) != 0) 
                {
                    GameManager.Instance.ShowAlert("재사용 대기중에는 해제 할 수 없습니다", EAlertType.LACK);
                    return false;//스킬의 재사용 대기시간이 돌고 있다면 못 빼게 해라.
                }
                PlayerSkillController.SkillList.Remove(skills[index]);
                PlayerSkillController.UpdateSkillList(PlayerSkillController.SkillList);
                GetCurrentSkillIndex();
                CurUsingSkill.UpdateImage();
                CurUsingSkillOnMain.UpdateImage();
                return true;
            }
        }
        return false;
    }
    public bool UnEquipSkillOnBuffer(int index)//버퍼에 저장하고 일괄적용
    {
        if (SkillDic.TryGetValue(0, out List<Skill> skills))
        {
            if (TempSkillIndex.Contains(index))//스킬을 가지고 있다면
            {
                
                 SubstractSkillBuffer(index);
                 CheckIsEmpty();
                 CurUsingSkill.UpdateImage(true);
                 return true;
            }
            else
            {
                //Debug.Log("장착한 스킬이 아닙니다.");
                return false;
            }
        }
        Debug.LogError("Player skill list does not exist.");
        return false;
    }

    private void CheckIsEmpty() //배열의 끝요소를 제외한 모든 요소를 탐색해서, 만약 비어 있을 시 배열의 모든 요소중에서 비어있지 않은 요소를 탐색.
                                //탐색되면 교환해주는 로직.
    {
        for(int i = 0; i < TempSkillIndex.Length-1; i++)
        {
            if(TempSkillIndex[i] == 99)//비어있다면
            {
                int ii=i+1;
                while (ii < TempSkillIndex.Length-1 && TempSkillIndex[ii] == 99)//배열의 마지막전까지 && 비어있지 않다.
                {
                    ii++;
                }
                int temp;
                temp = TempSkillIndex[i];
                TempSkillIndex[i] = TempSkillIndex[ii];
                TempSkillIndex[ii] = temp;
            }
        }
    }

    public void ApplySkill() //TempSKillIndex의 값들을 적용시키고 UI이미지 적용
    {
        if (SkillDic.TryGetValue(0, out List<Skill> skills))
        {
            
            PlayerSkillController.SkillList.Clear();
            for (int i = 0;i<3;i++)
            {
                if (TempSkillIndex[i] != 99)
                {
                    PlayerSkillController.SkillList.Add(skills[TempSkillIndex[i]]);
                }
            }
            PlayerSkillController.UpdateSkillList(PlayerSkillController.SkillList);
        }
        else
        {
            Debug.LogError("Player skill list does not exist.");
        }
        OnSkillBufferEnd();
        CurUsingSkillOnMain.UpdateImage();
    }

    #endregion
    public void OnBattleChanged()
    {
        PlayerSkillController.ShutDownSkill(PlayerSkillController.SkillList);
        for (int i = 0; i < PlayerSkillController.CoolDownList.Count; i++)
        {
            PlayerSkillController.CoolDownList[i] = 0;
        }
    }

    public void SaveData()
    {
        SkillMenu.SlotSaveData();
        SkillSaveData.PSkillIndex = PSkillIndex;
        SkillSaveData.IsLockArray = SkillMenu.IsLockArray;
        SkillSaveData.skillDataArray = new SkillData[SkillList.Count];

        for ( int i = 0; i < SkillList.Count; i++)
        {
            SkillSaveData.skillDataArray[i] = new SkillData();

            //Debug.Log(SkillSaveData);
            //Debug.Log(SkillSaveData.skillDataArray);
            //Debug.Log(SkillSaveData.skillDataArray[i]);
            //Debug.Log(SkillSaveData.skillDataArray[i].Stars);

            SkillSaveData.skillDataArray[i].Stars = SkillList[i].Stars;
            SkillSaveData.skillDataArray[i].Count = SkillList[i].Count;
        }
    }

    public void SkillLoadData()
    {
        SkillSaveData loadData = DataManager.Instance.LoadData<SkillSaveData>(ESaveType.SKILL);

        

        if (loadData == null)
        {
            //Debug.Log("1111");
            return;
        }
        else
        {
            SkillSaveData = loadData;

            //Debug.Log("2222");
            PSkillIndex = loadData.PSkillIndex;
            SkillMenu.IsLockArray=loadData.IsLockArray;
            SkillMenu.SlotLoadData();
            SkillMenu.LoadEquip(PSkillIndex);
            //SkillMenu.UpdateEquip(PSkillIndex);
            //PSkillIndex[i] = PlayerSkillController.SkillList[i].Data.SkillId;

            PlayerSkillController.SkillList.Clear();
            PlayerSkillController.CoolDownList.Clear();
            PlayerSkillController.waitSkillMotionList.Clear();

            for (int i = 0; i < PSkillIndex.Length; i++)
            {
                
                for ( int j = 0; j < SkillList.Count; j++)
                {
                    //Debug.Log($"{PSkillIndex[i]} / {SkillList[j].Data.SkillId}");

                    if (PSkillIndex[i] == SkillList[j].Data.SkillId)
                    {
                        //Debug.Log("여기!");
                        PlayerSkillController.SkillList.Add(SkillList[j]);

                        PlayerSkillController.SkillList[i].IsCharge = true;
                        PlayerSkillController.CoolDownList.Add(0);
                        PlayerSkillController.waitSkillMotionList.Add(new WaitForSeconds(PlayerSkillController.SkillList[i].Data.ChannelingTime));


                    }
                }
            }


            for ( int i = 0; i < loadData.skillDataArray.Length; i++)
            {
                SkillList[i].Stars = loadData.skillDataArray[i].Stars;
                SkillList[i].Count = loadData.skillDataArray[i].Count;
            }

        }
    }
}

