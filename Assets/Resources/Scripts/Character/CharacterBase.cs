using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterType
{
    ElPrimo,
    ElPrimo2,
    Babarian,
    Player,

    Eggy,
    Chilli,
    Kiwi,
}

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
    [SerializeField] protected CharacterType characterType = CharacterType.ElPrimo;
    [SerializeField] protected GameObject hpBarOrigin = null;
    [SerializeField] public GameObject attackCircleOrigin = null;

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

        characterStat.onCurrentHpChanged -= hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpChanged += hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpZero -= SetDead;
        characterStat.onCurrentHpZero += SetDead;

        attackCircle.onDetectEnemy -= UpdateEnemyList;
        attackCircle.onDetectEnemy += UpdateEnemyList;
        attackCircle.onUnDetectEnemy -= OnUnDetectEnemy;
        attackCircle.onUnDetectEnemy += OnUnDetectEnemy;
        attackCircle.SetCoin(characterStat.coin);
        attackCircle.SetGem(characterStat.gem);
    }

    protected virtual void Update()
    {
        //attackCircle.MoveAttackCircle(transform.position);
        hpBar.UpdatePos(transform.position + new Vector3(0, characterController.height + 0.5f, 0));
        //������Ʈ�� �̺�Ʈ �ϳ� ���� �� ��ӹ޴� Ŭ�������� Start���� �� ���.
        //���⼭ ������Ʈ�� �̺�Ʈ ��� Invoke�ϱ�(��ӹ޴� Ŭ�������� Update �ۼ� X)

        if (GetComponent<PhotonView>().IsMine)
        {
            return;
        }
    }

    public virtual void Init()
    {
        hpBar.SetMaxHp(characterStat.GetMaxHp());
        hpBar.UpdateCurrentHp(characterStat.GetCurrentHp());

        characterStat.onCurrentHpChanged -= hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpChanged += hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpZero -= SetDead;
        characterStat.onCurrentHpZero += SetDead;

        attackCircle.onDetectEnemy -= UpdateEnemyList;
        attackCircle.onDetectEnemy += UpdateEnemyList;
        attackCircle.onUnDetectEnemy -= OnUnDetectEnemy;
        attackCircle.onUnDetectEnemy += OnUnDetectEnemy;
        attackCircle.SetCoin(characterStat.coin);
        attackCircle.SetGem(characterStat.gem);
    }

    public virtual void SetCharacterType(CharacterType type)
    {
        characterType = type;
    }

    public virtual CharacterType GetCharacterType()
    {
        return characterType;
    }

    public virtual void TakeDamage(float inDamage)
    {
        if(isDead)
        {
            return;
        }
        characterStat.ApplyDamage(inDamage);
    }

    public virtual void SetDead()
    {
        isDead = true;
        hpBar.UPdateIsUsed(false);
        hpBar.SetActive(false);
        attackCircle.RemoveOwner(this);
        gameObject.SetActive(false);
    }

    protected virtual void UpdateEnemyList(CharacterBase target)
    {
        if (!DetectedEnemies.Contains(target.gameObject))
        {
            DetectedEnemies.Add(target.gameObject);
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

            if(Vector3.Distance(transform.position, attackCircle.transform.position) <= navMeshAgent.stoppingDistance)
            {
                navMeshAgent.ResetPath();
            }
            else
            {
                navMeshAgent.SetDestination(attackCircle.transform.position);
            }
            
            return;
        }

        animator.SetFloat(AnimLocalize.moveSpeed, navMeshAgent.speed);
        navMeshAgent.SetDestination(target.transform.position);
        //navMeshAgent.stoppingDistance = 1.5f;

        if (Vector3.Distance(transform.position, target.transform.position) <= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.ResetPath();
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            isAttacking = true;
            Attack(target);
        }
    }

    protected virtual void Attack(GameObject target)
    {

    }

    protected virtual void OnUnDetectEnemy(CharacterBase target)
    {
        if(DetectedEnemies.Contains(target.gameObject))
        {
            DetectedEnemies.Remove(target.gameObject);
        }
    }
}
