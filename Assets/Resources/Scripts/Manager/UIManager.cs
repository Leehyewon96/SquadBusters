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

    private void Awake()
    {
        selectCharacter = GetComponentInChildren<SelectCharacter>(true);
        coinUI = GetComponentInChildren<CoinUI>(true);
        treasureBoxCostUI = GetComponentInChildren<TreasureBoxCostUI>();
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
