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
