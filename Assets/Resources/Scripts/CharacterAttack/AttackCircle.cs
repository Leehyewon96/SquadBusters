using System.Collections;
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
    
    private List<CharacterBase> owners = new List<CharacterBase>();
    private AttackCircleStat attackCircleStat = null;

    public bool isUsed { get; private set; } = false;
    public circleType type = circleType.None;

    public delegate void DetectEnemy(CharacterBase target);
    public DetectEnemy onDetectEnemy;
    public delegate void UnDetectEnemy(CharacterBase target);
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

    public void UpdateOwners(CharacterBase newOwner)
    {
        if(!owners.Contains(newOwner))
        {
            owners.Add(newOwner);
        }

        //머지할 수 있는지 검사
        List<CharacterBase> chars = owners.FindAll(o => o.GetCharacterType() == newOwner.GetCharacterType()).ToList();
        if(chars.Count < 3)
        {
            return;
        }

        
        StartCoroutine(CoMergeCharacter(chars, newOwner.transform.position));
    }

    private IEnumerator CoMergeCharacter(List<CharacterBase> chars, Vector3 pos)
    {
        yield return new WaitForSeconds(0.7f);
        SpawnPlayer(CharacterType.ElPrimo2);
        foreach (var ch in chars)
        {
            ch.SetDead();
        }

        owners.LastOrDefault().transform.position = pos;
    }

    public void RemoveOwner(CharacterBase inOwner)
    {
        if (owners.Contains(inOwner))
        {
            owners.Remove(inOwner);
        }

        if(owners.Count == 0)
        {
            SetDead();
        }
    }

    private void SetDead()
    {
        //죽은 오브젝트 자리에 동전 생성
        GameManager.Instance.itemManager.ShowItem(attackCircleStat.coin, transform.position, ItemType.Coin);
        GameManager.Instance.itemManager.ShowItem(attackCircleStat.gem, transform.position, ItemType.Gem);
        GameManager.Instance.effectManager.Explosion(transform.position);

        UpdateIsUsed(false);
        SetActive(false);
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
            }

        }
    }

    public void SetCoin(int cnt)
    {
        attackCircleStat.coin = cnt;
    }

    public void SetGem(int cnt)
    {
        attackCircleStat.gem = cnt;
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
        SpawnPlayer(CharacterType.ElPrimo);
    }

    private void SpawnPlayer(CharacterType newType)
    {
        Vector3 pos = Vector3.zero;
        float x = Random.Range(-attackCircleStat.attackRadius, attackCircleStat.attackRadius);
        float z = Random.Range(0, Mathf.Pow(attackCircleStat.attackRadius, 2) - Mathf.Pow(x, 2));
        pos.x = x + transform.position.x;
        pos.z = Random.Range(-Mathf.Sqrt(z), Mathf.Sqrt(z)) + transform.position.z;
        CharacterBase player = GameManager.Instance.SpawnPlayer(pos, newType);
        UpdateOwners(player);
    }
}
