using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackCircle : MonoBehaviour
{
    public GameObject owner = null;

    public delegate void DetectEnemy(GameObject target);
    public DetectEnemy onDetectEnemy;
    public delegate void UnDetectEnemy(GameObject target);
    public UnDetectEnemy onUnDetectEnemy;
    private SphereCollider sphereCollider = null;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    //public void SetActiveDetectEnemy(bool isActive)
    //{
    //    sphereCollider.enabled = isActive;
    //}

    public void UpdateRadius(float newRadius)
    {
        transform.localScale = Vector3.one * newRadius * 2; // 콜라이더의 반지름이 아닌 전체 구 오브젝트의 지름이라서 *2
    }

    public void MoveAttackCircle(Vector3 newPos)
    {
        transform.position = newPos;
    }

    public void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.layer.Equals(gameObject.layer))
        {
            if (other.gameObject.TryGetComponent<AttackCircle>(out AttackCircle circle))
            {
                
                if (onDetectEnemy != null)
                {
                    onDetectEnemy.Invoke(circle.owner);
                }
            }
            
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log($"[exit] other.name {other.gameObject.name}");
        if (!other.gameObject.layer.Equals(gameObject.layer))
        {
            Debug.Log($"[exit] {gameObject.name} : {other.gameObject.name}");
            if (other.gameObject.TryGetComponent<AttackCircle>(out AttackCircle circle))
            {
                Debug.Log($"2");
                if (onUnDetectEnemy != null)
                {
                    Debug.Log($"3");
                    onUnDetectEnemy.Invoke(circle.owner);
                }
            }

        }
    }
}
