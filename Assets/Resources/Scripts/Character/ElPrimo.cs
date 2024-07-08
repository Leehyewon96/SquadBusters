using DG.Tweening;
using UnityEngine;

public class ElPrimo : CharacterPlayer
{
    private int killCount = 0;
    [SerializeField] private int FlyingElbowAttackValue = 2;

    private bool isFlyingElbowAttackMode = false;
    public Rigidbody body;
    float jumpForce = 100f;

    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<Rigidbody>();
    }

    protected override void Update()
    {
        //navMeshAgent.ResetPath();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 endPos = transform.position + transform.forward.normalized * 5f;
            FlyingElbowAttack(transform.position, endPos);
        }

    }

    private void FlyingElbowAttack(Vector3 startPos, Vector3 targetPos)
    {
        navMeshAgent.enabled = false;
        characterController.enabled = false;
        Vector3 midPos = startPos + ((targetPos - startPos) / 2f) + Vector3.up * 3f;
        Vector3[] jumpPath = { startPos, midPos, targetPos };
        body.DOPath(jumpPath, 0.5f, PathType.CatmullRom, PathMode.Full3D).OnComplete(
            () =>
            {
                navMeshAgent.enabled = true;
                characterController.enabled = true;
            });
    }
}
