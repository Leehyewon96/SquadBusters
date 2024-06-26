using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPlayer : CharacterBase, IAttackCircleItemInterface
{
    public delegate void OnTakeItem();
    public List<OnTakeItem> takeItemActions = new List<OnTakeItem>();

    protected override void Start()
    {
        hpBar = GameManager.Instance.hpBarManager.GetHpBar(HpBar.barType.Player); //GetComponentInChildren<HpBar>();
        attackCircle = GameManager.Instance.attackCircleManager.GetAttackCircle(AttackCircle.circleType.Player);
        OnTakeItem onTakeCoin = GainCoin;
        takeItemActions.Add(onTakeCoin);
        OnTakeItem onTakeGem = GainGem;
        takeItemActions.Add(onTakeGem);
        OnTakeItem onTakeTreasureBox = GainTreasureBox;
        takeItemActions.Add(onTakeTreasureBox);

        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (CheckInput()) //�÷��̾� �����Ҷ�
        {
            isAttacking = false;
            DetectedEnemies.Clear();
            animator.SetBool(AnimLocalize.contactEnemy, false);
            navMeshAgent.SetDestination(transform.position);
            Move();
        }
        else
        {
            if(!isAttacking)
            {
                MoveToEnemy();
            }
        }
    }


    protected virtual void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        movement3D.Move(x, z);
    }

    protected virtual bool CheckInput()
    {
        //����Ͽ��� ��ġ�� ����
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            characterController.enabled = true;
            return true;
        }

        characterController.enabled = false;
        return false;
    }

    protected override void UpdateEnemyList(GameObject target)
    {
        if (!DetectedEnemies.Contains(target)
            && Vector3.Distance(target.transform.position, transform.position) <= characterStat.GetAttackRadius())
        {
            DetectedEnemies.Add(target);
        }
    }

    protected override void Attack(GameObject target)
    {
        StartCoroutine(CoAttack(target));
    }


    protected virtual IEnumerator CoAttack(GameObject target)
    {
        animator.SetBool(AnimLocalize.contactEnemy, true);
        AnimationClip clip = animatorController.animationClips.ToList().Find(anim => anim.name.Equals(AnimLocalize.attack));
        attackTerm = new WaitForSecondsRealtime(clip.length);
        Debug.Log(clip.length);

        while (true)
        {
            yield return attackTerm;

            transform.LookAt(target.transform.position);

            if (characterController.enabled) //ĳ���� ���·� �Ǵ��ϵ��� �����ϱ�
            {
                animator.SetBool(AnimLocalize.contactEnemy, false);
                isAttacking = false;
                yield break;
            }
            if (target.TryGetComponent<CharacterBase>(out CharacterBase targetObj))
            {
                GameManager.Instance.effectManager.StoneHit(target.transform.position);
                targetObj.TakeDamage(attackDamage);

                if (targetObj.isDead)
                {
                    DetectedEnemies.Remove(target);
                    animator.SetBool(AnimLocalize.contactEnemy, false);
                    isAttacking = false;
                    yield break;
                }
            }
        }
    }

    public virtual void TakeItem(ItemType itemType)
    {
        takeItemActions[(int)itemType].DynamicInvoke();
    }

    protected virtual void GainCoin()
    {
        attackCircle.GainCoin();
    }

    protected virtual void GainGem()
    {
        attackCircle.GainGem();
    }

    protected virtual void GainTreasureBox()
    {
        attackCircle.GainTreasureBox();
    }
}
