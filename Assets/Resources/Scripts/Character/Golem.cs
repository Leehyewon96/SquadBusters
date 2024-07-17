using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Golem : CharacterNonPlayer
{
    protected override void MoveToEnemy()
    {
        GameObject target = GetTarget();
        if (target == gameObject)
        {
            return;
        }

        if(characterState == CharacterState.Attack)
        {
            return;
        }

        characterState = CharacterState.Attack;

        Attack(target);
    }

    protected override void Attack(GameObject target)
    {
        Vector3 dirVec = target.transform.position - transform.position;
        float angle = Quaternion.FromToRotation(transform.forward, dirVec).eulerAngles.y;
        dirVec = Vector3.up * angle;

        transform.DORotate(dirVec, 1f).OnComplete(() =>
        {
            StartCoroutine(CoAttack(target));
        });

    }

    protected override IEnumerator CoAttack(GameObject target)
    {
        Debug.Log("Enter");
        animator.SetTrigger(AnimLocalize.attack);
        yield return new WaitForSeconds(7.5f);
        Debug.Log("Out");
        GameManager.Instance.aoeManager.GetAOE(transform.position + transform.forward.normalized * 2f);
        characterState = CharacterState.Idle;
    }
}
