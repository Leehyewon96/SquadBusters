using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class CharacterPlayer : CharacterBase, ICharacterPlayerItemInterface, ICharacterPlayerProjectileInterface
{
    public delegate void OnTakeItem();
    private List<OnTakeItem> takeItemActions = new List<OnTakeItem>();

    public delegate void UpdateCoin(int newcoin);
    public UpdateCoin updateCoin;

    public delegate int TotalCoin();
    public TotalCoin totalCoin;

    public delegate void OnStun(float duration);
    public OnStun onStun;

    protected virtual void OnEnable()
    {
        if(photonView.IsMine)
        {
            GameManager.Instance.uiManager.fastMoveUI.onMoveFast -= () => movement3D.UpdateMoveSpeed(30f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveFast += () => movement3D.UpdateMoveSpeed(30f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon -= () => movement3D.UpdateMoveSpeed(15f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon += () => movement3D.UpdateMoveSpeed(15f);
            GameManager.Instance.effectManager.Play(EffectType.StarAura, transform.position, transform.forward);
        }
        
        //StartCoroutine(CoInit());
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
            DetectedEnemies.Clear();
            animator.SetBool(AnimLocalize.contactEnemy, false);

            Move();
            animator.SetFloat(AnimLocalize.moveSpeed, characterController.velocity.magnitude);
        }
        else
        {
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            
            MoveToEnemy();
        }
    }

    protected virtual void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        movement3D.Move(x, z);
    }

    public void PlayAnim()
    {
        animator.SetFloat(AnimLocalize.moveSpeed, characterController.velocity.magnitude);
    }

    protected virtual bool CheckInput()
    {
        //모바일에서 터치로 변경
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            characterController.enabled = true;
            navMeshAgent.enabled = false;
            return true;
        }

        characterController.enabled = false;
        navMeshAgent.enabled = true;
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
        //transform.LookAt(target.transform.position);
        Vector3 dirVec = target.transform.position - transform.position;
        dirVec.Normalize();
        float RotSpeed = 10f;
        RaycastHit hit;
        while (true)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 1.2f, transform.forward, out hit))
            {
                if (hit.transform.root.gameObject == target)
                {
                    break;
                }
            }

            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirVec), RotSpeed * Time.deltaTime);

            dirVec = target.transform.position - transform.position;
        }


        animator.SetBool(AnimLocalize.contactEnemy, true);
        AnimationClip clip = animatorController.animationClips.ToList().Find(anim => anim.name.Equals(AnimLocalize.attack));
        attackTerm = new WaitForSecondsRealtime(clip.length);

        while (true)
        {
            //transform.LookAt(target.transform.position);
            while (true)
            {
                if (Physics.Raycast(transform.position + Vector3.up * 1.2f, transform.forward.normalized, out hit))
                {
                    if (hit.transform.root.gameObject == target)
                    {
                        break;
                    }
                }

                yield return new WaitForFixedUpdate();
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirVec), RotSpeed * Time.deltaTime);

                dirVec = target.transform.position - transform.position;
            }

            if (characterController.enabled) //캐릭터 상태로 판단하도록 변경하기
            {
                isAttacking = false;
                yield break;
            }
            yield return attackTerm;

            if (target.TryGetComponent<CharacterBase>(out CharacterBase targetObj))
            {
                photonView.RPC("RPCEffect", RpcTarget.AllBuffered, (int)EffectType.ChargeSlashPurple, transform.position + Vector3.up * 1.5f + transform.forward.normalized * 0.5f, transform.forward);
                targetObj.TakeDamage(attackDamage);

                if (targetObj.isDead)
                {
                    OnTargetDead(target);
                    yield break;
                }
            }
        }
    }

    protected IEnumerator CoRotate(GameObject target)
    {
        Vector3 dirVec = target.transform.position - transform.position;
        dirVec.Normalize();
        float RotSpeed = 10f;
        RaycastHit hit;
        while (true)
        {
            if (Physics.Raycast(transform.position, transform.forward.normalized, out hit))
            {
                if(hit.transform.gameObject == target)
                {
                    break;
                }
            }

            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirVec), RotSpeed * Time.deltaTime);

            dirVec = target.transform.position - transform.position;
        }
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
        photonView.RPC("RPCTakeItem", RpcTarget.AllBuffered, (int)itemType);
    }

    [PunRPC]
    public virtual void RPCTakeItem(int itemType)
    {
        if (photonView.IsMine)
        {
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

    public virtual void Stun(float duration)
    {
        if (onStun != null)
        {
            onStun.Invoke(duration);
        }
        StartCoroutine(CoStun(duration));

        //photonView.RPC("RPCStun", RpcTarget.AllBuffered, duration);
    }

    [PunRPC]
    public virtual void RPCStun(float duration)
    {
        if(photonView.IsMine)
        {
            if (onStun != null)
            {
                onStun.Invoke(duration);
            }
            StartCoroutine(CoStun(duration));
        }
    }

    private IEnumerator CoStun(float duration)
    {
        SetCharacterState(CharacterState.Stun);
        animator.SetTrigger(AnimLocalize.knockBack);
        yield return new WaitForSeconds(duration);
        SetCharacterState(CharacterState.Idle);
    }
}
