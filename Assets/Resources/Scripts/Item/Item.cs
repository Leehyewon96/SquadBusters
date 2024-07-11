using DG.Tweening;
using Photon.Pun;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected PhotonView photonView = null;
    [SerializeField] private ItemType type = ItemType.Coin;

    private bool isPicked = false;

    protected virtual void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public virtual void SetActive(bool isActive)
    {
        photonView.RPC("RPCSetActive", RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    public virtual void RPCSetActive(bool isActive)
    {
        isPicked = !isActive;
        gameObject.SetActive(isActive);
    }

    public virtual void SetPosition(Vector3 newPos)
    {
        photonView.RPC("RPCSetPosition", RpcTarget.AllBuffered, newPos);
    }

    [PunRPC]
    public virtual void RPCSetPosition(Vector3 newPos)
    {
        gameObject.transform.position = newPos;
    }

    public ItemType GetItemType()
    {
        return type;
    }

    public void UpdateItemType(ItemType newType)
    {
        type = newType;
    }

    [PunRPC]
    public void SetIsPicked(bool picked)
    {
        if(photonView.IsMine)
        {
            isPicked = picked;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (isPicked)
        {
            return;
        }

        if (other.gameObject.TryGetComponent<IAttackCircleItemInterface>(out IAttackCircleItemInterface attackCircleItemInterface))
        {
            attackCircleItemInterface.TakeItem(type);

            if(type == ItemType.TreasureBox)
            {
                SetActive(false);
                return;
            }
            photonView.RPC("SetIsPicked", RpcTarget.AllBuffered, true);
            transform.DOMove(other.gameObject.transform.position + Vector3.up * 1.2f, 0.5f).OnComplete(() =>
            {
                SetActive(false);
            });
        }
    }
}
