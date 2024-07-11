using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text = null;

    public void SetCoin(int cnt)
    {
        text.SetText(cnt.ToString());
    }
}
