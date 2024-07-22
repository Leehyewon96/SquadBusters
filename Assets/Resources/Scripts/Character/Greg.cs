using UnityEngine;

public class Greg : CharacterPlayer
{
    public void CutMoneyTree(MoneyTree tree)
    {
        animator.SetBool(AnimLocalize.attack, true);
        tree.TakeDamage(10f);
    }
}
