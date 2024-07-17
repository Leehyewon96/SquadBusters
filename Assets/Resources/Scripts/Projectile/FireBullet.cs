using DG.Tweening;
using Photon.Pun;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    private float shotTime = 5f;
    private float stunTime = 3f;
    private PhotonView photonView = null;

    public void Awake()
    {
        photonView = GetComponent<PhotonView>();
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
        if(other.gameObject.TryGetComponent<ICharacterPlayerProjectileInterface>(out ICharacterPlayerProjectileInterface player))
        {
            player.Stun(stunTime, AnimLocalize.knockBack);
        }
    }

    public void SetActive(bool isActive)
    {
        photonView.RPC("RPCSetActive", RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    public void RPCSetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
