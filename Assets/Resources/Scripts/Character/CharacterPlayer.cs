using UnityEngine;

public class CharacterPlayer : CharacterBase
{
    protected Movement3D movement3D = null;

    protected override void Awake()
    {
        base.Awake();

        movement3D = GetComponent<Movement3D>();
    }

    protected void Update()
    {
        Move();
    }

    protected void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        movement3D.Move(x, z);
    }
}
