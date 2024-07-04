using Photon.Pun;
using UnityEngine;

public class SpawnPos : MonoBehaviour
{
    public CharacterType characterType;
    public float repeatInterval = 5f;
    private string path = null;
    private GameObject spawnObject = null;

    public void Awake()
    {
        path = $"Prefabs/Character/{characterType.ToString()}";
        
    }

    public void StartSpawn()
    {
        InvokeRepeating("Spawn", 0.0f, repeatInterval);
    }

    public GameObject Spawn()
    {
        if (spawnObject == null)
        {
            GameObject obj = PhotonNetwork.Instantiate(path, transform.position, Quaternion.identity);

            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.SetActive(true);
            return spawnObject = obj;
        }

        return null;
    }
}
