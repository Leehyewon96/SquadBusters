using System.Collections;
using UnityEngine;

public class CharacterNonPlayer : CharacterBase
{
    protected override void Update()
    {
        base.Update();

        if (!photonView.IsMine)
        {
            return;
        }

        MoveToEnemy();
    }

    protected override void Attack(GameObject target)
    {
        StartCoroutine(CoAttack(target));
    }

    protected virtual IEnumerator CoAttack(GameObject target)
    {
        animator.SetBool(AnimLocalize.contactEnemy, true);
        transform.LookAt(target.transform.position);
        while (true)
        {
            yield return attackTerm;
            if (target.TryGetComponent<CharacterBase>(out CharacterBase targetObj))
            {
                targetObj.TakeDamage(attackDamage);
                if (targetObj.isDead)
                {
                    animator.SetBool(AnimLocalize.contactEnemy, false);
                    isAttacking = false;
                    yield break;
                }
            }
        }
    }


    public override void OnUnDetectEnemy(CharacterBase target)
    {
        DetectedEnemies.Clear();
    }
}
