using UnityEngine;

public class PlayerAttackCircle : AttackCircle
{
    protected GameObject moveObj = null;
    protected Movement3D movement3D = null;
    protected CharacterController characterController = null;

    protected override void Awake()
    {
        base.Awake();
        moveObj = new GameObject($"Move{gameObject.name}");
        movement3D = moveObj.AddComponent<Movement3D>();
        characterController = moveObj.AddComponent<CharacterController>();
        moveObj.transform.SetParent(transform.parent.transform);

        type = circleType.Player;
    }

    protected override void Start()
    {
        base.Start();
        moveObj.transform.position = transform.position;
    }

    protected virtual void Update()
    {
        if((CheckInput()))
        {
            Move();
            transform.position = moveObj.transform.position;
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
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            characterController.enabled = true;
            return true;
        }

        characterController.enabled = false;
        return false;
    }
}
