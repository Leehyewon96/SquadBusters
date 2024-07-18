using System.Collections;
using UnityEngine;

public class Bomb : Projectile
{
    protected override void Awake()
    {
        base.Awake();
        projectileType = ProjectileType.Bomb;
    }

    public void Explode(float waiting)
    {
        StartCoroutine(CoExplode(waiting));
    }

    private IEnumerator CoExplode(float waiting)
    {
        yield return new WaitForSeconds(waiting);
        SetActive(false);

        AOE aoe = GameManager.Instance.aoeManager.GetAOE(transform.position, AOEType.Red);
        GameManager.Instance.effectManager.Play(EffectType.Explosion, transform.position, transform.forward);
    }
}
