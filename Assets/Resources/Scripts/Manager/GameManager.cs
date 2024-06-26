using UnityEngine;

public enum CharacterType
{ 
    ElPrimo,
    Babarian,
}


public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    [HideInInspector] public HpBarManager hpBarManager = null;
    [HideInInspector] public AttackCircleManager attackCircleManager = null;
    [HideInInspector] public ItemManager itemManager = null;
    [HideInInspector] public EffectManager effectManager = null;

    [SerializeField] private GameObject player = null;
    [SerializeField] private AttackCircle attackCircle = null;

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
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(instance);
        }

        InitGame();
    }

    public void Start()
    {
        SpawnCharacter();
    }

    public void InitGame()
    {
        hpBarManager = FindObjectOfType<HpBarManager>();
        attackCircleManager = FindObjectOfType<AttackCircleManager>();
        itemManager = FindObjectOfType<ItemManager>();
        effectManager = FindObjectOfType<EffectManager>();
        if (attackCircleManager != null)
        {
            attackCircleManager.InitAttackCircles();
        }

        
        
    }

    public void SpawnCharacter()
    {
        //AttackCircle (플레이어) 스폰
        GameObject newPlayer = SpawnPlayer(Vector3.zero);
        AttackCircle circle = attackCircleManager.GetAttackCircle(AttackCircle.circleType.Player);
        Camera.main.GetComponent<CameraFollow>().SetTarget(circle.gameObject);
        circle.UpdateLayer(LayerLocalize.playerAttackCircle);
        circle.UpdateOwners(newPlayer);
        circle.UpdateRadius(4f);
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

    public GameObject SpawnPlayer(Vector3 pos)
    {
        return Instantiate(player, pos, Quaternion.identity);
    }
}
