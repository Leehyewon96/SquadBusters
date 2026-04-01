using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPlayer : CharacterBase, ICharacterPlayerItemInterface
{
    private List<Action> takeItemActions = new List<Action>();

    public Action<int> updateCoin;
    public Func<int> totalCoin;

    protected EffectType attackEffectType;

    private Action cachedMoveFast;
    private Action cachedMoveCommon;

    protected virtual void OnEnable()
    {
        if (photonView.IsMine)
        {
            cachedMoveFast = () => movement3D.UpdateMoveSpeed(15f);
            cachedMoveCommon = () => movement3D.UpdateMoveSpeed(7.5f);

            GameManager.Instance.uiManager.fastMoveUI.onMoveFast += cachedMoveFast;
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon += cachedMoveCommon;
            GameManager.Instance.effectManager.Play(EffectType.StarAura, transform.position, transform.forward);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (photonView.IsMine && GameManager.Instance != null)
        {
            GameManager.Instance.uiManager.fastMoveUI.onMoveFast -= cachedMoveFast;
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon -= cachedMoveCommon;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (!photonView.IsMine) return;
        if (characterState == CharacterState.InVincible) return;

        if (characterState == CharacterState.Stun)
        {
            GameManager.Instance.soundManager.Stop(SoundEffectType.Walk);
            GameManager.Instance.soundManager.Stop(SoundEffectType.Shot);
            GameManager.Instance.soundManager.Play(SoundEffectType.Stun);
            StopAllCoroutines();
            isAttacking = false;
            DetectedEnemies.Clear();
            animator.SetBool(AnimLocalize.contactEnemy, false);
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            return;
        }

        if (CheckInput())
        {
            StopAllCoroutines();
            isAttacking = false;
            if (coroutineAttack != null)
            {
                StopCoroutine(coroutineAttack);
                coroutineAttack = null;
            }

            animator.SetBool(AnimLocalize.contactEnemy, false);

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Move(x, z);
        }
    }

    protected override void MoveToEnemy(GameObject target)
    {
        if (target == gameObject || target == null)
        {
            StopAllCoroutines();
            animator.SetBool(AnimLocalize.contactEnemy, false);
            isAttacking = false;
            navMeshAgent.enabled = true;
            SetDestination(destinationPos);
            return;
        }

        if (isAttacking) return;

        SetDestination(target.transform.position);

        if (Vector3.Distance(transform.position, target.transform.position) <= navMeshAgent.stoppingDistance)
        {
            ResetPath();
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            Attack(target);
        }
    }

    public override void AIActionByJob(GameObject target)
    {
        if (CheckInput()) return;
        base.AIActionByJob(target);
    }

    protected virtual bool CheckInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (movement3D.moveSpeed == 7.5f)
            {
                GameManager.Instance.soundManager.Stop(SoundEffectType.Run);
                GameManager.Instance.soundManager.Play(SoundEffectType.Walk);
            }
            else
            {
                GameManager.Instance.soundManager.Stop(SoundEffectType.Walk);
                GameManager.Instance.soundManager.Play(SoundEffectType.Run);
            }
            characterController.enabled = true;
            ResetPath();
            navMeshAgent.enabled = false;
            animator.SetFloat(AnimLocalize.moveSpeed, movement3D.moveSpeed);
            return true;
        }

        GameManager.Instance.soundManager.Stop(SoundEffectType.Walk);
        characterController.enabled = false;
        navMeshAgent.enabled = true;
        animator.SetFloat(AnimLocalize.moveSpeed, navMeshAgent.velocity.magnitude);
        return false;
    }

    protected override void Attack(GameObject target)
    {
        if (target == null) return;
        base.Attack(target);
    }

    protected override IEnumerator CoAttack(GameObject target)
    {
        TweenCallback callBack = () =>
        {
            animator.SetBool(AnimLocalize.contactEnemy, true);
            AnimationClip clip = animatorController.animationClips.ToList().Find(anim => anim.name.Equals(AnimLocalize.attack));
            attackTerm = new WaitForSecondsRealtime(clip.length);
        };
        ForwardToEnemy(target, callBack);

        while (true)
        {
            ForwardToEnemy(target);
            if (characterController.enabled || characterState == CharacterState.Stun)
            {
                isAttacking = false;
                yield break;
            }
            yield return attackTerm;

            if (AttackTarget(target))
                yield break;
        }
    }

    protected virtual bool AttackTarget(GameObject target)
    {
        isAttacking = true;

        if (target.TryGetComponent<CharacterBase>(out CharacterBase targetObj))
        {
            photonView.RPC("RPCEffect", RpcTarget.AllBuffered, (int)attackEffectType,
                transform.position + Vector3.up * 1.5f + transform.forward.normalized * 0.5f, transform.forward);

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

    protected override void OnTargetDead(GameObject target)
    {
        base.OnTargetDead(target);
        GameManager.Instance.soundManager.Play(SoundEffectType.Kill);
        DetectedEnemies.Remove(target);
        animator.SetBool(AnimLocalize.contactEnemy, false);
        isAttacking = false;
    }

    public virtual void TakeItem(ItemType itemType)
    {
        Debug.Log($"[{gameObject.name}] itemType : {itemType}");
        photonView.RPC("RPCTakeItem", RpcTarget.AllBuffered, (int)itemType);
    }

    [PunRPC]
    public virtual void RPCTakeItem(int itemType)
    {
        if (photonView.IsMine)
        {
            Debug.Log($"RPCTakeItem [{gameObject.name}] itemType : {itemType}");
            takeItemActions[itemType].Invoke();
        }
    }

    public virtual void AddTakeItemActions(Action onTakeItem)
    {
        if (!takeItemActions.Contains(onTakeItem))
            takeItemActions.Add(onTakeItem);
    }

    public virtual void UpdateTotalCoin(int newCoin) => updateCoin?.Invoke(newCoin);

    public int GetTotalCoin() => totalCoin?.Invoke() ?? 0;

    [PunRPC]
    public override void RPCGetAOE(float inDamage, Vector3 fromPos, float distance)
    {
        if (photonView.IsMine && Camera.main.TryGetComponent<CameraFollow>(out CameraFollow cam))
            cam.onCameraShake?.Invoke(0.5f);

        base.RPCGetAOE(inDamage, fromPos, distance);
    }

    public override void SetDead()
    {
        base.SetDead();
        GameManager.Instance.soundManager.Play(SoundEffectType.Die);
    }
}
