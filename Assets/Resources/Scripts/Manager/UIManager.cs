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
    [HideInInspector] public EndingUI endingUI = null;
    [HideInInspector] public NoticeUI noticeUI = null;

    private void Awake()
    {
        selectCharacter = GetComponentInChildren<SelectCharacter>(true);
        coinUI = GetComponentInChildren<CoinUI>(true);
        treasureBoxCostUI = GetComponentInChildren<TreasureBoxCostUI>(true);
        fastMoveUI = GetComponentInChildren<FastMoveUI>(true);
        timeUI = GetComponentInChildren<TimeUI>(true);
        skillUI = GetComponentInChildren<SkillUI>(true);
        rankUI = GetComponentInChildren<RankUI>(true);
        endingUI = GetComponentInChildren<EndingUI>(true);
        noticeUI = GetComponentInChildren<NoticeUI>(true);

        selectCharacter.onDisabled += () => ShowUI(UIType.SelectCharacter, false);
    }

    public void ShowUI(UIType uiType, bool isActive)
    {
        switch (uiType)
        {
            case UIType.SelectCharacter:
                if (isActive == selectCharacter.gameObject.activeSelf) return;

                selectCharacter.SetActive(isActive);
                fastMoveUI.SetActive(!isActive);
                skillUI.SetActive(!isActive);

                int sign = isActive ? -1 : 1;
                float halfWidth = selectCharacter.GetComponent<RectTransform>().sizeDelta.x * 0.5f;

                Vector3 coinPos = coinUI.GetComponent<RectTransform>().position - sign * Vector3.right * halfWidth;
                coinUI.UpdatePos(coinPos);

                Vector3 boxPos = treasureBoxCostUI.GetComponent<RectTransform>().position - sign * Vector3.right * halfWidth;
                treasureBoxCostUI.UpdatePos(boxPos);
                break;
        }
    }

    public void OnStopGame()
    {
        selectCharacter.SetActive(false);
        coinUI.SetActive(false);
        treasureBoxCostUI.SetActive(false);
        fastMoveUI.SetActive(false);
        timeUI.SetActive(false);
        skillUI.SetActive(false);
        rankUI.SetActive(false);

        endingUI.SetActive(true);
    }
}
