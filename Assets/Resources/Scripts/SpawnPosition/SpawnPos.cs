using UnityEngine;

public class SpawnPos : MonoBehaviour
{
    public CharacterType characterType;
    public float repeatInterval = 5f;
    private GameObject origin = null;
    private GameObject spawnObject = null;

    public void Awake()
    {
        origin = Resources.Load($"Prefabs/Character/NPC/{characterType.ToString()}") as GameObject;
    }

    public void Start()
    {
        InvokeRepeating("Spawn", 0.0f, repeatInterval);
    }

    public GameObject Spawn()
    {
        if (spawnObject == null)
        {
            GameObject obj = Instantiate(origin, transform.position, Quaternion.identity);

            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.GetComponent<ICharacterSpawnerInterface>().SetIsDead(false);
            obj.SetActive(true);
            obj.GetComponent<ICharacterSpawnerInterface>().Init();
            return spawnObject = obj;
        }

        return null;
    }
}
