using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject target = null; // 로컬 플레이어 찾아서 넣는 코드로 변경 필요
    [SerializeField] private float delayTime = 30f;

    [SerializeField] private float m_roughness = 50f; //거칠기 정도
    [SerializeField] private float m_magnitude = 2f; //움직임 범위
    private bool cameraShake = false;

    public delegate void CameraShake(float duration);
    public CameraShake onCameraShake = null;

    private Camera cam = null;

    private Vector3 fixedPos = Vector3.zero;
    private Vector3 offsetVec = Vector3.zero;
   

    private void Start()
    {
        cam = Camera.main;
        onCameraShake += ShakeCamera;
    }

    private void Update()
    {
        if(target == null)
        {
            return;
        }

        if (cameraShake)
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

    private void ShakeCamera(float duration)
    {
        if(cameraShake)
        {
            return;
        }
        Debug.Log("cameraShake");
        cameraShake = true;
        StartCoroutine(Shake(duration, transform.position));
    }


    private IEnumerator Shake(float duration, Vector3 originPos)
    {
        float halfDuration = duration / 2;
        float elapsed = 0f;
        float tick = Random.Range(-10f, 10f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime / halfDuration;

            tick += Time.deltaTime * m_roughness;
            transform.position = originPos +  new Vector3(
                Mathf.PerlinNoise(tick, 0) - .5f,
                Mathf.PerlinNoise(0, tick) - .5f,
                Mathf.PerlinNoise(0, tick) - .5f) * m_magnitude * Mathf.PingPong(elapsed, halfDuration);

            yield return null;
        }

        cameraShake = false;
    }
}
