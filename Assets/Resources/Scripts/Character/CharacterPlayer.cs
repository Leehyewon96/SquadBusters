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

    protected virtual void OnEnable()
    {
        if(photonView.IsMine)
        {
            GameManager.Instance.uiManager.fastMoveUI.onMoveFast -= () => movement3D.UpdateMoveSpeed(30f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveFast += () => movement3D.UpdateMoveSpeed(30f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon -= () => movement3D.UpdateMoveSpeed(15f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon += () => movement3D.UpdateMoveSpeed(15f);
            GameManager.Instance.effectManager.Play(EffectType.StarAura, transform.position);
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

        if (characterState == CharacterState.Skilled)
        {
            return;
        }

        if (characterState == CharacterState.KnockBack)
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
            ResetPath();
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
        animator.SetBool(AnimLocalize.contactEnemy, true);
        AnimationClip clip = animatorController.animationClips.ToList().Find(anim => anim.name.Equals(AnimLocalize.attack));
        attackTerm = new WaitForSecondsRealtime(clip.length);

        while (true)
        {
            transform.LookAt(target.transform.position);

            if (characterController.enabled) //캐릭터 상태로 판단하도록 변경하기
            {
                isAttacking = false;
                yield break;
            }
            if (target.TryGetComponent<CharacterBase>(out CharacterBase targetObj))
            {
                photonView.RPC("RPCEffect", RpcTarget.AllBuffered, (int)EffectType.StoneSlash, transform.position);
                targetObj.TakeDamage(attackDamage);

                if (targetObj.isDead)
                {
                    OnTargetDead(target);
                    yield break;
                }
            }

            yield return attackTerm;
        }
    }

    [PunRPC]
    public void RPCEffect(int type, Vector3 pos)
    {
        GameManager.Instance.effectManager.Play((EffectType)type, pos);
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
        GameManager.Instance.effectManager.Play(EffectType.StarAura, transform.position);
        
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
}
