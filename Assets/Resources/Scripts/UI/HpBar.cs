using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Image imgFill = null;
    private float maxHp = -1;

    private Camera cam = null;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void FixedUpdate()
    {
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetMaxHp(float newMaxHp)
    {
        maxHp = newMaxHp;
    }

    public void UpdateCurrentHp(float newHp)
    {
        Debug.Log($"curHp : {newHp}");
        imgFill.fillAmount = newHp / maxHp;
        Debug.Log($"imgFill.fillAmount : {imgFill.fillAmount}");
    }
}
