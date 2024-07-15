using DG.Tweening;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    private float shotTime = 5f;
    private float stunTime = 3f;

    public void SetStunTime(float newTime)
    {
        stunTime = newTime;
    }

    public void Shot(Vector3 destination)
    {
        transform.DOMove(destination, shotTime).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<ICharacterPlayerProjectileInterface>(out ICharacterPlayerProjectileInterface player))
        {
            player.Stun(stunTime);
            //gameObject.SetActive(false);
        }
    }
}
