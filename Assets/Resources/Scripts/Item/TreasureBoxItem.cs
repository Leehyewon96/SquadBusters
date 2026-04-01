using Photon.Pun;
using UnityEngine;

public class TreasureBoxItem : Item
{
    protected override void Awake()
    {
        base.Awake();
        type = ItemType.TreasureBox;
    }

    protected virtual void OnDisable()
    {
        onUndetectedPlayerAttack?.Invoke();
        onUndetectedPlayerAttack = null;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (isPicked) return;

        if (other.gameObject.TryGetComponent<IAttackCircleItemInterface>(out IAttackCircleItemInterface attackCircleItemInterface))
        {
            if (attackCircleItemInterface.GetCoin() < GameManager.Instance.treasureBoxCost)
            {
                attackCircleItemInterface.ShowNotice(NoticeType.TreasureBox, this);
                return;
            }

            if (GameManager.IsTrainingMode)
                isPicked = true;
            else
                photonView.RPC("SetIsPicked", RpcTarget.AllBuffered, true);

            attackCircleItemInterface.GainTreasureBox();
            attackCircleItemInterface.SetCoin(attackCircleItemInterface.GetCoin() - GameManager.Instance.treasureBoxCost);
            GameManager.Instance.SetTreasureBoxCost(GameManager.Instance.treasureBoxCost + 2);
            SetActive(false);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<IAttackCircleItemInterface>(out IAttackCircleItemInterface circleItemInterface))
        {
            onUndetectedPlayerAttack?.Invoke();
            onUndetectedPlayerAttack = null;
            circleItemInterface.OnUnDetectedItem(this);
        }
    }
}
