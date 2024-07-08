using DG.Tweening;
using System.Linq;
using UnityEngine;

public class ElPrimo : CharacterPlayer
{
    private int killCount = 0;
    [SerializeField] private int FlyingElbowAttackValue = 2;

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
        animator.SetTrigger(AnimLocalize.flyingElbow);

        float jumpTime = animatorController.animationClips.ToList().Find(a => a.name.Equals(AnimLocalize.jump)).length;
        float elbowTime = animatorController.animationClips.ToList().Find(a => a.name.Equals(AnimLocalize.elbow)).length;
        float dotTime = jumpTime + elbowTime;

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.transform.position;
        Vector3 midPos = startPos + ((targetPos - startPos) / 2f) + Vector3.up * 3f;
        Vector3[] jumpPath = { startPos, midPos, targetPos };
        body.DOPath(jumpPath, dotTime, PathType.CatmullRom, PathMode.Full3D).OnComplete(
            () =>
            {
                navMeshAgent.enabled = true;
                characterController.enabled = true;
                //Å¸°Ù ³Ë¹é
                isAttacking = false;
            });
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

        navMeshAgent.SetDestination(target.transform.position);

        if (Vector3.Distance(transform.position, target.transform.position) <= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
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
