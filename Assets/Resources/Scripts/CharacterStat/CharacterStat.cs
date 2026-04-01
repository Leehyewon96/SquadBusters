using System;
using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    private float maxHp = 100f;
    private float attackDamage = 30f;
    private float currentHp = -1f;

    private int coin = 0;
    private int gem = 0;

    public event Action<float> onCurrentHpChanged;
    public event Action onCurrentHpZero;
    public event Action<float> onAttackRadiusChanged;
    public event Action onCharacterBeginAttack;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public float GetCurrentHp() => currentHp;
    public float GetMaxHp() => maxHp;
    public float GetAttackDamage() => attackDamage;
    public int GetCoin() => coin;
    public int GetGem() => gem;

    public void Init(float inMaxHp, float inAttackDamage, int inCoin, int inGem)
    {
        maxHp = inMaxHp;
        currentHp = maxHp;
        attackDamage = inAttackDamage;
        coin = inCoin;
        gem = inGem;
    }

    public void ApplyDamage(float inDamage)
    {
        currentHp = Mathf.Clamp(currentHp - inDamage, 0, maxHp);
        onCurrentHpChanged?.Invoke(currentHp);
    }

    public bool CheckDead()
    {
        if (currentHp <= 0)
        {
            onCurrentHpZero?.Invoke();
            return true;
        }
        return false;
    }
}
