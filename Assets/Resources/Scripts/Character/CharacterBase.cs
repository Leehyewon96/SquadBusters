using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] protected AttackCircle attackCircle = null; //���� �Ŵ������� attackCircle �Ŵ��� ���� �� �Ҵ� �ʿ�

    protected Animator animator = null;
    
    protected Movement3D movement3D = null;
    protected CharacterStat characterStat = null;
    protected NavMeshAgent navMeshAgent = null;
   
    protected HpBar hpBar = null;
    
    protected float attackRange = 2f;
    protected float attackRadius = 1f;
    [SerializeField] protected float attackDamage = 30f;
    protected WaitForSecondsRealtime attackTerm = new WaitForSecondsRealtime(1.5f);
    protected bool isAttacking = false; // CharacterStat�ȿ� �־�� �ǳ�?
    public bool isDead { get; protected set; } = false;

    protected List<GameObject> DetectedEnemies = new List<GameObject>();


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        characterStat = GetComponent<CharacterStat>();
        hpBar = GetComponentInChildren<HpBar>();
        movement3D = GetComponent<Movement3D>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        //attackCircle = 
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
        //������Ʈ�� �̺�Ʈ �ϳ� ���� �� ��ӹ޴� Ŭ�������� Start���� �� ���.
        //���⼭ ������Ʈ�� �̺�Ʈ ��� Invoke�ϱ�(��ӹ޴� Ŭ�������� Update �ۼ� X)
    }

    public virtual void TakeDamage(float inDamage)
    {
        characterStat.ApplyDamage(inDamage);
    }

    protected virtual void SetDead()
    {
        //���� ������Ʈ �ڸ��� ���� ����
        isDead = true;
        attackCircle.SetActive(false);
        gameObject.SetActive(false);
    }

    protected virtual void UpdateEnemyList(GameObject target)
    {
        if(!DetectedEnemies.Contains(target))
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
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
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
            Debug.Log($"{gameObject.name} : {target.name}");
            DetectedEnemies.Remove(target);
        }
    }

    protected virtual void MoveAttackCircle()
    {
        attackCircle.MoveAttackCircle(transform.position);
    }
}
