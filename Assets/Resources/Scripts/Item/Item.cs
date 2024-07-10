using DG.Tweening;
using Photon.Pun;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected PhotonView photonView = null;
    [SerializeField] private ItemType type = ItemType.Coin;

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

    protected void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.TryGetComponent<IAttackCircleItemInterface>(out IAttackCircleItemInterface attackCircleItemInterface))
        {
            attackCircleItemInterface.TakeItem(type);

            if(type == ItemType.TreasureBox)
            {
                SetActive(false);
                return;
            }

            transform.DOMove(other.gameObject.transform.position + Vector3.up * 1.2f, 0.5f).OnComplete(() =>
            {
                SetActive(false);
            });
        }
    }
}
