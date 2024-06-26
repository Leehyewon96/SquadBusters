using UnityEngine;

public class SpawnPos : MonoBehaviour
{
    public CharacterType characterType;
    public float repeatInterval = 5f;
    private GameObject spawnObject = null;

    public void Start()
    {
        InvokeRepeating("Spawn", 0.0f, repeatInterval);
    }

    public GameObject Spawn()
    {
        if (spawnObject == null || spawnObject.GetComponent<ICharacterSpawnerInterface>().GetIsDead())
        {
            GameObject obj = GameManager.Instance.NPCPool.Find(n => !n.activeSelf 
            && n.GetComponent<ICharacterSpawnerInterface>().GetCharacterType() == characterType);
            if(obj == null)
            {
                GameObject origin = GameManager.Instance.NPCPool.Find(n => n.GetComponent<ICharacterSpawnerInterface>().GetCharacterType() == characterType);
                obj = Instantiate(origin, transform.position, Quaternion.identity);
                obj.transform.SetParent(GameManager.Instance.NPCParent.transform);
                GameManager.Instance.NPCPool.Add(obj);
            }

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
