using Photon.Pun;
using System.Collections;
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

    public GameObject attackCircle = null;
    public bool isConnect { get; set; } = false;

    public int treasureBoxCost { get; private set; } = 3;
    private int playTime = 240;

    public string userName = "프루니";

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
