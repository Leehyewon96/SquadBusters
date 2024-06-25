using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPlayer : CharacterBase, ICharacterItemInterface
{
    public delegate void OnTakeItem();
    public List<OnTakeItem> takeItemActions = new List<OnTakeItem>();
    public List<GameObject> units = new List<GameObject>();
    

    protected override void Awake()
    {
        base.Awake();
        OnTakeItem onTakeCoin = GainCoin;
        takeItemActions.Add(onTakeCoin);
        OnTakeItem onTakeGem = GainGem;
        takeItemActions.Add(onTakeGem);
        OnTakeItem onTakeTreasureBox = GainTreasureBox;
        takeItemActions.Add(onTakeTreasureBox);
    }

    protected override void Start()
    {
        hpBar = GameManager.Instance.hpBarManager.GetHpBar(HpBar.barType.Player); //GetComponentInChildren<HpBar>();
        attackCircle = GameManager.Instance.attackCircleManager.GetAttackCircle(AttackCircle.circleType.Player);
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (CheckInput()) //플레이어 조작할때
        {
            isAttacking = false;
            DetectedEnemies.Clear();
            animator.SetBool(AnimLocalize.contactEnemy, false);
            navMeshAgent.SetDestination(transform.position);
            //MoveAttackCircle();
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
        //모바일에서 터치로 변경
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

            if (characterController.enabled) //캐릭터 상태로 판단하도록 변경하기
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
        characterStat.coin += 1;
    }

    protected virtual void GainGem()
    {
        characterStat.gem += 1;
    }

    protected virtual void GainTreasureBox()
    {
        Vector3 pos = Vector3.zero;
        float x = Random.Range(-characterStat.GetAttackRadius(), characterStat.GetAttackRadius());
        float z = Random.Range(0, Mathf.Pow(characterStat.GetAttackRadius(), 2) - Mathf.Pow(x, 2));
        pos.x = x + attackCircle.gameObject.transform.position.x;//Random.Range(transform.position.x - 0.1f, transform.position.x + 0.1f);
        pos.z = Random.Range(-Mathf.Sqrt(z), Mathf.Sqrt(z)) + attackCircle.gameObject.transform.position.z;//Random.Range(transform.position.z - 0.1f, transform.position.z + 0.1f);
        GameObject player = GameManager.Instance.Spawn(pos);
    }
}
