using Photon.Pun;
using System.Collections;
using UnityEngine;

public class AOE : MonoBehaviour
{
    private float lifeTime = 3f;
    private float damage = 10f;

    private PhotonView photonView = null;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        StartCoroutine(CoSetActive(false));

    }

    private IEnumerator CoSetActive(bool isActive)
    {
        yield return new WaitForSeconds(lifeTime);
        SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<ICharacterPlayerProjectileInterface>(out ICharacterPlayerProjectileInterface player))
        {
            player.TakeDamage(damage);
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
