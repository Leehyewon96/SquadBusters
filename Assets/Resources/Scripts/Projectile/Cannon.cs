using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Projectile
{
    private float shotDistance = 8f;
    public float lifeTime { get; private set; } = 10.0f;
    private float hp = 3500f;
    private List<GameObject> hosts = new List<GameObject>();
    private bool isAttacking = false;
    [SerializeField] private GameObject shotPoint = null;
    [SerializeField] private ParticleSystem effect = null;

    protected override void Awake()
    {
        base.Awake();
        projectileType = ProjectileType.Cannon;
        SetDamage(145f);
    }

    public void SetHost(GameObject newHost)
    {
        if (!hosts.Contains(newHost))
            hosts.Add(newHost);
    }

    public void Attack(GameObject target)
    {
        if (hosts.Contains(target)) return;
        isAttacking = true;

        TweenCallback callback = () => StartCoroutine(CoAttack(target));
        ForwardToTarget(target, callback);
    }

    private void ForwardToTarget(GameObject target, TweenCallback action = null)
    {
        Vector3 dirVec = target.transform.position - transform.position;
        float angle = Quaternion.FromToRotation(transform.forward, dirVec).eulerAngles.y;
        angle += Quaternion.FromToRotation(Vector3.forward, transform.forward).eulerAngles.y;
        dirVec = Vector3.up * angle;
        transform.DORotate(dirVec, 0.5f).OnComplete(action);
    }

    private IEnumerator CoAttack(GameObject target)
    {
        while (target.activeSelf)
        {
            ForwardToTarget(target);

            Projectile projectile = GameManager.Instance.projectileManager.GetProjectile(shotPoint.transform.position, ProjectileType.Bullet);
            projectile.SetDamage(damage);
            projectile.SetDirection(transform.forward);
            projectile.Shot(transform.position + transform.forward.normalized * shotDistance, 1f);

            if (GameManager.IsTrainingMode)
                RPCShotEffect();
            else
                photonView.RPC("RPCShotEffect", RpcTarget.AllBuffered);

            yield return new WaitForSeconds(0.5f);
        }
        isAttacking = false;
    }

    [PunRPC]
    public void RPCShotEffect() => effect.Play();

    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine && !GameManager.IsTrainingMode) return;
        if (isAttacking) return;

        if (other.gameObject.TryGetComponent<ICharacterProjectileInterface>(out _))
            Attack(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine && !GameManager.IsTrainingMode) return;

        if (other.gameObject.TryGetComponent<ICharacterProjectileInterface>(out _))
        {
            StopAllCoroutines();
            isAttacking = false;
        }
    }
}
