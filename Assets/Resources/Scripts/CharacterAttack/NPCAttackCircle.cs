using System.Linq;

public class NPCAttackCircle : AttackCircle
{
    protected override void Awake()
    {
        base.Awake();
        type = circleType.NPC;
        if (photonView.IsMine)
        {
            CharacterBase character = SpawnPlayer(transform.position, CharacterType.Eggy);
            UpdateOwners(character);
        }
    }

    protected override void Update()
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
}
