using UnityEngine;

public class CharacterPlayer : CharacterBase
{
    [SerializeField] protected AttackCircle attackCircle = null; //attackCircle 매니저 구현 후 할당 필요

    protected override void Awake()
    {
        base.Awake();
        //attackCircle = GetComponent<AttackCircle>();
    }

    protected override void Start()
    {
        base.Start();
        attackCircle.UpdateRadius(characterStat.GetAttackRadius());
        characterStat.onAttackRadiusChanged += attackCircle.UpdateRadius;
        attackCircle.onDetectEnemy += UpdateEnemyList;
    }

    protected override void Update()
    {
        base.Update();
        Move();
        MoveAttackCircle();
        //attackCircle.MoveAttackCircle(transform.position);
    }

    protected override void Move()
    {
        base.Move();

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if(x == 0 && z == 0)
        {
            return;
        }
        movement3D.Move(x, z);
    }

    protected virtual void MoveAttackCircle()
    {
        attackCircle.MoveAttackCircle(transform.position);
    }
}
