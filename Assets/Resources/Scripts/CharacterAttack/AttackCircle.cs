using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public class AttackCircle : MonoBehaviour
{
    public enum circleType
    { 
        None = 0,

        Player,
        NPC,

        End,
    }


    protected List<CharacterBase> owners = new List<CharacterBase>();
    protected AttackCircleStat attackCircleStat = null;

    protected List<GameObject> DetectedEnemies = new List<GameObject>();

    public bool isUsed { get; private set; } = false;
    [HideInInspector] public circleType type = circleType.None;

    public delegate void DetectEnemy(CharacterBase target);
    public DetectEnemy onDetectEnemy;
    public delegate void UnDetectEnemy(CharacterBase target);
    public UnDetectEnemy onUnDetectEnemy;

    private SphereCollider sphereCollider = null;
    protected PhotonView photonView = null;

    private string postFixLayer = "AttackCircle";

    protected virtual void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        attackCircleStat = GetComponent<AttackCircleStat>();
        photonView = GetComponent<PhotonView>();
    }

    protected virtual void Start()
    {
        if (owners.Count == 0)
        {
            return;
        }

        transform.position = owners.FirstOrDefault().transform.position;
    }

    protected virtual void Update()
    {
        if(DetectedEnemies.Count == 0)
        {
            //owners.ForEach(o => o.SetDestination());
            return;
        }

        owners.ForEach(o => o.SetDestination(DetectedEnemies.FirstOrDefault().transform.position));
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void UpdateIsUsed(bool used)
    {
        isUsed = used;
    }

    public virtual void UpdateOwners(CharacterBase newOwner)
    {
        if(!owners.Contains(newOwner))
        {
            owners.Add(newOwner);
            onDetectEnemy -= newOwner.UpdateEnemyList;
            onDetectEnemy += newOwner.UpdateEnemyList;
            onUnDetectEnemy -= newOwner.OnUnDetectEnemy;
            onUnDetectEnemy += newOwner.OnUnDetectEnemy;
            newOwner.deadAction -= RemoveOwner;
            newOwner.deadAction += RemoveOwner;
        }
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

    public CharacterBase SpawnPlayer(Vector3 pos, CharacterType charType)
    {
        string path = $"Prefabs/Character/{charType.ToString()}";
        GameObject character = PhotonNetwork.Instantiate(path, pos, Quaternion.identity);
        CharacterBase characterBase = character.GetComponent<CharacterBase>();
        characterBase.gameObject.name += "mine";
        UpdateOwners(characterBase);
        return characterBase;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterBase>(out CharacterBase character))
        {
            if (owners.Contains(character))
            {
                return;
            }

            if(onDetectEnemy == null)
            {
                return;
            }
            onDetectEnemy.Invoke(character);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterBase>(out CharacterBase character))
        {
            if (onUnDetectEnemy == null)
            {
                return;
            }
            onUnDetectEnemy.Invoke(character);
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
}
