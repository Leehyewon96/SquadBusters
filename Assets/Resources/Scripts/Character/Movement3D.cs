using UnityEngine;

public class Movement3D : MonoBehaviour
{
    [HideInInspector] public CharacterController characterController = null;

    protected float moveSpeed = 7.5f;
    protected Vector3 moveDirection = Vector3.zero;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void Move(float x, float z)
    {
        if(x == 0 && z == 0)
        {
            return;
        }

        moveDirection = new Vector3(x, 0, z);

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), 0.1f);
    }

    public void UpdateMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}
