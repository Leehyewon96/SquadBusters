using System.Collections;
using UnityEngine;

public class CharacterNonPlayer : CharacterBase, ICharacterSpawnerInterface
{
    protected override void Start()
    {
        /*hpBar = GameManager.Instance.hpBarManager.GetHpBar(HpBar.barType.NPC);
        attackCircle = GameManager.Instance.attackCircleManager.GetAttackCircle(AttackCircle.circleType.NPC);
        attackCircle.UpdateOwners(this); // 게임매니저에서 NPC 스폰하는 방식으로 바꾸면 거기서 Owner지정하고 이 코드 지우기
        base.Start();*/
    }

    protected override void Update()
    {
        base.Update();
        MoveToEnemy();
    }

    protected override void Attack(GameObject target)
    {
        StartCoroutine(CoAttack(target));
    }

    public override void Init()
    {
        hpBar = GameManager.Instance.hpBarManager.GetHpBar(HpBar.barType.NPC);
        attackCircle = GameManager.Instance.attackCircleManager.GetAttackCircle(AttackCircle.circleType.NPC);
        attackCircle.UpdateOwners(this); // 게임매니저에서 NPC 스폰하는 방식으로 바꾸면 거기서 Owner지정하고 이 코드 지우기
        characterStat.Init();
        base.Init();
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


    public bool GetIsDead()
    {
        return isDead;
    }

    public override CharacterType GetCharacterType()
    {
        return characterType;
    }

    public void SetIsDead(bool value)
    {
        isDead = value; 
    }
}
