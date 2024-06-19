using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBase : MonoBehaviour
{
    protected Animator animator = null;
    protected CharacterController characterController = null;
    protected Movement3D movement3D = null;
    protected CharacterStat characterStat = null;
    protected NavMeshAgent navMeshAgent = null;
   
    protected HpBar hpBar = null;
    
    protected float attackRange = 2f;
    protected float attackRadius = 1f;
    protected float attackDamage = 30f;
    protected WaitForSecondsRealtime attackTerm = new WaitForSecondsRealtime(1.5f);
    protected bool isAttacking = false; // CharacterStat안에 있어야 되나?
    protected bool isDead = false;

    protected List<GameObject> DetectedEnemies = new List<GameObject>();

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        characterStat = GetComponent<CharacterStat>();
        hpBar = GetComponentInChildren<HpBar>();
        movement3D = GetComponent<Movement3D>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        hpBar.SetMaxHp(characterStat.GetMaxHp());
        hpBar.UpdateCurrentHp(characterStat.GetCurrentHp());
        
        characterStat.onCurrentHpChanged += hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpZero += SetDead;
    }

    protected virtual void Update()
    {
        //MoveToEnemy();
    }

    protected virtual void TakeDamage(float inDamage)
    {
        characterStat.ApplyDamage(inDamage);
    }

    protected virtual void SetDead()
    {
        //죽은 오브젝트 자리에 동전 생성
        isDead = true;
        gameObject.SetActive(false);
    }

    protected virtual void UpdateEnemyList(GameObject target)
    {
        if(!DetectedEnemies.Contains(target))
        {
            DetectedEnemies.Add(target);
        }
    }

    protected virtual void MoveToEnemy()
    {
        if(DetectedEnemies.Count == 0)
        {
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            return;
        }

        GameObject target = GetTarget();
        if (target == gameObject)
        {
            return;
        }

        animator.SetFloat(AnimLocalize.moveSpeed, navMeshAgent.speed);
        navMeshAgent.SetDestination(target.transform.position);

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            //navMeshAgent.SetDestination(transform.position);
            isAttacking = true;
            StartCoroutine(CoAttack(target));
        }
    }

    protected GameObject GetTarget()
    {
        GameObject target = DetectedEnemies.Find(e => !e.GetComponent<CharacterBase>().isDead);
        if(target != null)
        {
            return target;
        }

        return gameObject;
    }

    protected virtual IEnumerator CoAttack(GameObject target)
    {
        animator.SetBool(AnimLocalize.contactEnemy, true);
        transform.LookAt(target.transform.position);
        while (true)
        {
            //RaycastHit hit;
            //if (Physics.SphereCast(transform.position, attackRadius, transform.forward, out hit, attackRange))
            //{
            //    if (hit.collider.gameObject != this && hit.collider.gameObject.TryGetComponent<CharacterBase>(out CharacterBase hitObj))
            //    {
            //        hitObj.TakeDamage(attackDamage);
            //    }
            //}
            //else
            //{
            //    animator.SetBool(AnimLocalize.contactEnemy, false);
            //    isAttacking = false;
            //    yield break;
            //}

            yield return attackTerm;
            if(characterController.enabled)
            {
                yield break;
            }
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

    protected virtual void OnUnDetectEnemy(GameObject target)
    {
        //StartCoroutine(CoMoveToEnemy(target));
        if(DetectedEnemies.Contains(target))
        {
            DetectedEnemies.Remove(target);
        }
    }
}
