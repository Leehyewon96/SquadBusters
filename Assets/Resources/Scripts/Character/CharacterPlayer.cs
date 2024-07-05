using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPlayer : CharacterBase, ICharacterPlayerItemInterface
{
    public delegate void OnTakeItem();
    public List<OnTakeItem> takeItemActions = new List<OnTakeItem>();

    protected override void Update()
    {
        base.Update();

        if (!photonView.IsMine)
        {
            return;
        }

        if (CheckInput())
        {
            StopAllCoroutines();
            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;

            isAttacking = false;
            DetectedEnemies.Clear();
            animator.SetBool(AnimLocalize.contactEnemy, false);

            Move();
            animator.SetFloat(AnimLocalize.moveSpeed, characterController.velocity.magnitude);
        }
        else
        {
            animator.SetFloat(AnimLocalize.moveSpeed, 0);
            //if (!isAttacking)
            //{
            //    MoveToEnemy();
            //}
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

    //public override void UpdateEnemyList(CharacterBase target)
    //{
    //    Debug.Log($"player UpdateEnemyList");
    //    Debug.Log($"DetectedEnemies {DetectedEnemies.Count} / characterStat.GetAttackRadius() {characterStat.GetAttackRadius()} / 거리 : {Vector3.Distance(target.transform.position, transform.position)}");
    //    if (!DetectedEnemies.Contains(target.gameObject)
    //        && Vector3.Distance(target.transform.position, transform.position) <= characterStat.GetAttackRadius())
    //    {
    //        Debug.Log($"player가 {target.gameObject.name} 추가 ");
    //        DetectedEnemies.Add(target.gameObject);
    //    }
    //}

    protected override void Attack(GameObject target)
    {
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
            yield return attackTerm;

            transform.LookAt(target.transform.position);

            if (characterController.enabled) //캐릭터 상태로 판단하도록 변경하기
            {
                animator.SetBool(AnimLocalize.contactEnemy, false);
                isAttacking = false;
                yield break;
            }
            if (target.TryGetComponent<CharacterBase>(out CharacterBase targetObj))
            {
                GameManager.Instance.effectManager.StoneHit(target.transform.position);
                targetObj.TakeDamage(attackDamage);

                if (targetObj.isDead)
                {
                    DetectedEnemies.Remove(target);
                    animator.SetBool(AnimLocalize.contactEnemy, false);
                    isAttacking = false;
                    yield break;
                }
            }
        }
    }

    public virtual void TakeItem(ItemType itemType)
    {
        takeItemActions[(int)itemType].DynamicInvoke();
    }

    public virtual void GainTreasureBox()
    {
        GameManager.Instance.uiManager.ShowUI(UIType.SelectCharacter);
    }
}
