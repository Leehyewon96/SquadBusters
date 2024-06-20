using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] protected AttackCircle attackCircle = null; //attackCircle 매니저 구현 후 할당 필요

    protected Animator animator = null;
    protected CharacterController characterController = null;
    protected Movement3D movement3D = null;
    protected CharacterStat characterStat = null;
    protected NavMeshAgent navMeshAgent = null;
   
    protected HpBar hpBar = null;
    
    protected float attackRange = 2f;
    protected float attackRadius = 1f;
    [SerializeField] protected float attackDamage = 30f;
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
        characterController = GetComponent<CharacterController>();
    }

    protected virtual void Start()
    {
        hpBar.SetMaxHp(characterStat.GetMaxHp());
        hpBar.UpdateCurrentHp(characterStat.GetCurrentHp());
        
        characterStat.onCurrentHpChanged += hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpZero += SetDead;

        attackCircle.UpdateRadius(characterStat.GetAttackRadius());
        characterStat.onAttackRadiusChanged += attackCircle.UpdateRadius;
        attackCircle.onDetectEnemy += UpdateEnemyList;
        attackCircle.onUnDetectEnemy += OnUnDetectEnemy;
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
        attackCircle.gameObject.SetActive(false);
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
        GameObject target = GetTarget();
        if (target == gameObject)
        {
            StopAllCoroutines();
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            return;
        }

        animator.SetFloat(AnimLocalize.moveSpeed, navMeshAgent.speed);
        navMeshAgent.SetDestination(target.transform.position);

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            isAttacking = true;
            StartCoroutine(CoAttack(target));
        }
    }

    protected GameObject GetTarget()
    {
        if (DetectedEnemies.Count > 0)
        {
            GameObject target = DetectedEnemies.Find(e => !e.GetComponent<CharacterBase>().isDead);
            if (target != null)
            {
                return target;
            }
        }
            
        return gameObject;
    }

    protected virtual void Attack(GameObject target)
    {
        animator.SetBool(AnimLocalize.contactEnemy, true);
        transform.LookAt(target.transform.position);
        if (target.TryGetComponent<CharacterBase>(out CharacterBase targetObj))
        {
            targetObj.TakeDamage(attackDamage);
        }
            
    }

    protected virtual IEnumerator CoAttack(GameObject target)
    {
        animator.SetBool(AnimLocalize.contactEnemy, true);
        transform.LookAt(target.transform.position);
        while (true)
        {
            yield return attackTerm;
            if (characterController.enabled) //캐릭터 상태로 판단하도록 변경하기
            {
                animator.SetBool(AnimLocalize.contactEnemy, false);
                isAttacking = false;
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
        Debug.Log($"target.name {target.name}");
        if(DetectedEnemies.Contains(target))
        {
            Debug.Log($"{gameObject.name} : {target.name}");
            DetectedEnemies.Remove(target);
        }
    }

    protected virtual void MoveAttackCircle()
    {
        attackCircle.MoveAttackCircle(transform.position);
    }
}
