using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBase : MonoBehaviour
{
    protected AttackCircle attackCircle = null;
    protected HpBar hpBar = null;

    protected Animator animator = null;
    protected RuntimeAnimatorController animatorController = null;
    protected Movement3D movement3D = null;
    protected CharacterStat characterStat = null;
    protected NavMeshAgent navMeshAgent = null;
    protected CharacterController characterController = null;

    protected float attackRange = 2f;
    protected float attackRadius = 1f;
    [SerializeField] protected float attackDamage = 30f;
    protected WaitForSecondsRealtime attackTerm = new WaitForSecondsRealtime(0.933f);
    protected bool isAttacking = false; // CharacterStat�ȿ� �־�� �ǳ�?
    public bool isDead { get; protected set; } = false;

    protected List<GameObject> DetectedEnemies = new List<GameObject>();
    public Vector3 offsetPos = Vector3.zero;


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        animatorController = animator.runtimeAnimatorController;
        characterStat = GetComponent<CharacterStat>();
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

        attackCircle.onDetectEnemy += UpdateEnemyList;
        attackCircle.onUnDetectEnemy += OnUnDetectEnemy;
    }

    protected virtual void Update()
    {
        //attackCircle.MoveAttackCircle(transform.position);
        hpBar.UpdatePos(transform.position + new Vector3(0, characterController.height + 0.5f, 0));
        //������Ʈ�� �̺�Ʈ �ϳ� ���� �� ��ӹ޴� Ŭ�������� Start���� �� ���.
        //���⼭ ������Ʈ�� �̺�Ʈ ��� Invoke�ϱ�(��ӹ޴� Ŭ�������� Update �ۼ� X)
    }

    public virtual void TakeDamage(float inDamage)
    {
        characterStat.ApplyDamage(inDamage);
    }

    protected virtual void SetDead()
    {
        isDead = true;
        attackCircle.RemoveOwner(gameObject);
        hpBar.UPdateIsUsed(false);
        hpBar.SetActive(false);

        gameObject.SetActive(false);
    }

    protected virtual void UpdateEnemyList(GameObject target)
    {
        if (!DetectedEnemies.Contains(target))
        {
            DetectedEnemies.Add(target);
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

    protected virtual void MoveToEnemy()
    {
        GameObject target = GetTarget();
        if (target == gameObject)
        {
            StopAllCoroutines(); //StopAttack���� �̺�Ʈ�� �Լ��� ��ġ��
            animator.SetFloat(AnimLocalize.moveSpeed, navMeshAgent.velocity.magnitude);
            animator.SetBool(AnimLocalize.contactEnemy, false);
            navMeshAgent.SetDestination(attackCircle.transform.position + offsetPos);
            return;
        }

        animator.SetFloat(AnimLocalize.moveSpeed, navMeshAgent.speed);
        navMeshAgent.SetDestination(target.transform.position);

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            isAttacking = true;
            Attack(target);
        }
    }

    protected virtual void Attack(GameObject target)
    {

    }

    protected virtual void OnUnDetectEnemy(GameObject target)
    {
        if(DetectedEnemies.Contains(target))
        {
            DetectedEnemies.Remove(target);
        }
    }
}
