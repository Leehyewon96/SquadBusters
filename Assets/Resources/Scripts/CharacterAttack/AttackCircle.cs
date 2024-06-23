using UnityEngine;

public class AttackCircle : MonoBehaviour
{
    [SerializeField] public GameObject owner = null;
    public bool isUsed { get; private set; } = false;

    public delegate void DetectEnemy(GameObject target);
    public DetectEnemy onDetectEnemy;
    public delegate void UnDetectEnemy(GameObject target);
    public UnDetectEnemy onUnDetectEnemy;
    private SphereCollider sphereCollider = null;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void UpdateIsUsed(bool used)
    {
        isUsed = used;
    }

    public void UpdateOwner(GameObject newOwner)
    {
        owner = newOwner;
    }

    public void UpdateRadius(float newRadius)
    {
        transform.localScale = Vector3.one * newRadius * 2; // 콜라이더의 반지름이 아닌 전체 구 오브젝트의 지름이라서 *2
    }

    public void MoveAttackCircle(Vector3 newPos)
    {
        transform.position = newPos;
    }

    public void UpdateLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
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
        if (!other.gameObject.layer.Equals(gameObject.layer))
        {
            if (other.gameObject.TryGetComponent<AttackCircle>(out AttackCircle circle))
            {
                if (onUnDetectEnemy != null)
                {
                    onUnDetectEnemy.Invoke(circle.owner);
                }
            }

        }
    }
}
