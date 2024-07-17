using UnityEngine;


public class CharacterStat : MonoBehaviour
{
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float attackRadius = 5f;
    [SerializeField] private float attackDamage = 30f;
    private float currentHp = -1f;

    public int coin = 0;
    public int gem = 0;

    public delegate void OnCurrentHpChanged(float newHp);
    public delegate void OnCurrentHpZero();
    public delegate void OnAttackRadiusChanged(float newAttackRadius);
    public delegate void OnCharacterBeginAttack();

    public event OnCurrentHpChanged onCurrentHpChanged;
    public event OnCurrentHpZero onCurrentHpZero;
    public event OnAttackRadiusChanged onAttackRadiusChanged;
    public event OnCharacterBeginAttack onCharacterBeginAttack;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public float GetCurrentHp() { return currentHp; }
    public float GetMaxHp() { return maxHp; }
    public float GetAttackRadius() { return attackRadius; }
    public float GetAttackDamage() { return attackDamage; }
    public int GetCoin() { return coin; }
    public int GetGem() { return gem; }
    

    public void Init()
    {
        maxHp = 100.0f;
        currentHp = maxHp;
    }

    public void ApplyDamage(float inDamage)
    {
        currentHp = Mathf.Clamp(currentHp - inDamage, 0, maxHp);

        if(onCurrentHpChanged != null)
        {
            onCurrentHpChanged.Invoke(currentHp);
        }
    }

    public bool CheckDead()
    {
        if (currentHp <= 0)
        {
            if (onCurrentHpZero != null)
            {
                onCurrentHpZero.Invoke();
            }
            return true;
        }

        return false;
    }

    public void UpdateAttackCircle(float newRadius)
    {
        attackRadius = newRadius;
        if (onAttackRadiusChanged != null)
        {
            onAttackRadiusChanged.Invoke(newRadius);
        }
    }

}
