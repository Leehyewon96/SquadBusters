using Photon.Pun;
using UnityEngine;

public class TreasureBoxItem : Item
{
    protected override void Awake()
    {
        base.Awake();
        type = ItemType.TreasureBox;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (isPicked)
        {
            return;
        }

        if (other.gameObject.TryGetComponent<IAttackCircleItemInterface>(out IAttackCircleItemInterface attackCircleItemInterface))
        {
            //�׽�Ʈ�� ���Ͽ� ���� ���� ����
            if (attackCircleItemInterface.GetCoin() < GameManager.Instance.treasureBoxCost)
            {
                return;
            }
            photonView.RPC("SetIsPicked", RpcTarget.AllBuffered, true);
            attackCircleItemInterface.GainTreasureBox();
            attackCircleItemInterface.SetCoin(attackCircleItemInterface.GetCoin() - GameManager.Instance.treasureBoxCost);
            GameManager.Instance.SetTreasureBoxCost(GameManager.Instance.treasureBoxCost + 2);
            SetActive(false);
        }
    }
}
