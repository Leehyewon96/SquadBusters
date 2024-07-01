using System.Collections;
using System.Linq;
using UnityEngine;

public class ElPrimo : CharacterPlayer
{
    private int killCount = 0;
    [SerializeField] private int FlyingElbowAttackValue = 2;

    private bool isFlyingElbowAttackMode = false;
    public Rigidbody body;
    float jumpForce = 100f;
    protected override void Update()
    {
        //Jump();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FlyingElbowAttack(transform.position + transform.forward.normalized * 4f);
        }

    }

    private void Jump()
    {
        // �����̽��ٸ� ������(�Ǵ� ������ ������)
        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetTrigger(AnimLocalize.Jump);
            // body�� ���� ���Ѵ�(AddForce)
            // AddForce(����, ���� ��� ���� ���ΰ�)
            navMeshAgent.enabled = false;
            body.AddForce(Vector3.up * jumpForce);
            StartCoroutine(SetEnableNavMeshAgent(false));
            //navMeshAgent.enabled = true;
        }
    }

    private IEnumerator SetEnableNavMeshAgent(bool isEnabled)
    {
        yield return new WaitForSeconds(0.5f);
        navMeshAgent.enabled = isEnabled;
    }

    private void FlyingElbowAttack(Vector3 target)
    {
        animator.SetBool(AnimLocalize.Jump, true);
        AnimationClip clip = animatorController.animationClips.ToList().Find(c => c.name.Equals(AnimLocalize.Jump));
        StartCoroutine(CoJump(clip.length, transform.position, target));
    }

    IEnumerator CoJump(float duration, Vector3 startPos, Vector3 targetPos)
    {
        var runTime = 0.0f;

        while (runTime < duration)
        {
            runTime += Time.deltaTime;

            characterController.Move((targetPos - startPos).normalized * Time.deltaTime);
            //transform.position = Vector3.Lerp(startPos, targetPos, runTime / duration);

            yield return null;
        }
        animator.SetBool(AnimLocalize.Jump, false);
    }
}
