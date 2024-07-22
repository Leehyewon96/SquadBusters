using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class RankElem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText = null;
    [SerializeField] private TextMeshProUGUI gemCntText = null;
    [SerializeField] private TextMeshProUGUI rankText = null;

    public bool isAssigned { get; private set; } = false;

    public void UpdateInfo(string name, string gemCnt, string rank)
    {
        nameText.SetText(name);
        gemCntText.SetText(gemCnt);
        rankText.SetText($"{rank}st");
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetIsAssigned(bool assigned)
    {
        isAssigned = assigned;
    }

    public string GetName()
    {
        return nameText.text;
    }

    public int GetRank()
    {
        return int.Parse(rankText.text.First().ToString());
    }

    public string GetGemCnt()
    {
        return gemCntText.text;
    }
}
