using System.Collections.Generic;
using UnityEngine;

public class AttackCircle : MonoBehaviour
{
    public delegate void DetectEnemy(GameObject target);
    public DetectEnemy onDetectEnemy;
    public delegate void UnDetectEnemy(GameObject target);
    public UnDetectEnemy onUnDetectEnemy;
    private SphereCollider sphereCollider = null;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    public void SetActiveDetectEnemy(bool isActive)
    {
        sphereCollider.enabled = isActive;
    }

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
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer(LayerLocalize.enemy)))
        {
            if (onDetectEnemy != null)
            {
                onDetectEnemy.Invoke(other.gameObject);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer(LayerLocalize.enemy)))
        {
            if (onUnDetectEnemy != null)
            {
                onUnDetectEnemy.Invoke(other.gameObject);
            }
        }
    }
}
