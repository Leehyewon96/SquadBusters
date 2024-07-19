using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static CharacterPlayer;

public enum CharacterType
{
    ElPrimo,
    Babarian,

    Eggy,
    Chilli,
    Kiwi,
    Usurper,
    Golem,

    End,
}

public enum CharacterLevel
{
    NPC,
    Classic,
    Super,
    Ultra,

    End,
}

public enum CharacterState
{
    Idle,
    Attack,
    Dead,
    Stun,
    InVincible,
}

public class CharacterBase : MonoBehaviour, ICharacterProjectileInterface
{
    //protected AttackCircle attackCircle = null;
    protected HpBar hpBar = null;

    protected Animator animator = null;
    protected RuntimeAnimatorController animatorController = null;
    protected Movement3D movement3D = null;
    protected CharacterStat characterStat = null;
    protected NavMeshAgent navMeshAgent = null;
    protected CharacterController characterController = null;

    protected WaitForSecondsRealtime attackTerm = new WaitForSecondsRealtime(0.933f);

    protected CharacterState characterState = CharacterState.Idle;
    protected CharacterLevel characterLevel = CharacterLevel.Classic;
    protected bool isAttacking = false; // CharacterStat안에 있어야 되나?
    public bool isDead { get; protected set; } = false;

    protected List<GameObject> DetectedEnemies = new List<GameObject>();
    [SerializeField] protected CharacterType characterType = CharacterType.ElPrimo;
    [SerializeField] protected GameObject hpBarOrigin = null;
    [SerializeField] public GameObject attackCircleOrigin = null;

    protected PhotonView photonView = null;

    public delegate void DeadAction(CharacterBase characterBase);
    public DeadAction deadAction = null;

    public delegate void OnStun(float duration);
    public OnStun onStun;

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
        hpBar.UpdatePos(transform.position + new Vector3(0, characterController.height + 0.4f, 0));

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
    }

    public virtual void SetCharacterType(CharacterType type)
    {
        characterType = type;
    }

    public virtual void SetCharacterState(CharacterState newState)
    {
        photonView.RPC("RPCSetCharacterState", RpcTarget.AllBuffered, newState);
    }

    [PunRPC]
    public virtual void RPCSetCharacterState(CharacterState newState)
    {
        if(photonView.IsMine)
        {
            characterState = newState;
        }
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

        if(characterStat.CheckDead())
        {
            if(photonView.IsMine)
            {
                if (deadAction != null)
                {
                    deadAction.Invoke(this);
                }
            }
        }
        GameManager.Instance.effectManager.Play(EffectType.StonesHit, gameObject.transform.position, transform.forward);
    }

    public virtual void Merged()
    {
        isDead = true;
        hpBar.UPdateIsUsed(false);
        hpBar.SetActive(false);
        GameManager.Instance.effectManager.Play(EffectType.Explosion, transform.position, transform.forward);

        gameObject.SetActive(false);
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
        GameManager.Instance.effectManager.Play(EffectType.Explosion, transform.position, transform.forward);

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
        if(navMeshAgent == null || !navMeshAgent.enabled)
        {
            return;
        }
        navMeshAgent.SetDestination(destination);
    }

    public virtual void ResetPath()
    {
        if (navMeshAgent == null || !navMeshAgent.enabled)
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
            GameObject target = DetectedEnemies.Find(e => e != null && !e.GetComponent<CharacterBase>().isDead);
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

    public virtual void KnockBack(float inDamage, float inKnockBackTime, float inKnockBackDis)
    {
        photonView.RPC("RPCKnockBack", RpcTarget.AllBuffered, inDamage, inKnockBackTime, inKnockBackDis);
    }

    [PunRPC]
    public virtual void RPCKnockBack(float inDamage, float inKnockBackTime, float inKnockBackDis)
    {
        if (photonView.IsMine)
        {
            navMeshAgent.enabled = false;
            characterController.enabled = false;
            animator.SetTrigger(AnimLocalize.knockBack);

            Vector3 destination = transform.position - transform.forward.normalized * inKnockBackDis;
            Vector3[] path = { transform.position, destination };
            TakeDamage(inDamage);
            transform.DOPath(path, inKnockBackTime, PathType.CatmullRom, PathMode.Full3D).OnComplete(() =>
            {
                navMeshAgent.enabled = true;
                characterController.enabled = true;
                characterState = CharacterState.Idle;
            });
        }
    }

    public virtual int GetCoin()
    {
        return characterStat.GetCoin();
    }

    public virtual int GetGem()
    {
        return characterStat.GetGem();
    }

    public virtual void GetAOE(float inDamage, Vector3 fromPos, float distance)
    {
        photonView.RPC("RPCGetAOE", RpcTarget.AllBuffered, inDamage, fromPos, distance);
    }

    [PunRPC]
    public virtual void RPCGetAOE(float inDamage, Vector3 fromPos, float distance)
    {
        characterState = CharacterState.Stun;
        fromPos.y = transform.position.y;
        Vector3 dir = transform.position - fromPos;
        Vector3 startPos = transform.position;
        Vector3 endPoint = startPos + dir.normalized * distance;
        Vector3 midPoint = startPos + (endPoint - transform.position) * 0.5f;
        midPoint.y += 2f;
        endPoint.y = 2.6f;
        Vector3[] paths = { startPos, midPoint, endPoint };
        transform.DOPath(paths, 0.5f, PathType.CatmullRom, PathMode.Full3D).OnComplete(() =>
        {
            TakeDamage(inDamage);
            characterState = CharacterState.Idle;
        });
    }

    public virtual CharacterLevel GetCharacterLevel()
    {
        return characterLevel;
    }

    public virtual void Stun(float duration, string animName)
    {
        if (onStun != null)
        {
            onStun.Invoke(duration);
        }
        StartCoroutine(CoStun(duration, animName));
    }

    private IEnumerator CoStun(float duration, string animName)
    {
        SetCharacterState(CharacterState.Stun);
        animator.SetTrigger(animName);
        yield return new WaitForSeconds(duration);
        SetCharacterState(CharacterState.Idle);
    }
}
