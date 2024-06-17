using UnityEngine;

public class Movement3D : MonoBehaviour
{
    [HideInInspector] public CharacterController characterController = null;
    [HideInInspector] public Animator animator = null;

    [SerializeField] protected float moveSpeed = 50.0f;
    protected Vector3 moveDirection = Vector3.zero;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void Move(float x, float z)
    {
        if (x == 0 && z == 0)
        {
            return;
        }

        moveDirection = new Vector3(x, 0, z);

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), 0.05f);
        PlayAnim();
    }

    public void PlayAnim()
    {
        animator.SetFloat(AnimLocalize.moveSpeed, characterController.velocity.magnitude);
    }
}
