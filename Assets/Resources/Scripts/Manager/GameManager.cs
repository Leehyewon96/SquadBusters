using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    [HideInInspector] public HpBarManager hpBarManager = null;
    [HideInInspector] public AttackCircleManager attackCircleManager = null;
    [HideInInspector] public ItemManager itemManager = null;
    [HideInInspector] public EffectManager effectManager = null;

    [SerializeField] private GameObject player = null;

    public GameObject NPCParent = null;
    [HideInInspector] public List<GameObject> NPCPool = new List<GameObject>();

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

        //SpawnPoses = FindObjectOfType<Spawn>
    }

    public void Start()
    {
        SpawnCharacter(Vector3.zero, CharacterType.ElPrimo);
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

        List<CharacterNonPlayer> NPCTemp = NPCParent.GetComponentsInChildren<CharacterNonPlayer>(true).ToList();

        NPCTemp.ForEach(n => NPCPool.Add(n.gameObject));
    }

    //게임시작시 최초로 AttackCircle, Player 스폰시키는 함수
    public void SpawnCharacter(Vector3 pos, CharacterType chartype)
    {
        CharacterBase newPlayer = SpawnPlayer(pos, chartype);
        AttackCircle circle = attackCircleManager.GetAttackCircle(AttackCircle.circleType.Player);
        Camera.main.GetComponent<CameraFollow>().SetTarget(circle.gameObject);
        circle.UpdateLayer(LayerLocalize.playerAttackCircle);
        circle.UpdateOwners(newPlayer);
        circle.UpdateRadius(4f); // Localize 시키기
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

    public CharacterBase SpawnPlayer(Vector3 pos, CharacterType charType)
    {
        GameObject character = Instantiate(player, pos, Quaternion.identity);
        CharacterBase characterBase = character.GetComponent<CharacterBase>();
        characterBase.SetCharacterType(charType);
        effectManager.AttachStarAura(character);
        return characterBase;
    }
}
