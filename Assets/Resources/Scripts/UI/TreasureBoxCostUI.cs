using TMPro;
using UnityEngine;

public class TreasureBoxCostUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI boxCostText = null;

    public void SetBoxCostText(int newCost)
    {
        boxCostText.text = newCost.ToString();
    }
}
