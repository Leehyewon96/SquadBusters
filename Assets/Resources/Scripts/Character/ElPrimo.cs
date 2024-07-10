using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ElPrimo : CharacterPlayer
{
    private int killCount = 0;
    [SerializeField] private int FlyingElbowAttackValue = 2;
    [SerializeField] protected float knockBackTime = 3f;
    [SerializeField] protected float knockBackDistance = 4f;

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
        StopAllCoroutines();

        isAttacking = true;
        navMeshAgent.enabled = false;
        characterController.enabled = false;
        SetCharacterState(CharacterState.Skilled);
        animator.SetTrigger(AnimLocalize.flyingElbow);

        StartCoroutine(CoFlyingElbowAttack(target));
    }

    private IEnumerator CoFlyingElbowAttack(GameObject target) 
    {
        transform.LookAt(target.transform.position);
        target.transform.LookAt(gameObject.transform.position);
        if(target.TryGetComponent<CharacterBase>(out CharacterBase targetBase))
        {
            targetBase.SetCharacterState(CharacterState.KnockBack);
        }

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

        targetBase.KnockBack(attackDamage * 2f, knockBackTime, knockBackDistance);
        OnUnDetectEnemy(targetBase);
        SetCharacterState(CharacterState.Idle);
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
