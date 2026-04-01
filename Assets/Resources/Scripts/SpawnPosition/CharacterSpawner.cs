using Photon.Pun;
using UnityEngine;

public class CharacterSpawner : Spawner
{
    [SerializeField] protected CharacterType characterType;

    protected override void Awake()
    {
        base.Awake();
        repeatInterval = 10f;

        if (characterType < CharacterType.Eggy)
            SetPath("Prefabs/Character/PlayerAttackCircle");
        else
            SetPath("Prefabs/Character/NPCAttackCircle");
    }

    protected override GameObject Spawn()
    {
        if (spawnObject != null && spawnObject.activeSelf) return null;

        if (!GameManager.IsTrainingMode)
            photonView.RPC("RPCEffect", RpcTarget.AllBuffered);

        GameObject obj;
        if (GameManager.IsTrainingMode)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[Training] Character Prefab 없음: {path}");
                return null;
            }
            obj = Instantiate(prefab, transform.position, Quaternion.identity);
        }
        else
        {
            obj = PhotonNetwork.Instantiate(path, transform.position, Quaternion.identity);
        }

        if (obj.TryGetComponent<NPCAttackCircle>(out NPCAttackCircle attackCircle))
            attackCircle.SpawnCharacter(transform.position, characterType);

        return spawnObject = obj;
    }
}
