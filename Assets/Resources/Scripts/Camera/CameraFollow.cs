using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject target = null; // ���� �÷��̾� ã�Ƽ� �ִ� �ڵ�� ���� �ʿ�
    [SerializeField] private float delayTime = 30f;

    private Camera cam = null;

    private Vector3 fixedPos = Vector3.zero;
    private Vector3 offsetVec = Vector3.zero;
   

    private void Awake()
    {
        cam = Camera.main;
        offsetVec = cam.transform.position - target.transform.position;
    }

    private void Update()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        fixedPos = offsetVec + target.transform.position;
        cam.transform.position = Vector3.Lerp(cam.transform.position, fixedPos, Time.deltaTime * delayTime);
    }
}
