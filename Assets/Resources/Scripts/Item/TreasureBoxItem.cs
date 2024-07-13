using Photon.Pun;
using UnityEngine;

public class TreasureBoxItem : Item
{
    

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (isPicked)
        {
            return;
        }

        if (other.gameObject.TryGetComponent<ICharacterPlayerItemInterface>(out ICharacterPlayerItemInterface attackCircleItemInterface))
        {
            //테스트를 위하여 코인 제한 없앰
            //if (attackCircleItemInterface.GetTotalCoin() < GameManager.Instance.treasureBoxCost)
            //{
            //    return;
            //}
            photonView.RPC("SetIsPicked", RpcTarget.AllBuffered, true);
            attackCircleItemInterface.TakeItem(type);
            attackCircleItemInterface.UpdateTotalCoin(attackCircleItemInterface.GetTotalCoin() - GameManager.Instance.treasureBoxCost);
            GameManager.Instance.SetTreasureBoxCost(GameManager.Instance.treasureBoxCost * 2);
            SetActive(false);
        }
    }
}
