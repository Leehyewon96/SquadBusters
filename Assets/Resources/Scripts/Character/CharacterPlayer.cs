using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPlayer : CharacterBase, ICharacterPlayerItemInterface
{
    public delegate void OnTakeItem();
    private List<OnTakeItem> takeItemActions = new List<OnTakeItem>();

    protected virtual void OnEnable()
    {
        StartCoroutine(CoEffect());
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
            ResetPath();
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
            return true;
        }

        characterController.enabled = false;
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
                animator.SetBool(AnimLocalize.contactEnemy, false);
                isAttacking = false;
                yield break;
            }
            if (target.TryGetComponent<CharacterBase>(out CharacterBase targetObj))
            {
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

    protected virtual void OnTargetDead(GameObject target)
    {
        DetectedEnemies.Remove(target);
        animator.SetBool(AnimLocalize.contactEnemy, false);
        isAttacking = false;
    }

    public virtual void TakeItem(ItemType itemType)
    {
        if (photonView.IsMine)
        {
            takeItemActions[(int)itemType].DynamicInvoke();
        }
    }

    public virtual void AddTakeItemActions(OnTakeItem onTakeItem)
    {
        takeItemActions.Add(onTakeItem);
    }

    public virtual void GainTreasureBox()
    {
        GameManager.Instance.uiManager.ShowUI(UIType.SelectCharacter);
    }

    protected IEnumerator CoEffect()
    {
        yield return new WaitUntil(() => GameManager.Instance.effectManager != null);
        GameManager.Instance.effectManager.Play(EffectType.StarAura, transform.position);
    }
}
