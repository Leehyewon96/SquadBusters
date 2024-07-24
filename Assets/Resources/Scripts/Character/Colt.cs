using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Colt : CharacterPlayer
{
    [SerializeField] private GameObject gunPoint = null;
    private float shotDistance = 10f;

    protected override void MoveToEnemy()
    {
        GameObject target = GetTarget();
        if(target == gameObject)
        {
            animator.SetBool(AnimLocalize.contactEnemy, false);
            isAttacking = false;
            return;
        }

        if(isAttacking)
        {
            return;
        }

        Attack(target);
    }

    protected override void Attack(GameObject target)
    {
        isAttacking = true;
        TweenCallback callback = () =>
        {
            StartCoroutine(CoAttack(target));
        };
        ForwardToEnemy(target, callback);
    }

    protected override IEnumerator CoAttack(GameObject target)
    {
        int cnt = 4;
        animator.SetBool(AnimLocalize.contactEnemy, true);
        while (cnt > 0)
        {
            ForwardToEnemy(target);
            Projectile projectile = GameManager.Instance.projectileManager.GetProjectile(gunPoint.transform.position, ProjectileType.Bullet);
            projectile.SetDirection(transform.forward.normalized);
            projectile.SetDamage(characterStat.GetAttackDamage());
            projectile.Shot(transform.position + transform.forward.normalized * shotDistance);
            
            cnt--;
            yield return new WaitForSeconds(0.6f);
        }

        animator.SetBool(AnimLocalize.contactEnemy, false);
        isAttacking = false;
    }
}
