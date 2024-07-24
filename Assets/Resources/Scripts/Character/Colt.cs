using System.Collections;
using UnityEngine;

public class Colt : CharacterPlayer
{
    [SerializeField] private GameObject gunPoint = null;
    private float shotDistance = 5f;

    protected override void MoveToEnemy()
    {
        
    }

    protected override IEnumerator CoAttack(GameObject target)
    {
        ForwardToEnemy(target);
        Projectile projectile = GameManager.Instance.projectileManager.GetProjectile(gunPoint.transform.position, ProjectileType.Bullet);
        projectile.SetDamage(characterStat.GetAttackDamage());
        projectile.Shot(transform.position + transform.forward.normalized * shotDistance);

        yield return new WaitForSeconds(0.6f);
    }
}
