using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharacterNonPlayer : CharacterBase
{
    protected override void Update()
    {
        base.Update();
        MoveToEnemy();
        MoveAttackCircle();

    }
}
