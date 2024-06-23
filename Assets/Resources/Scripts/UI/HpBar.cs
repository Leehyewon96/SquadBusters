using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public enum barType
    {
        None = 0,

        Player,
        NPC,

        End = NPC + 1,
    }

    [SerializeField] private Image imgFill = null;
    public barType type = barType.None;
    private float maxHp = -1;
    [HideInInspector] public bool isUsed = false;

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

    public void UPdateIsUsed(bool used)
    {
        isUsed = used;
    }

    public void SetMaxHp(float newMaxHp)
    {
        maxHp = newMaxHp;
    }

    public void UpdateCurrentHp(float newHp)
    {
        imgFill.fillAmount = newHp / maxHp;
    }

    public void UpdatePos(Vector3 newPos)
    {
        transform.position = newPos;
    }
}
