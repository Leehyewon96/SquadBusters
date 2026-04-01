using System;
using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject target = null;
    [SerializeField] private float delayTime = 30f;

    [SerializeField] private float m_roughness = 50f;
    [SerializeField] private float m_magnitude = 2f;
    private bool cameraShake = false;

    public Action<float> onCameraShake = null;

    private Camera cam = null;

    private Vector3 fixedPos = Vector3.zero;
    private Vector3 offsetVec = Vector3.zero;

    [Header("카메라 속성")]
    [Tooltip("타겟과의 시작 거리")]
    public float distance = 10.0f;
    [Tooltip("카메라 회전 속도")]
    public float rotationSpeed = 3.0f;
    [Tooltip("카메라 줌 속도")]
    public float zoomSpeed = 5.0f;

    [Header("거리 제한")]
    [Tooltip("가장 가까워질 수 있는 거리")]
    public float minDistance = 2.0f;
    [Tooltip("가장 멀어질 수 있는 거리")]
    public float maxDistance = 20.0f;

    [Header("각도 제한")]
    [Tooltip("카메라의 최소 수직 각도")]
    public float minYAngle = -20.0f;
    [Tooltip("카메라의 최대 수직 각도")]
    public float maxYAngle = 80.0f;

    private float xAngle = 0.0f;
    private float yAngle = 0.0f;

    private void Start()
    {
        cam = Camera.main;
        onCameraShake += ShakeCamera;

        Vector3 angles = transform.eulerAngles;
        xAngle = angles.y;
        yAngle = angles.x;
    }

    private void LateUpdate()
    {
        if (target == null) return;
        if (cameraShake) return;

        if (Input.GetMouseButton(1))
        {
            xAngle += Input.GetAxis("Mouse X") * rotationSpeed;
            yAngle -= Input.GetAxis("Mouse Y") * rotationSpeed;
            yAngle = ClampAngle(yAngle, minYAngle, maxYAngle);
        }

        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Quaternion rotation = Quaternion.Euler(yAngle, xAngle, 0);
        Vector3 position = rotation * new Vector3(0, 0, -distance) + target.transform.position;

        cam.transform.rotation = rotation;
        cam.transform.position = position;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    private void FollowTarget()
    {
        fixedPos = offsetVec + target.transform.position;
        cam.transform.position = Vector3.Lerp(cam.transform.position, fixedPos, Time.deltaTime * delayTime);
    }

    public void SetTarget(GameObject inTarget)
    {
        target = inTarget;
        offsetVec = new Vector3(0, 9.3f, -7.43f);
    }

    private void ShakeCamera(float duration)
    {
        if (cameraShake) return;
        cameraShake = true;
        StartCoroutine(Shake(duration, transform.position));
    }

    private IEnumerator Shake(float duration, Vector3 originPos)
    {
        float halfDuration = duration / 2;
        float elapsed = 0f;
        float tick = UnityEngine.Random.Range(-10f, 10f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime / halfDuration;
            tick += Time.deltaTime * m_roughness;

            transform.position = originPos + new Vector3(
                Mathf.PerlinNoise(tick, 0) - .5f,
                Mathf.PerlinNoise(0, tick) - .5f,
                Mathf.PerlinNoise(0, tick) - .5f) * m_magnitude * Mathf.PingPong(elapsed, halfDuration);

            yield return null;
        }

        cameraShake = false;
    }
}
