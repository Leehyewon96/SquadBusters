using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    [SerializeField] private float maxHp = 100f;
    private float currentHp = -1;

    public delegate void OnCurrentHpChanged(float newHp);
    public delegate void OnCurrentHpZero();

    public event OnCurrentHpChanged onCurrentHpChanged;
    public event OnCurrentHpZero onCurrentHpZero;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public float GetCurrentHp() { return currentHp; }
    public float GetMaxHp() { return maxHp; }

    public void ApplyDamage(float inDamage)
    {
        currentHp = Mathf.Clamp(currentHp - inDamage, 0, maxHp);

        if (onCurrentHpChanged != null)
        {
            onCurrentHpChanged.Invoke(currentHp);
        }

        if(currentHp <= 0)
        {
            if (onCurrentHpZero != null)
            {
                onCurrentHpZero.Invoke();
            }
        }
    }
}
