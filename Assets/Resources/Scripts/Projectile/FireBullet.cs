using DG.Tweening;
using Photon.Pun;
using UnityEngine;

public class FireBullet : Projectile
{
    private float shotTime = 5f;
    private float stunTime = 3f;

    protected override void Awake()
    {
        base.Awake();
        projectileType = ProjectileType.FireBullet;
    }

    public void SetStunTime(float newTime)
    {
        stunTime = newTime;
    }

    public void Shot(Vector3 destination)
    {
        transform.DOMove(destination, shotTime).OnComplete(() =>
        {
            SetActive(false);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<ICharacterProjectileInterface>(out ICharacterProjectileInterface player))
        {
            player.Stun(stunTime, AnimLocalize.knockBack);
        }
    }
}
