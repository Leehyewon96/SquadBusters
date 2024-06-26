using System.Collections;
using UnityEngine;

public class CharacterNonPlayer : CharacterBase
{
    protected override void Start()
    {
        hpBar = GameManager.Instance.hpBarManager.GetHpBar(HpBar.barType.NPC);
        attackCircle = GameManager.Instance.attackCircleManager.GetAttackCircle(AttackCircle.circleType.NPC);
        attackCircle.UpdateOwners(gameObject);
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        MoveToEnemy();
        //MoveAttackCircle();

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

    protected override void SetDead()
    {
        attackCircle.UpdateIsUsed(false);
        attackCircle.SetActive(false);

        //죽은 오브젝트 자리에 동전 생성
        GameManager.Instance.itemManager.ShowItem(characterStat.coin, transform.position, ItemType.Coin);
        GameManager.Instance.itemManager.ShowItem(characterStat.gem, transform.position, ItemType.Gem);
        GameManager.Instance.effectManager.Explosion(transform.position);

        base.SetDead();
    }
}
