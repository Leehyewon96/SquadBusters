using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackCircle : MonoBehaviour
{
    public enum circleType
    { 
        None = 0,

        Player,
        NPC,

        End,
    }
    
    private List<GameObject> owners = new List<GameObject>();

    public bool isUsed { get; private set; } = false;
    public circleType type = circleType.None;

    public delegate void DetectEnemy(GameObject target);
    public DetectEnemy onDetectEnemy;
    public delegate void UnDetectEnemy(GameObject target);
    public UnDetectEnemy onUnDetectEnemy;
    private SphereCollider sphereCollider = null;

    private string postFixLayer = "AttackCircle";

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        MoveAttackCircle(owners.FirstOrDefault().transform.position);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void UpdateIsUsed(bool used)
    {
        isUsed = used;
    }

    public void UpdateOwners(GameObject newOwner)
    {
        if(!owners.Contains(newOwner))
        {
            owners.Add(newOwner);
        }
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
        gameObject.layer = LayerMask.NameToLayer(layerName + postFixLayer);
    }

    public void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.layer.Equals(gameObject.layer) && !other.gameObject.layer.Equals(LayerMask.NameToLayer(LayerLocalize.item)))
        {
            if (other.gameObject.TryGetComponent<AttackCircle>(out AttackCircle circle))
            {
                foreach(var owner in circle.owners)
                {
                    if (onDetectEnemy != null)
                    {
                        onDetectEnemy.Invoke(owner);
                    }
                }
                
                /*if (onDetectEnemy != null)
                {
                    onDetectEnemy.Invoke(circle.owner);
                }*/
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.layer.Equals(gameObject.layer) && !other.gameObject.layer.Equals(LayerMask.NameToLayer(LayerLocalize.item)))
        {
            if (other.gameObject.TryGetComponent<AttackCircle>(out AttackCircle circle))
            {
                foreach (var owner in circle.owners)
                {
                    if (onUnDetectEnemy != null)
                    {
                        onUnDetectEnemy.Invoke(owner);
                    }
                }
                /*if (onUnDetectEnemy != null)
                {
                    onUnDetectEnemy.Invoke(circle.owner);
                }*/
            }

        }
    }
}
