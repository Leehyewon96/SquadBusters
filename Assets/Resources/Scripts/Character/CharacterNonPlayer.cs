using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CharacterNonPlayer : CharacterBase, ICharacterSpawnerInterface
{
    //protected override void Update()
    //{
    //    base.Update();
    //    //MoveToEnemy();
    //}

    //public virtual void SetAttackCircle(NPCAttackCircle inAttackCircle)
    //{
    //    attackCircle = inAttackCircle;
    //}

    protected override void Attack(GameObject target)
    {
        StartCoroutine(CoAttack(target));
    }

    public override void Init()
    {
        string path = $"Prefabs/UI/HpBar/NPCHpBarCanvas";
        GameObject obj = Resources.Load(path) as GameObject;
        GameObject hpBarobj = Instantiate(obj, transform.position, Quaternion.identity);
        hpBar = hpBarobj.GetComponentInChildren<HpBar>();//GameManager.Instance.hpBarManager.GetHpBar(HpBar.barType.NPC);
        //attackCircle = Instantiate(attackCircleOrigin, transform.position, Quaternion.identity).GetComponent<NPCAttackCircle>();
        //attackCircle.UpdateOwners(this);
        DetectedEnemies.Clear();
        characterStat.Init();
        //attackCircle.UpdateRadius(4f);

        StartCoroutine(CoInit());
    }

    private IEnumerator CoInit()
    {
        yield return new WaitUntil(() => hpBar != null);
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
