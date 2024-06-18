using UnityEngine;

public class CharacterPlayer : CharacterBase
{

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
        Move();
    }

    protected void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        movement3D.Move(x, z);
    }
}
