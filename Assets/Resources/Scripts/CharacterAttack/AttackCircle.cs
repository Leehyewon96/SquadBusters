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
    private AttackCircleStat attackCircleStat = null;

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
        attackCircleStat = GetComponent<AttackCircleStat>();
    }


    private void Update()
    {
        MoveAttackCircle();

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

    public void RemoveOwner(GameObject inOwner)
    {
        if (owners.Contains(inOwner))
        {
            owners.Remove(inOwner);
        }
    }

    public void UpdateRadius(float newRadius)
    {
        transform.localScale = Vector3.one * newRadius * 2; // 콜라이더의 반지름이 아닌 전체 구 오브젝트의 지름이라서 *2
    }

    public void MoveAttackCircle()
    {
        if(owners.Count == 0)
        {
            return;
        }

        transform.position = owners.FirstOrDefault().transform.position;
    }

    public void UpdateLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
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

    public void GainCoin()
    {
        attackCircleStat.coin += 1;
    }

    public void GainGem()
    {
        attackCircleStat.gem += 1;
    }

    public void GainTreasureBox()
    {
        Vector3 pos = Vector3.zero;
        float x = Random.Range(-attackCircleStat.attackRadius, attackCircleStat.attackRadius);
        float z = Random.Range(0, Mathf.Pow(attackCircleStat.attackRadius, 2) - Mathf.Pow(x, 2));
        pos.x = x + transform.position.x;//Random.Range(transform.position.x - 0.1f, transform.position.x + 0.1f);
        pos.z = Random.Range(-Mathf.Sqrt(z), Mathf.Sqrt(z)) + transform.position.z;//Random.Range(transform.position.z - 0.1f, transform.position.z + 0.1f);
        GameObject player = GameManager.Instance.SpawnPlayer(pos);
    }
}
