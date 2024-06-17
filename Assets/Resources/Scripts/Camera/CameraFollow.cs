using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject target = null;
    [SerializeField] private float delayTime = 30f;

    private Camera mainCamera = null;

    private Vector3 fixedPos = Vector3.zero;
    private Vector3 offsetVec = Vector3.zero;
   

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        offsetVec = mainCamera.transform.position - target.transform.position;
    }

    private void Update()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        fixedPos = offsetVec + target.transform.position;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, fixedPos, Time.deltaTime * delayTime);
    }
}
