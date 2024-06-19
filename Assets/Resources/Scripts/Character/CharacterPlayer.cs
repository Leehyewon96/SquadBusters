using UnityEngine;
using static AttackCircle;

public class CharacterPlayer : CharacterBase
{
    [SerializeField] protected AttackCircle attackCircle = null; //attackCircle 매니저 구현 후 할당 필요

    protected override void Awake()
    {
        base.Awake();
        characterController = GetComponent<CharacterController>();
        //attackCircle = GetComponent<AttackCircle>();
    }

    protected override void Start()
    {
        base.Start();
        attackCircle.UpdateRadius(characterStat.GetAttackRadius());
        characterStat.onAttackRadiusChanged += attackCircle.UpdateRadius;
        attackCircle.onDetectEnemy += UpdateEnemyList;
        attackCircle.onUnDetectEnemy += OnUnDetectEnemy;
    }

    protected override void Update()
    {
        base.Update();
        if (CheckInput()) //플레이어 조작할때
        {
            isAttacking = false;
            DetectedEnemies.Clear();
            navMeshAgent.SetDestination(transform.position);
            MoveAttackCircle();
            attackCircle.SetActive(false); //탐지기능만 끄는걸로 변경
            Move();
        }
        else
        {
            attackCircle.SetActive(true); // 보이는것만 액티브되는것.
            MoveToEnemy();

        }
    }

    protected virtual void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        movement3D.Move(x, z);
    }

    protected virtual bool CheckInput()
    {
        //모바일에서 터치로 변경
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            characterController.enabled = true;
            return true;
        }

        characterController.enabled = false;
        return false;
    }

    protected virtual void MoveAttackCircle()
    {
        attackCircle.MoveAttackCircle(transform.position);
    }
}
