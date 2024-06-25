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

    public GameObject Spawn(Vector3 pos)
    {
        return Instantiate(player, pos, Quaternion.identity);
    }
}
