using Photon.Pun;
using UnityEngine;

public class Greg : CharacterPlayer
{
    public void OnDetectedMoneyTree(MoneyTree tree)
    {
        if(!DetectedEnemies.Contains(tree.gameObject))
        {
            DetectedEnemies.Add(tree.gameObject);
            foreach (var e in DetectedEnemies)
            {
                Debug.Log($"{e.gameObject.name}");
            }
        }
    }

    public void OnUnDetectedMoneyTree(MoneyTree tree)
    {
        if (DetectedEnemies.Contains(tree.gameObject))
        {
            DetectedEnemies.Remove(tree.gameObject);
        }
    }

    protected override GameObject GetTarget()
    {
        if (DetectedEnemies.Count > 0)
        {
            GameObject moneyTree = DetectedEnemies.Find(e => e != null && e.TryGetComponent<MoneyTree>(out _));
            if (moneyTree != null)
            {
                return moneyTree;
            }
        }

        return base.GetTarget();
    }

    protected override bool AttackTarget(GameObject target)
    {
        if (target.TryGetComponent<MoneyTree>(out MoneyTree tree))
        {
            photonView.RPC("RPCEffect", RpcTarget.AllBuffered, (int)EffectType.ChargeSlashPurple, transform.position + Vector3.up * 1.5f + transform.forward.normalized * 0.5f, transform.forward);
            tree.TakeDamage(characterStat.GetAttackDamage());

            if (tree.isDead)
            {
                OnTargetDead(target);
                return true;
            }
        }

        return base.AttackTarget(target);
    }
}
