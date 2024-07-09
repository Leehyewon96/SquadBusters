using DG.Tweening;
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

public enum CharacterState
{
    Idle,
    Attack,
    Dead,
    KnockBack,
    Skilled,
}

public class CharacterBase : MonoBehaviour
{
    //protected AttackCircle attackCircle = null;
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

    protected CharacterState characterState = CharacterState.Idle;
    protected bool isAttacking = false; // CharacterStat안에 있어야 되나?
    public bool isDead { get; protected set; } = false;

    protected List<GameObject> DetectedEnemies = new List<GameObject>();
    [SerializeField] protected CharacterType characterType = CharacterType.ElPrimo;
    [SerializeField] protected GameObject hpBarOrigin = null;
    [SerializeField] public GameObject attackCircleOrigin = null;

    protected PhotonView photonView = null;

    public delegate void DeadAction(CharacterBase characterBase);
    public DeadAction deadAction = null;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        animatorController = animator.runtimeAnimatorController;
        characterStat = GetComponent<CharacterStat>();
        movement3D = GetComponent<Movement3D>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        hpBar = GetComponentInChildren<HpBar>();
        photonView = GetComponent<PhotonView>();
    }

    protected virtual void Start()
    {
        Init();
        
    }

    protected virtual void Update()
    {
        //attackCircle.MoveAttackCircle(transform.position);
        hpBar.UpdatePos(transform.position + new Vector3(0, characterController.height + 0.5f, 0));

        //업데이트용 이벤트 하나 생성 후 상속받는 클래스에서 Start에서 다 등록.
        //여기서 업데이트용 이벤트 계속 Invoke하기(상속받는 클래스에는 Update 작성 X)
    }

    public virtual void Init()
    {
        hpBar.SetMaxHp(characterStat.GetMaxHp());
        hpBar.UpdateCurrentHp(characterStat.GetCurrentHp());

        characterStat.onCurrentHpChanged -= hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpChanged += hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpZero -= SetDead;
        characterStat.onCurrentHpZero += SetDead;

        if (!photonView.IsMine)
        {
            return;
        }
        //attackCircle.onDetectEnemy -= UpdateEnemyList;
        //attackCircle.onDetectEnemy += UpdateEnemyList;
        //attackCircle.onUnDetectEnemy -= OnUnDetectEnemy;
        //attackCircle.onUnDetectEnemy += OnUnDetectEnemy;
        ////attackCircle.SetCoin(characterStat.coin);
        //attackCircle.SetGem(characterStat.gem);
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
        photonView.RPC("RPCTakeDamage", RpcTarget.AllBuffered, inDamage);
    }

    [PunRPC]
    public virtual void RPCTakeDamage(float inDamage)
    {
        if (isDead)
        {
            return;
        }
        characterStat.ApplyDamage(inDamage);
        GameManager.Instance.effectManager.Play(EffectType.StoneHit, gameObject.transform.position);
    }

    
    public virtual void SetDead()
    {
        photonView.RPC("RPCSetDead", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public virtual void RPCSetDead()
    {
        isDead = true;
        hpBar.UPdateIsUsed(false);
        hpBar.SetActive(false);
        if(deadAction != null)
        {
            deadAction.Invoke(this);
        }
        GameManager.Instance.effectManager.Play(EffectType.Explosion, transform.position);
        gameObject.SetActive(false);
    }

    public virtual void OnDetectEnemy(CharacterBase target)
    {
        if (!DetectedEnemies.Contains(target.gameObject))
        {
            DetectedEnemies.Add(target.gameObject);
        }
    }

    public virtual void SetDestination(Vector3 destination)
    {
        if(!navMeshAgent.enabled)
        {
            return;
        }
        navMeshAgent.SetDestination(destination);
    }

    public virtual void ResetPath()
    {
        if (!navMeshAgent.enabled)
        {
            return;
        }
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;
    }

    public virtual void SetSpeed(float inSpeed)
    {
        if (!navMeshAgent.enabled)
        {
            return;
        }
        navMeshAgent.speed = inSpeed;
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
        animator.SetFloat(AnimLocalize.moveSpeed, navMeshAgent.velocity.magnitude);

        GameObject target = GetTarget();
        if (target == gameObject)
        {
            StopAllCoroutines();
            animator.SetBool(AnimLocalize.contactEnemy, false);
            isAttacking = false;
            navMeshAgent.enabled = true;
            return;
        }

        if(isAttacking)
        {
            return;
        }

        SetDestination(target.transform.position);

        if (Vector3.Distance(transform.position, target.transform.position) <= navMeshAgent.stoppingDistance)
        {
            ResetPath();
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            Attack(target);
        }
    }

    protected virtual void Attack(GameObject target)
    {
        navMeshAgent.enabled = false;
    }

    public virtual void OnUnDetectEnemy(CharacterBase target)
    {
        if(DetectedEnemies.Contains(target.gameObject))
        {
            DetectedEnemies.Remove(target.gameObject);
        }
    }

    public virtual void KnockBack(float inDamage)
    {
        navMeshAgent.enabled = false;
        characterController.enabled = false;
        characterState = CharacterState.KnockBack;
        Vector3 destination = transform.position + transform.forward.normalized * 3f;
        Vector3[] path = { transform.position, destination };
        transform.DOPath(path, 3f, PathType.CatmullRom, PathMode.Full3D).OnComplete(() =>
        {
            navMeshAgent.enabled = true;
            characterController.enabled = true;
            characterState = CharacterState.Idle;
            TakeDamage(inDamage);
        });
    }

}
