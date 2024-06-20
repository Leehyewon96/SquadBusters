using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharacterPlayer : CharacterBase
{
    protected override void Awake()
    {
        base.Awake();
        
        //attackCircle = GetComponent<AttackCircle>();
    }

    protected override void Start()
    {
        base.Start();
        
    }

    protected override void Update()
    {
        base.Update();
        if (CheckInput()) //플레이어 조작할때
        {
            isAttacking = false;
            DetectedEnemies.Clear();
            animator.SetBool(AnimLocalize.contactEnemy, false);
            navMeshAgent.SetDestination(transform.position);
            MoveAttackCircle();
            //attackCircle.SetActiveDetectEnemy(false);
            Move();
        }
        else
        {
            if(!isAttacking)
            {
                //attackCircle.SetActiveDetectEnemy(true);
                MoveToEnemy();
            }
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

    protected override void UpdateEnemyList(GameObject target)
    {
        if (!DetectedEnemies.Contains(target)
            && Vector3.Distance(target.transform.position, transform.position) <= characterStat.GetAttackRadius())
        {
            DetectedEnemies.Add(target);
        }
    }
}
