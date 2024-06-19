using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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
    protected bool isAttacking = false; // CharacterStat안에 있어야 되나?

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
        if(isAttacking)
        {
            return;
        }

        if(DetectedEnemies.Count == 0)
        {
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            return;
        }

        isAttacking = true;
        animator.SetFloat(AnimLocalize.moveSpeed, navMeshAgent.speed);
        navMeshAgent.SetDestination(DetectedEnemies.FirstOrDefault().transform.position);
        
    }

    protected virtual void Attack(GameObject target)
    {
        if(isAttacking)
        {
            return;
        }

        isAttacking = true;

        animator.SetBool(AnimLocalize.contactEnemy, true);
        StartCoroutine(CoAttack(target));
    }

    private IEnumerator CoAttack(GameObject target)
    {
        while (true)
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, attackRadius, transform.forward, out hit, attackRange))
            {
                if (hit.collider.gameObject != this && hit.collider.gameObject.TryGetComponent<CharacterBase>(out CharacterBase hitObj))
                {
                    hitObj.TakeDamage(attackDamage);
                }
            }
            else
            {
                animator.SetBool(AnimLocalize.contactEnemy, false);
                isAttacking = false;
                yield break;
            }

            //if(target.TryGetComponent<CharacterBase>(out CharacterBase targetObj))
            //{
            //    targetObj.TakeDamage(attackDamage);
            //}

            yield return new WaitForSeconds(1f);
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
