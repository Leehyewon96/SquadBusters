using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject target = null; // 로컬 플레이어 찾아서 넣는 코드로 변경 필요
    [SerializeField] private float delayTime = 30f;

    private Camera cam = null;

    private Vector3 fixedPos = Vector3.zero;
    private Vector3 offsetVec = Vector3.zero;
   

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if(target == null)
        {
            return;
        }

        FollowTarget();
    }

    private void FollowTarget()
    {
        fixedPos = offsetVec + target.transform.position;
        cam.transform.position = Vector3.Lerp(cam.transform.position, fixedPos, Time.deltaTime * delayTime);
    }

    public void SetTarget(GameObject inTarget)
    {
        target = inTarget;
        offsetVec = cam.transform.position - target.transform.position;
    }

}
