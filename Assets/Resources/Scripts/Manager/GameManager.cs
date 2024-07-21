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
    [HideInInspector] public ProjectileManager projectileManager = null;
    [HideInInspector] public AOEManager aoeManager = null;
    private PhotonView photonView = null;
    public GameObject attackCircle = null;
    public bool isConnect { get; set; } = false;

    public int treasureBoxCost { get; private set; } = 0;
    private int playTime = 240;

    public string userName = "프루니";

    private List<PlayerAttackCircleSpawnPoint> playerSpawnPoints = new List<PlayerAttackCircleSpawnPoint>();

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
        photonView = gameObject.GetComponent<PhotonView>();
        StartCoroutine(CoInitGame());
    }

    private IEnumerator CoInitGame()
    {
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
        projectileManager = FindObjectOfType<ProjectileManager>();
        aoeManager = FindObjectOfType<AOEManager>();

        if(PhotonNetwork.IsMasterClient)
        {
            SpawnCharacter();
        }
        

        SetTreasureBoxCost(treasureBoxCost);
    }

    //게임시작시 최초로 AttackCircle, Player 스폰시키는 함수
    public void SpawnCharacter()
    {
        playerSpawnPoints = FindObjectsOfType<PlayerAttackCircleSpawnPoint>().ToList();

        foreach (var p in PhotonNetwork.PlayerList)
        {
            PlayerAttackCircleSpawnPoint spawnPoint = playerSpawnPoints.Find(p => !p.GetIsAssigned());
            spawnPoint.SetIsAssigned(true);
            Vector3 pos = spawnPoint.gameObject.transform.position;
            photonView.RPC("RPCSpawnCharacter", p, pos);
        }
    }

    [PunRPC]
    public void RPCSpawnCharacter(Vector3 pos)
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
