using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [HideInInspector] public ItemManager itemManager;
    [HideInInspector] public EffectManager effectManager;
    [HideInInspector] public UIManager uiManager;
    [HideInInspector] public ProjectileManager projectileManager;
    [HideInInspector] public AOEManager aoeManager;
    [HideInInspector] public SoundManager soundManager;
    private PhotonView photonView;
    public GameObject attackCircle;
    public bool isConnect { get; set; } = false;
    public bool endGame { get; set; } = false;

    public int treasureBoxCost { get; private set; } = 0;
    private int playTime = 3600;

    public string userName = "프루니";

    private List<PlayerAttackCircleSpawnPoint> playerSpawnPoints = new List<PlayerAttackCircleSpawnPoint>();
    private Dictionary<string, int> rankDic = new Dictionary<string, int>();

    [HideInInspector] public List<GameObject> EnemiesInFullRange = new List<GameObject>();
    [HideInInspector] public const int MAX_UNITS = 20;
    [HideInInspector] public const int MAX_ENEMIES = 20;

    [Header("강화학습 설정")]
    [SerializeField] private bool isTrainingMode = false;
    public static bool IsTrainingMode { get; private set; } = false;

    public static GameManager Instance
    {
        get
        {
            if (instance == null) return null;
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
            Destroy(gameObject);
        }

        IsTrainingMode = isTrainingMode;
    }

    public void Start()
    {
        photonView = gameObject.GetComponent<PhotonView>();
        soundManager = FindObjectOfType<SoundManager>();

        if (!IsTrainingMode)
            soundManager.Play(SoundEffectType.LobbyBG);

        StartCoroutine(CoInitGame());
    }

    private IEnumerator CoInitGame()
    {
        if (IsTrainingMode)
        {
            InitGame();
            yield break;
        }

        yield return new WaitUntil(() => SceneManager.GetActiveScene().name.Equals(SceneLocalize.gameScene));
        InitGame();

        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(CoStartTimer());
    }

    private IEnumerator CoStartTimer()
    {
        while (playTime > 0)
        {
            string newTime = $"{playTime / 60} : {playTime % 60}";

            if (IsTrainingMode)
                RPCUpdateTimer(newTime);
            else
                GetComponent<PhotonView>().RPC("RPCUpdateTimer", RpcTarget.AllBuffered, newTime);

            yield return new WaitForSecondsRealtime(1f);
            playTime -= 1;
        }

        playTime = 0;

        var rankList = rankDic.OrderByDescending(r => r.Value).ToList();
        int order = 1;
        for (int i = 0; i < rankList.Count; ++i)
        {
            if (i > 0 && rankList[i].Value < rankList[i - 1].Value)
                order++;

            if (IsTrainingMode)
                RPCUpdateEndingUI(rankList[i].Key, rankList[i].Value.ToString(), order.ToString());
            else
                photonView.RPC("RPCUpdateEndingUI", RpcTarget.AllBuffered, rankList[i].Key, rankList[i].Value.ToString(), order.ToString());
        }

        if (IsTrainingMode)
            StopGame();
        else
            photonView.RPC("StopGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPCUpdateTimer(string newTime)
    {
        if (uiManager == null) return;
        uiManager.timeUI.UpdateTime(newTime);
    }

    public void InitGame()
    {
        if (!IsTrainingMode)
        {
            soundManager.Stop(SoundEffectType.LobbyBG);
            soundManager.Play(SoundEffectType.InGameBG);
        }

        itemManager = FindObjectOfType<ItemManager>();
        effectManager = FindObjectOfType<EffectManager>();
        uiManager = FindObjectOfType<UIManager>();
        projectileManager = FindObjectOfType<ProjectileManager>();
        aoeManager = FindObjectOfType<AOEManager>();

        if (IsTrainingMode)
            SpawnCharacterTraining();
        else if (PhotonNetwork.IsMasterClient)
            SpawnCharacter();

        UpdateRank(userName, 0);
        SetTreasureBoxCost(treasureBoxCost);
    }

    private void SpawnCharacterTraining()
    {
        playerSpawnPoints = FindObjectsOfType<PlayerAttackCircleSpawnPoint>().ToList();
        PlayerAttackCircleSpawnPoint spawnPoint = playerSpawnPoints.FirstOrDefault();
        if (spawnPoint == null)
        {
            Debug.LogError("[Training] SpawnPoint가 없습니다. 씬에 PlayerAttackCircleSpawnPoint를 배치해주세요.");
            return;
        }

        Vector3 pos = spawnPoint.gameObject.transform.position;
        RPCSpawnCharacterTraining(pos);
    }

    private void RPCSpawnCharacterTraining(Vector3 pos)
    {
        string path = "Prefabs/Character/PlayerAttackCircle";
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"[Training] Prefab을 찾을 수 없습니다: {path}");
            return;
        }
        attackCircle = Instantiate(prefab, pos, Quaternion.identity);
        Camera.main.GetComponent<CameraFollow>().SetTarget(attackCircle.gameObject);
    }

    public void SpawnCharacter()
    {
        playerSpawnPoints = FindObjectsOfType<PlayerAttackCircleSpawnPoint>().ToList();
        foreach (var p in PhotonNetwork.PlayerList)
        {
            PlayerAttackCircleSpawnPoint spawnPoint = playerSpawnPoints.Find(sp => !sp.GetIsAssigned());
            spawnPoint.SetIsAssigned(true);
            Vector3 pos = spawnPoint.gameObject.transform.position;
            photonView.RPC("RPCSpawnCharacter", p, pos);
        }
    }

    [PunRPC]
    public void RPCSpawnCharacter(Vector3 pos)
    {
        string path = "Prefabs/Character/PlayerAttackCircle";
        attackCircle = PhotonNetwork.Instantiate(path, pos, Quaternion.identity);
        Camera.main.GetComponent<CameraFollow>().SetTarget(attackCircle.gameObject);
    }

    public void UpdateRank(string name, int gemCnt)
    {
        if (IsTrainingMode)
        {
            RPCUpdateRank(name, gemCnt);
            return;
        }
        photonView.RPC("RPCUpdateRank", RpcTarget.MasterClient, name, gemCnt);
    }

    [PunRPC]
    public void RPCUpdateRank(string name, int gemCnt)
    {
        bool canUpdate = IsTrainingMode || (photonView != null && photonView.IsMine);
        if (!canUpdate) return;

        rankDic[name] = gemCnt;

        var rank = rankDic.OrderByDescending(r => r.Value).ToList();
        int order = 1;
        for (int i = 0; i < rank.Count; ++i)
        {
            if (i > 0 && rank[i].Value < rank[i - 1].Value)
                order++;

            if (IsTrainingMode)
                RPCUpdateRankUI(rank[i].Key, rank[i].Value.ToString(), order.ToString());
            else
                photonView.RPC("RPCUpdateRankUI", RpcTarget.AllBuffered, rank[i].Key, rank[i].Value.ToString(), order.ToString());
        }
    }

    [PunRPC]
    public void RPCUpdateRankUI(string inName, string gemCnt, string rank)
    {
        StartCoroutine(CoUpdateRank(inName, gemCnt, rank));
    }

    private IEnumerator CoUpdateRank(string inName, string gemCnt, string rank)
    {
        yield return new WaitUntil(() => uiManager != null);
        uiManager.rankUI.UpdateRank(inName, gemCnt, rank);
        if (inName == userName)
            uiManager.rankUI.UpdateMyRank(rank);
    }

    [PunRPC]
    public void StopGame()
    {
        endGame = true;
        if (!IsTrainingMode)
        {
            soundManager.Stop(SoundEffectType.InGameBG);
            soundManager.Play(SoundEffectType.EndingBG);
        }
        uiManager.OnStopGame();
    }

    [PunRPC]
    public void RPCUpdateEndingUI(string inName, string gemCnt, string rank)
    {
        uiManager.endingUI.UpdateRank(inName, gemCnt, rank);
    }

    public void SetTreasureBoxCost(int newCost)
    {
        treasureBoxCost = newCost;
        uiManager.treasureBoxCostUI.SetBoxCostText(treasureBoxCost);
    }
}
