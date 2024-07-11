using System.Linq;
using UnityEngine;

public class NPCAttackCircle : AttackCircle
{
    protected override void Awake()
    {
        base.Awake();
        type = circleType.NPC;
    }

    protected virtual void Update()
    {
        Move();
    }

    public override void UpdateOwners(CharacterBase newOwner)
    {
        base.UpdateOwners(newOwner);
        if (!owners.Contains(newOwner))
        {
            attackCircleStat.SetCoin(newOwner.GetCoin());
            attackCircleStat.SetGem(newOwner.GetGem());
        }
    }

    protected virtual void Move()
    {
        if(owners.Count == 0)
        {
            return;
        }

        transform.position = owners.FirstOrDefault().transform.position;
    }

    public virtual void SpawnNPC(CharacterType characterType)
    {
        CharacterBase character = SpawnPlayer(transform.position, characterType);
    }
}
