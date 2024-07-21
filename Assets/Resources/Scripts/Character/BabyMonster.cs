using DG.Tweening;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BabyMonster : CharacterNonPlayer
{
    protected int attackCount = 0;
    protected WaitForSecondsRealtime attackReadyTime = new WaitForSecondsRealtime(5f);
    protected float attackDistance = 10f;
    protected float attackIntervalAngle = 45f;
    protected float stunTime = 3.233f;

    protected override void MoveToEnemy()
    {
        if (characterState == CharacterState.Attack)
        {
            return;
        }

        characterState = CharacterState.Attack;

        GameObject target = GetTarget();
        Vector3 dirVec = Vector3.zero;
        if (target == gameObject)
        {
            float angle = Quaternion.FromToRotation(Vector3.forward, transform.forward).eulerAngles.y;
            angle += attackIntervalAngle;
            dirVec = Vector3.up * angle;
        }
        else
        {
            //Ÿ�������� ȸ��
            Vector3 dir = target.transform.position - transform.position;
            float angle = Quaternion.FromToRotation(transform.forward, dir).eulerAngles.y;
            angle += Quaternion.FromToRotation(Vector3.forward, transform.forward).eulerAngles.y;
            dirVec = Vector3.up * angle;
        }

        transform.DORotate(dirVec, 1f).OnComplete(() =>
        {
            //���̾ ����
            if (attackCount < 3)
            {
                attackCount++;
                characterState = CharacterState.Idle;
                ShotFireBall(transform.forward);
            }
            else
            {
                attackCount = 0;
                StartCoroutine(CoShotFireBall());
            }
        });
    }

    protected virtual IEnumerator CoShotFireBall()
    {
        //���̾ 3���� �� �̸����� ȿ�� ���
        Vector3[] dirVecs = new Vector3[3];
        dirVecs[0] = transform.forward;
        dirVecs[1] = Quaternion.AngleAxis(-attackIntervalAngle, Vector3.up) * dirVecs[0];
        dirVecs[2] = Quaternion.AngleAxis(attackIntervalAngle, Vector3.up) * dirVecs[0];

        foreach(Vector3 dir in dirVecs)
        {
            photonView.RPC("RPCEffect", RpcTarget.AllBuffered, dir);
        }

        yield return attackReadyTime;

        
        //���̾ �߻�
        foreach (Vector3 dir in dirVecs)
        {
            ShotFireBall(dir);
        }

        yield return new WaitForSeconds(2f);
        characterState = CharacterState.Idle;
    }

    protected virtual void ShotFireBall(Vector3 dirVec)
    {
        animator.SetTrigger(AnimLocalize.attack);
        Projectile projectile = GameManager.Instance.projectileManager.GetProjectile(transform.position + transform.forward.normalized * 0.5f + Vector3.up * 2.5f, ProjectileType.FireBullet);
        FireBullet bullet = projectile.gameObject.GetComponent<FireBullet>();
        bullet.transform.rotation = Quaternion.LookRotation(dirVec);
        bullet.SetStunTime(stunTime);
        bullet.Shot(transform.position + dirVec.normalized * attackDistance);
    }

    [PunRPC]
    public void RPCEffect(Vector3 dirVec)
    {
        GameManager.Instance.effectManager.Play(EffectType.LaserAOE, transform.position + dirVec.normalized * 5f + Vector3.up * 0.4f, dirVec);
    }
}
