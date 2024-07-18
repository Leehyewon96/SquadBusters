using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private PhotonView photonView = null;
    protected ProjectileType projectileType;

    protected virtual void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public ProjectileType GetProjectileType()
    {
        return projectileType; 
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
