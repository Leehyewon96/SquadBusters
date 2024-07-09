using DG.Tweening;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ElPrimo : CharacterPlayer
{
    private int killCount = 0;
    [SerializeField] private int FlyingElbowAttackValue = 2;
    [SerializeField] private float waitTime = 1.7f;

    private bool isFlyingElbowAttackMode = false;
    public Rigidbody body;
    float jumpForce = 100f;

    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<Rigidbody>();
    }

    private void FlyingElbowAttack(GameObject target)
    {
        isAttacking = true;
        navMeshAgent.enabled = false;
        characterController.enabled = false;
        characterState = CharacterState.Skilled;
        animator.SetTrigger(AnimLocalize.flyingElbow);

        StartCoroutine(CoFlyingElbowAttack(target));
    }

    private IEnumerator CoFlyingElbowAttack(GameObject target) 
    {
        transform.LookAt(target.transform.position);
        target.transform.LookAt(gameObject.transform.position);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(AnimLocalize.jump));

        float jumpTime = animatorController.animationClips.ToList().Find(a => a.name.Equals(AnimLocalize.jump)).length;
        float elbowTime = animatorController.animationClips.ToList().Find(a => a.name.Equals(AnimLocalize.elbow)).length;

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.transform.position;
        Vector3 midPos = startPos + ((targetPos - startPos) / 2f) + Vector3.up * 2f;
        Vector3[] jumpPath = { startPos, midPos, targetPos };
        
        body.DOPath(jumpPath, jumpTime, PathType.CatmullRom, PathMode.Full3D);

        yield return new WaitForSeconds(elbowTime);

        isAttacking = false;
        navMeshAgent.enabled = true;
        characterController.enabled = true;
        
        target.GetComponent<CharacterBase>().KnockBack(attackDamage * 2f);
        characterState = CharacterState.Idle;
    }

    protected override void MoveToEnemy()
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

        if (isAttacking)
        {
            return;
        }

        if (killCount >= FlyingElbowAttackValue)
        {
            killCount = 0;
            FlyingElbowAttack(target);
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

    protected override void OnTargetDead(GameObject target)
    {
        base.OnTargetDead(target);
        killCount++;
    }

}
