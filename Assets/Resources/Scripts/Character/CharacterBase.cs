using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBase : MonoBehaviour
{
    protected AttackCircle attackCircle = null;
    protected HpBar hpBar = null;

    protected Animator animator = null;
    protected Movement3D movement3D = null;
    protected CharacterStat characterStat = null;
    protected NavMeshAgent navMeshAgent = null;
    protected CharacterController characterController = null;

    protected float attackRange = 2f;
    protected float attackRadius = 1f;
    [SerializeField] protected float attackDamage = 30f;
    protected WaitForSecondsRealtime attackTerm = new WaitForSecondsRealtime(1.5f);
    protected bool isAttacking = false; // CharacterStat안에 있어야 되나?
    public bool isDead { get; protected set; } = false;

    protected List<GameObject> DetectedEnemies = new List<GameObject>();


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        characterStat = GetComponent<CharacterStat>();
        movement3D = GetComponent<Movement3D>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        //attackCircle = 
    }

    protected virtual void Start()
    {
        attackCircle = GameManager.Instance.attackCircleManager.GetAttackCircle();

        hpBar.SetMaxHp(characterStat.GetMaxHp());
        hpBar.UpdateCurrentHp(characterStat.GetCurrentHp());
        
        characterStat.onCurrentHpChanged += hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpZero += SetDead;

        attackCircle.UpdateRadius(characterStat.GetAttackRadius());
        characterStat.onAttackRadiusChanged += attackCircle.UpdateRadius;
        attackCircle.onDetectEnemy += UpdateEnemyList;
        attackCircle.onUnDetectEnemy += OnUnDetectEnemy;
        attackCircle.UpdateOwner(gameObject);
        attackCircle.UpdateLayer(LayerMask.LayerToName(gameObject.layer));
    }

    protected virtual void Update()
    {
        attackCircle.MoveAttackCircle(transform.position);
        hpBar.UpdatePos(transform.position + new Vector3(0, characterController.height + 0.5f, 0));
        //업데이트용 이벤트 하나 생성 후 상속받는 클래스에서 Start에서 다 등록.
        //여기서 업데이트용 이벤트 계속 Invoke하기(상속받는 클래스에는 Update 작성 X)
    }

    public virtual void TakeDamage(float inDamage)
    {
        characterStat.ApplyDamage(inDamage);
    }

    protected virtual void SetDead()
    {
        //죽은 오브젝트 자리에 동전 생성
        isDead = true;
        attackCircle.UpdateIsUsed(false);
        attackCircle.SetActive(false);
        hpBar.UPdateIsUsed(false);
        hpBar.SetActive(false);
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
            StopAllCoroutines(); //StopAttack같은 이벤트나 함수로 고치기
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            animator.SetBool(AnimLocalize.contactEnemy, false);
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
