using Photon.Pun;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] protected CharacterType characterType;
    protected float repeatInterval = 5f;
    protected string path = null;
    protected GameObject spawnObject = null;

    protected virtual void Awake()
    {
        SetPath($"Prefabs/Character/NPCAttackCircle");
    }

    protected virtual void SetPath(string inPath)
    {
        path = inPath;
    }

    public virtual void StartSpawn()
    {
        InvokeRepeating("Spawn", 0.0f, repeatInterval);
    }

    protected virtual GameObject Spawn()
    {
        if (spawnObject == null)
        {
            GameObject obj = PhotonNetwork.Instantiate(path, transform.position + Vector3.up * 2.6f, Quaternion.identity);
            if(obj.TryGetComponent<NPCAttackCircle>(out NPCAttackCircle npcAttackCircle))
            {
                obj.GetComponent<NPCAttackCircle>().SpawnNPC(characterType);
            }
            return spawnObject = obj;
        }

        return null;
    }
}
