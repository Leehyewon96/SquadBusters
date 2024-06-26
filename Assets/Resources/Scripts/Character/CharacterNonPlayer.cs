using System.Collections;
using UnityEngine;

public class CharacterNonPlayer : CharacterBase, ICharacterSpawnerInterface
{
    protected override void Start()
    {
        /*hpBar = GameManager.Instance.hpBarManager.GetHpBar(HpBar.barType.NPC);
        attackCircle = GameManager.Instance.attackCircleManager.GetAttackCircle(AttackCircle.circleType.NPC);
        attackCircle.UpdateOwners(this); // ���ӸŴ������� NPC �����ϴ� ������� �ٲٸ� �ű⼭ Owner�����ϰ� �� �ڵ� �����
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
        attackCircle.UpdateOwners(this); // ���ӸŴ������� NPC �����ϴ� ������� �ٲٸ� �ű⼭ Owner�����ϰ� �� �ڵ� �����
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
