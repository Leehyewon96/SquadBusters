using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    [HideInInspector] public ItemManager itemManager = null;
    [HideInInspector] public EffectManager effectManager = null;
    [HideInInspector] public UIManager uiManager = null;

    public GameObject attackCircle = null;
    public bool isConnect { get; set; } = false;

    public int treasureBoxCost { get; private set; } = 3;
    private int playTime = 240; 

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(instance);
        }
    }

    public void Start()
    {
        StartCoroutine(CoInitGame());
    }

    private IEnumerator CoInitGame()
    {
        //yield return new WaitUntil(() => isConnect);
        //yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name.Equals(SceneLocalize.gameScene));
        InitGame();
        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CoStartTimer());
        }
        
    }

    private IEnumerator CoStartTimer()
    {
        while(playTime > 0)
        {
            string newTime = $"{playTime / 60} : {playTime % 60}";

            GetComponent<PhotonView>().RPC("RPCUpdateTimer", RpcTarget.AllBuffered, newTime);
            yield return new WaitForSecondsRealtime(1f);
            playTime -= 1;
        }
    }

    [PunRPC]
    public void RPCUpdateTimer(string newTime)
    {
        if(uiManager == null)
        {
            return;
        }

        uiManager.timeUI.UpdateTime(newTime);
    }

    public void InitGame()
    {
        //hpBarManager = FindObjectOfType<HpBarManager>();
        //attackCircleManager = FindObjectOfType<AttackCircleManager>();
        itemManager = FindObjectOfType<ItemManager>();
        effectManager = FindObjectOfType<EffectManager>();
        uiManager = FindObjectOfType<UIManager>();

        List<NPCSpawnPoint> npcSpawnPoses = FindObjectsOfType<NPCSpawnPoint>().ToList();
        List<TreasureBoxSpawnPoint> treasureBoxSpawnPoses = FindObjectsOfType<TreasureBoxSpawnPoint>().ToList();

        string npcSpawnerPath = $"Prefabs/Spawner/NPCSpawner";
        foreach (var spawnPoint in npcSpawnPoses)
        {
            GameObject spawner = PhotonNetwork.Instantiate(npcSpawnerPath, spawnPoint.gameObject.transform.position, spawnPoint.transform.rotation);
            if(PhotonNetwork.IsMasterClient)
            {
                spawner.GetComponent<Spawner>().StartSpawn();
            }
            
        }

        string tbSpawnerPath = $"Prefabs/Spawner/TreasureBoxSpawner";
        Debug.Log($"treasureBoxSpawnPoses {treasureBoxSpawnPoses.Count}");
        foreach (var spawnPoint in treasureBoxSpawnPoses)
        {
            GameObject spawner = PhotonNetwork.Instantiate(tbSpawnerPath, spawnPoint.gameObject.transform.position, spawnPoint.transform.rotation);
            
            if (PhotonNetwork.IsMasterClient)
            {
                spawner.GetComponent<TreasureBoxSpawner>().StartSpawn();
            }

        }

        SpawnCharacter(Vector3.up * 2.26f, CharacterType.ElPrimo);

        SetTreasureBoxCost(treasureBoxCost);
    }

    //게임시작시 최초로 AttackCircle, Player 스폰시키는 함수
    public void SpawnCharacter(Vector3 pos, CharacterType chartype)
    {
        string path = $"Prefabs/Character/PlayerAttackCircle";
        attackCircle = PhotonNetwork.Instantiate(path, pos, Quaternion.identity);
        Camera.main.GetComponent<CameraFollow>().SetTarget(attackCircle.gameObject);
        PlayerAttackCircle circle = attackCircle.GetComponent<PlayerAttackCircle>();
    }

    public void PauseGame()
    {

    }

    public void ContinueGame()
    {

    }

    public void RestartGame()
    {

    }

    public void StopGame()
    {

    }

    public void SetTreasureBoxCost(int newCost)
    {
        treasureBoxCost = newCost;
        uiManager.treasureBoxCostUI.SetBoxCostText(treasureBoxCost);
    }
}
