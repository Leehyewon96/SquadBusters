using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Userper : CharacterNonPlayer
{
    protected int attackCount = 0;
    protected WaitForSecondsRealtime attackReadyTime = new WaitForSecondsRealtime(5f);
    protected float attackDistance = 5f;
    protected float attackIntervalAngle = 60f;
    protected float stunTime = 5f;

    protected override void MoveToEnemy()
    {
        GameObject target = GetTarget();
        Vector3 dirVec = Vector3.zero;
        if (target == gameObject)
        {
            //���������� 30�� ȸ��
            dirVec = Vector3.up / Mathf.Asin(attackIntervalAngle);
        }
        else
        {
            //Ÿ�������� ȸ��
            dirVec = target.transform.position - transform.position;
        }

        transform.DORotate(dirVec, 0.5f).OnComplete(() =>
        {
            //���̾ ����
            if(attackCount < 3)
            {
                attackCount++;
                ShotFireBall(transform.forward);
            }
            else
            {
                characterState = CharacterState.InVincible;
                attackCount = 0;
                StartCoroutine(CoShotFireBall(transform.forward));
            }
        });
    }

    protected virtual IEnumerator CoShotFireBall(Vector3 dirVec)
    {
        //���̾ 3���� �� �̸����� ȿ�� ���
        dirVec.Normalize();
        Vector3[] dirVecs = new Vector3[3];
        dirVecs[0] = transform.forward;
        dirVecs[1] = Vector3.up / Mathf.Asin(attackIntervalAngle);
        dirVecs[1] = Vector3.up / Mathf.Asin(-attackIntervalAngle);

        yield return attackReadyTime;

        //���̾ �߻�
        foreach(Vector3 dir in dirVecs)
        {
            ShotFireBall(dir);
            characterState = CharacterState.Idle;
        }
    }

    protected virtual void ShotFireBall(Vector3 dirVec)
    {
        FireBullet bullet = GameManager.Instance.projectileManager.GetBullet(transform.position + transform.forward.normalized * 0.5f);
        bullet.transform.rotation = Quaternion.LookRotation(dirVec);
        bullet.SetStunTime(stunTime);
        bullet.Shot(transform.position + dirVec.normalized * attackDistance);
    }
}
