using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackCircle : MonoBehaviour
{
    public enum CircleType
    {
        None = 0,
        Player,
        NPC,
        End,
    }

    protected List<CharacterBase> owners = new List<CharacterBase>();
    protected AttackCircleStat attackCircleStat = null;

    protected List<GameObject> detectedEnemies = new List<GameObject>();
    protected List<Item> detectedItems = new List<Item>();

    public bool IsUsed { get; private set; } = false;
    [HideInInspector] public CircleType type = CircleType.None;

    public Action<CharacterBase> onDetectEnemy;
    public Action<CharacterBase> onUnDetectEnemy;

    private SphereCollider sphereCollider = null;
    protected PhotonView photonView = null;

    public List<CharacterBase> GetOwners => owners;
    public List<GameObject> GetDetectedEnemies => detectedEnemies;
    public List<Item> GetDetectedItems => detectedItems;

    protected virtual void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        attackCircleStat = GetComponent<AttackCircleStat>();
        photonView = GetComponent<PhotonView>();
        UpdateRadius(attackCircleStat.attackRadius);
    }

    protected virtual void Start()
    {
        if (owners.Count == 0) return;
        transform.position = owners.FirstOrDefault().transform.position;
    }

    public void SetActive(bool isActive) => gameObject.SetActive(isActive);

    public void UpdateIsUsed(bool used) => IsUsed = used;

    public virtual void UpdateOwners(CharacterBase newOwner, bool isMerged)
    {
        if (owners.Contains(newOwner)) return;

        owners.Add(newOwner);
        onDetectEnemy -= newOwner.OnDetectEnemy;
        onDetectEnemy += newOwner.OnDetectEnemy;
        onUnDetectEnemy -= newOwner.OnUnDetectEnemy;
        onUnDetectEnemy += newOwner.OnUnDetectEnemy;
        newOwner.deadAction -= RemoveOwner;
        newOwner.deadAction += RemoveOwner;
    }

    public void RemoveOwner(CharacterBase inOwner)
    {
        owners.Remove(inOwner);

        if (owners.Count == 0)
            SetDead();
    }

    private void SetDead()
    {
        if (PhotonNetwork.IsMasterClient || GameManager.IsTrainingMode)
        {
            GameManager.Instance.itemManager.ShowItem(attackCircleStat.GetCoin(), transform.position, ItemType.Coin);
            GameManager.Instance.itemManager.ShowItem(attackCircleStat.GetGem(), transform.position, ItemType.Gem);
        }

        UpdateIsUsed(false);
        SetActive(false);
    }

    public void UpdateRadius(float newRadius)
    {
        transform.localScale = Vector3.one * newRadius * 2;
    }

    public CharacterBase SpawnCharacter(Vector3 pos, CharacterType charType, CharacterLevel level = CharacterLevel.NPC, bool isMerged = false)
    {
        string path = $"Prefabs/Character/{charType}{level}";

        GameObject character;
        if (GameManager.IsTrainingMode)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[Training] Character Prefab ¾øÀ½: {path}");
                return null;
            }
            character = Instantiate(prefab, pos, Quaternion.identity);
        }
        else
        {
            character = PhotonNetwork.Instantiate(path, pos, Quaternion.identity);
        }

        CharacterBase characterBase = character.GetComponent<CharacterBase>();
        characterBase.gameObject.name += "mine";
        UpdateOwners(characterBase, isMerged);
        return characterBase;
    }

    public virtual void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterBase>(out CharacterBase character))
        {
            if (owners.Contains(character)) return;

            onDetectEnemy?.Invoke(character);

            if (!detectedEnemies.Contains(character.gameObject))
                detectedEnemies.Add(character.gameObject);
        }

        if (other.gameObject.TryGetComponent<MoneyTree>(out MoneyTree moneyTree))
        {
            if (!detectedEnemies.Contains(moneyTree.gameObject))
                detectedEnemies.Add(moneyTree.gameObject);
        }

        if (other.gameObject.TryGetComponent<Item>(out Item item))
        {
            if (!detectedItems.Contains(item))
                detectedItems.Add(item);
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterBase>(out CharacterBase character))
        {
            onUnDetectEnemy?.Invoke(character);
            detectedEnemies.Remove(character.gameObject);
        }

        if (other.gameObject.TryGetComponent<MoneyTree>(out MoneyTree moneyTree))
            detectedEnemies.Remove(moneyTree.gameObject);

        if (other.gameObject.TryGetComponent<Item>(out Item item))
            detectedItems.Remove(item);
    }
}
