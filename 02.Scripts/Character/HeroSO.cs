using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Hero", menuName = "HeroSO")]
public class HeroSO : ScriptableObject
{
    public int hid;
    public ERoleType roleType;
    public Sprite icon;

    public string heroName;
    public string heroDescription;

    public string RCode;

    public CharacterStat multipleStat;

}