using UnityEngine;

public enum UIType
{
    SelectCharacter,

}

public class UIManager : MonoBehaviour
{
    [HideInInspector] public SelectCharacter selectCharacter = null;
    [HideInInspector] public CoinUI coinUI = null;
    [HideInInspector] public TreasureBoxCostUI treasureBoxCostUI = null;
    [HideInInspector] public FastMoveUI fastMoveUI = null;
    [HideInInspector] public TimeUI timeUI = null;
    [HideInInspector] public SkillUI skillUI = null;
    [HideInInspector] public RankUI rankUI = null;

    private void Awake()
    {
        selectCharacter = GetComponentInChildren<SelectCharacter>(true);
        coinUI = GetComponentInChildren<CoinUI>(true);
        treasureBoxCostUI = GetComponentInChildren<TreasureBoxCostUI>(true);
        fastMoveUI = GetComponentInChildren<FastMoveUI>(true);
        timeUI = GetComponentInChildren<TimeUI>(true);
        skillUI = GetComponentInChildren<SkillUI>(true);
        rankUI = GetComponentInChildren<RankUI>(true);
    }

    public void ShowUI(UIType uiType)
    {
        switch (uiType)
        {
            case UIType.SelectCharacter:
                selectCharacter.SetActive(true);
                break;
            default:
                break;
        }

    }
}
