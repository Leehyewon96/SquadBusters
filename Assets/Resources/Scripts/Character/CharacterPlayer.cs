using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class CharacterPlayer : CharacterBase, ICharacterPlayerItemInterface
{
    public delegate void OnTakeItem();
    private List<OnTakeItem> takeItemActions = new List<OnTakeItem>();

    public delegate void UpdateCoin(int newcoin);
    public UpdateCoin updateCoin;

    public delegate int TotalCoin();
    public TotalCoin totalCoin;

    protected EffectType attackEffectType;

    protected virtual void OnEnable()
    {
        if(photonView.IsMine)
        {
            GameManager.Instance.uiManager.fastMoveUI.onMoveFast -= () => movement3D.UpdateMoveSpeed(15f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveFast += () => movement3D.UpdateMoveSpeed(15f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon -= () => movement3D.UpdateMoveSpeed(7.5f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon += () => movement3D.UpdateMoveSpeed(7.5f);
            GameManager.Instance.effectManager.Play(EffectType.StarAura, transform.position, transform.forward);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (!photonView.IsMine)
        {
            return;
        }

        if (characterState == CharacterState.InVincible)
        {
            return;
        }

        if (characterState == CharacterState.Stun)
        {
            StopAllCoroutines();
            //ResetPath();
            isAttacking = false;
            DetectedEnemies.Clear();
            animator.SetBool(AnimLocalize.contactEnemy, false);
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            return;
        }

        if (CheckInput())
        {
            StopAllCoroutines();
            //ResetPath();
            isAttacking = false;
            //DetectedEnemies.Clear();
            animator.SetBool(AnimLocalize.contactEnemy, false);

            Move();
            //animator.SetFloat(AnimLocalize.moveSpeed, characterController.velocity.magnitude);
        }
        else
        {
            //animator.SetFloat(AnimLocalize.moveSpeed, 0);
            MoveToEnemy();
        }
    }

    protected virtual void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        movement3D.Move(x, z);
    }

    protected override void MoveToEnemy()
    {
        //animator.SetFloat(AnimLocalize.moveSpeed, navMeshAgent.velocity.magnitude);

        GameObject target = GetTarget();
        if (target == gameObject)
        {
            StopAllCoroutines();
            animator.SetBool(AnimLocalize.contactEnemy, false);
            isAttacking = false;
            navMeshAgent.enabled = true;
            SetDestination(destinationPos);
            return;
        }

        if (isAttacking)
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

    protected virtual bool CheckInput()
    {
        //모바일에서 터치로 변경
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            characterController.enabled = true;
            ResetPath();
            navMeshAgent.enabled = false;
            animator.SetFloat(AnimLocalize.moveSpeed, characterController.velocity.magnitude);
            return true;
        }

        characterController.enabled = false;
        navMeshAgent.enabled = true;
        animator.SetFloat(AnimLocalize.moveSpeed, navMeshAgent.velocity.magnitude);
        return false;
    }

    protected override void Attack(GameObject target)
    {
        base.Attack(target);
        StartCoroutine(CoAttack(target));
        isAttacking = true;
    }

    protected virtual IEnumerator CoAttack(GameObject target)
    {
        TweenCallback callBack = null;
        callBack = () =>
        {
            animator.SetBool(AnimLocalize.contactEnemy, true);
            AnimationClip clip = animatorController.animationClips.ToList().Find(anim => anim.name.Equals(AnimLocalize.attack));
            attackTerm = new WaitForSecondsRealtime(clip.length);
        };
        ForwardToEnemy(target, callBack);

        while (true)
        {
            ForwardToEnemy(target);
            if (characterController.enabled || characterState == CharacterState.Stun) //캐릭터 상태로 판단하도록 변경하기
            {
                isAttacking = false;
                yield break;
            }
            yield return attackTerm;

            if(AttackTarget(target))
            {
                yield break;
            }
        }
    }

    protected virtual bool AttackTarget(GameObject target)
    {
        if (target.TryGetComponent<CharacterBase>(out CharacterBase targetObj))
        {
            photonView.RPC("RPCEffect", RpcTarget.AllBuffered, (int)attackEffectType, transform.position + Vector3.up * 1.5f + transform.forward.normalized * 0.5f, transform.forward);
            targetObj.TakeDamage(characterStat.GetAttackDamage());

            if (targetObj.isDead)
            {
                OnTargetDead(target);
                return true;
            }
        }

        return false;
    }

    [PunRPC]
    public void RPCEffect(int type, Vector3 pos, Vector3 rot)
    {
        GameManager.Instance.effectManager.Play((EffectType)type, pos, rot);
    }

    protected virtual void OnTargetDead(GameObject target)
    {
        DetectedEnemies.Remove(target);
        animator.SetBool(AnimLocalize.contactEnemy, false);
        isAttacking = false;
    }

    protected IEnumerator CoInit()
    {
        yield return new WaitUntil(() => GameManager.Instance.effectManager != null);
        GameManager.Instance.effectManager.Play(EffectType.StarAura, transform.position, transform.forward);
        
    }

    public virtual void TakeItem(ItemType itemType)
    {
        Debug.Log($"[{gameObject.name}] itemType : {itemType.ToString()}");
        photonView.RPC("RPCTakeItem", RpcTarget.AllBuffered, (int)itemType);
    }

    [PunRPC]
    public virtual void RPCTakeItem(int itemType)
    {
        if (photonView.IsMine)
        {
            Debug.Log($" RPCTakeItem [{gameObject.name}] itemType : {itemType.ToString()}");
            takeItemActions[itemType].DynamicInvoke();
        }
    }

    public virtual void AddTakeItemActions(OnTakeItem onTakeItem)
    {
        if (!takeItemActions.Contains(onTakeItem))
        {
            takeItemActions.Add(onTakeItem);
        }
    }

    public virtual void UpdateTotalCoin(int newCoin)
    {
        if (updateCoin != null)
        {
            updateCoin.Invoke(newCoin);
        }
    }

    public int GetTotalCoin()
    {
        if(totalCoin != null)
        {
            return totalCoin.Invoke();
        }
        return 0;
    }

    public override void GetAOE(float inDamage, Vector3 fromPos, float distance)
    {
        if (photonView.IsMine && Camera.main.TryGetComponent<CameraFollow>(out CameraFollow cam))
        {
            if (cam.onCameraShake != null)
            {
                cam.onCameraShake.Invoke(0.65f);
            }
        }

        base.GetAOE(inDamage, fromPos, distance);
    }

    //[PunRPC]
    //public override void RPCGetAOE(float inDamage, Vector3 fromPos, float distance)
    //{
    //    if (photonView.IsMine && Camera.main.TryGetComponent<CameraFollow>(out CameraFollow cam))
    //    {
    //        if (cam.onCameraShake != null)
    //        {
    //            cam.onCameraShake.Invoke(0.65f);
    //        }
    //    }

    //    base.RPCGetAOE(inDamage, fromPos, distance);
    //}


}
