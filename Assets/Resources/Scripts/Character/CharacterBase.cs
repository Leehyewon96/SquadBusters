using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterType
{
    ElPrimo,
    Greg,
    Colt,
    PlayerEnd,

    Eggy,
    Chilli,
    Kiwi,
    BabyDragon,
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
    protected bool isAttacking = false; // CharacterStat�ȿ� �־�� �ǳ�?
    public bool isDead { get; protected set; } = false;

    protected List<GameObject> DetectedEnemies = new List<GameObject>();
    [SerializeField] protected CharacterType characterType = CharacterType.ElPrimo;
    [SerializeField] protected GameObject hpBarOrigin = null;
    [SerializeField] public GameObject attackCircleOrigin = null;

    protected PhotonView photonView = null;
    protected Vector3 destinationPos;

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
        Init();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        if(GameManager.Instance.endGame)
        {
            return;
        }
        //attackCircle.MoveAttackCircle(transform.position);
        hpBar.UpdatePos(transform.position + new Vector3(0, characterController.height + 0.4f, 0));

        //������Ʈ�� �̺�Ʈ �ϳ� ���� �� ��ӹ޴� Ŭ�������� Start���� �� ���.
        //���⼭ ������Ʈ�� �̺�Ʈ ��� Invoke�ϱ�(��ӹ޴� Ŭ�������� Update �ۼ� X)
    }

    public virtual void Init()
    {
        InitCharacterStat();
        hpBar.SetMaxHp(characterStat.GetMaxHp());
        hpBar.UpdateCurrentHp(characterStat.GetCurrentHp());
        if(!photonView.IsMine)
        {
            hpBar.InitColor(Color.red);
        }

        characterStat.onCurrentHpChanged -= hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpChanged += hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpZero -= SetDead;
        characterStat.onCurrentHpZero += SetDead;

        if (!photonView.IsMine)
        {
            return;
        }
    }

    protected virtual void InitCharacterStat()
    {
        Dictionary<string, object> stat = CSVReader.Read("CharacterStat").Find(s => s["name"].ToString().Equals(characterType.ToString()));
        if(stat == null)
        {
            Debug.Log($"{characterType.ToString()} ����");
        }
        else
            characterStat.Init(float.Parse(stat.GetValueOrDefault("maxHp").ToString()), float.Parse(stat.GetValueOrDefault("attackDamage").ToString()), int.Parse(stat.GetValueOrDefault("coin").ToString()), int.Parse(stat.GetValueOrDefault("gem").ToString()));
        //Debug.Log($"[{gameObject.name}] : maxHp : {characterStat.GetMaxHp()}");
    }

    public virtual void SetCharacterState(CharacterState newState)
    {
        photonView.RPC("RPCSetCharacterState", RpcTarget.AllBuffered, newState);
    }

    public virtual void PlayStunAnimation()
    {
        animator.SetTrigger(AnimLocalize.knockBack);
    }

    [PunRPC]
    public virtual void RPCSetCharacterState(CharacterState newState)
    {
        characterState = newState;
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

    public virtual void SetDestinationPos(Vector3 destination)
    {
        destinationPos = destination;
    }

    public virtual void SetDestination(Vector3 destination)
    {
        if(navMeshAgent == null || !navMeshAgent.enabled)
        {
            return;
        }

        navMeshAgent.SetDestination(destination);
        navMeshAgent.isStopped = false;
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
    }

    public virtual void ResetPath()
    {
        if (navMeshAgent == null || !navMeshAgent.enabled)
        {
            return;
        }
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
    }

    public virtual void SetSpeed(float inSpeed)
    {
        if (!navMeshAgent.enabled)
        {
            return;
        }
        navMeshAgent.speed = inSpeed;
    }

    protected virtual GameObject GetTarget()
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

    protected virtual void ForwardToEnemy(GameObject target, TweenCallback action = null)
    {
        Vector3 dirVec = target.transform.position - transform.position;
        float angle = Quaternion.FromToRotation(transform.forward, dirVec).eulerAngles.y;
        angle += Quaternion.FromToRotation(Vector3.forward, transform.forward).eulerAngles.y;
        dirVec = Vector3.up * angle;
        transform.DORotate(dirVec, 0.5f).OnComplete(action);
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

}
