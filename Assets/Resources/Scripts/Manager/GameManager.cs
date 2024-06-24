using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    [HideInInspector] public HpBarManager hpBarManager = null;
    [HideInInspector] public AttackCircleManager attackCircleManager = null;
    [HideInInspector] public RewardManager rewardManager = null;
    [HideInInspector] public EffectManager effectManager = null;

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
        rewardManager = FindObjectOfType<RewardManager>();
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


}
