using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText = null;

    public void UpdateTime(string newTime)
    {
        timeText.SetText(newTime);
    }
}
