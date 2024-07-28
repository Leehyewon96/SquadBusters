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
    [HideInInspector] public SoundManager soundManager = null;
    private PhotonView photonView = null;
    public GameObject attackCircle = null;
    public bool isConnect { get; set; } = false;
    public bool endGame { get; set; } = false;

    public int treasureBoxCost { get; private set; } = 0;
    private int playTime = 240;

    public string userName = "�����";

    private List<PlayerAttackCircleSpawnPoint> playerSpawnPoints = new List<PlayerAttackCircleSpawnPoint>();
    Dictionary<string, int> rankDic = new Dictionary<string, int>();

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
        soundManager = FindObjectOfType<SoundManager>();
        soundManager.Play(SoundEffectType.LobbyBG);
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

        playTime = 0;
        
        var rankList = rankDic.OrderByDescending(r => r.Value).ToList();
        int order = 1;
        for (int i = 0; i < rankList.Count; ++i)
        {
            if(i > 0 && rankList[i].Value < rankList[i - 1].Value)
            {
                order++;
            }
            photonView.RPC("RPCUpdateEndingUI", RpcTarget.AllBuffered, rankList[i].Key, rankList[i].Value.ToString(), order.ToString());
        }

        photonView.RPC("StopGame", RpcTarget.AllBuffered);
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
        soundManager.Stop(SoundEffectType.LobbyBG);
        soundManager.Play(SoundEffectType.InGameBG);

        //hpBarManager = FindObjectOfType<HpBarManager>();
        //attackCircleManager = FindObjectOfType<AttackCircleManager>();
        itemManager = FindObjectOfType<ItemManager>();
        effectManager = FindObjectOfType<EffectManager>();
        uiManager = FindObjectOfType<UIManager>();
        projectileManager = FindObjectOfType<ProjectileManager>();
        aoeManager = FindObjectOfType<AOEManager>();

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnCharacter();
        }

        UpdateRank(userName, 0);
        SetTreasureBoxCost(treasureBoxCost);
    }

    //���ӽ��۽� ���ʷ� AttackCircle, Player ������Ű�� �Լ�
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

    public void UpdateRank(string name, int gemCnt)
    {
        photonView.RPC("RPCUpdateRank", RpcTarget.MasterClient, name, gemCnt);
    }

    [PunRPC]
    public void RPCUpdateRank(string name, int gemCnt)
    {
        if(photonView.IsMine)
        {
            if(!rankDic.ContainsKey(name))
            {
                rankDic.Add(name, gemCnt);
            }
            else
            {
                rankDic[name] = gemCnt;
            }

            //����
            var rank = rankDic.OrderByDescending(r => r.Value).ToList();

            int order = 1;
            for (int i = 0; i < rank.Count; ++i)
            {
                
                if(i > 0 && rank[i].Value < rank[i - 1].Value)
                {
                    order++;
                }

                photonView.RPC("RPCUpdateRankUI", RpcTarget.AllBuffered, rank[i].Key, rank[i].Value.ToString(), order.ToString());
            }
            
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
        if(inName == userName)
        {
            uiManager.rankUI.UpdateMyRank(rank);
        }
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

    [PunRPC]
    public void StopGame()
    {
        endGame = true;
        soundManager.Stop(SoundEffectType.InGameBG);
        soundManager.Play(SoundEffectType.EndingBG);
        uiManager.OnStopGame();
    }

    [PunRPC]
    public void RPCUpdateEndingUI(string inName, string gemCnt, string rank)
    {
        Debug.Log("RPCUpdateEndingUI");
        uiManager.endingUI.UpdateRank(inName, gemCnt, rank);
    }

    public void SetTreasureBoxCost(int newCost)
    {
        treasureBoxCost = newCost;
        uiManager.treasureBoxCostUI.SetBoxCostText(treasureBoxCost);
    }
}
