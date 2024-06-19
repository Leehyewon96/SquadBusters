using UnityEngine;
using static AttackCircle;

public class CharacterPlayer : CharacterBase
{
    [SerializeField] protected AttackCircle attackCircle = null; //attackCircle �Ŵ��� ���� �� �Ҵ� �ʿ�

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
        if (CheckInput()) //�÷��̾� �����Ҷ�
        {
            isAttacking = false;
            DetectedEnemies.Clear();
            navMeshAgent.SetDestination(transform.position);
            MoveAttackCircle();
            attackCircle.SetActive(false); //Ž����ɸ� ���°ɷ� ����
            Move();
        }
        else
        {
            attackCircle.SetActive(true); // ���̴°͸� ��Ƽ��Ǵ°�.
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
        //����Ͽ��� ��ġ�� ����
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
