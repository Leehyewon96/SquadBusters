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
    [HideInInspector] public UIManager uiManager = null;

    public GameObject attackCircle { get; private set; } = null;

    public GameObject NPCParent = null;
    [HideInInspector] public List<GameObject> NPCPool = new List<GameObject>();

    public GameObject PlayerParent = null;
    [HideInInspector] private List<GameObject> playerPool = new List<GameObject>();

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
        SpawnCharacter(Vector3.up * 2.26f, CharacterType.Babarian);
    }

    public void InitGame()
    {
        hpBarManager = FindObjectOfType<HpBarManager>();
        attackCircleManager = FindObjectOfType<AttackCircleManager>();
        itemManager = FindObjectOfType<ItemManager>();
        effectManager = FindObjectOfType<EffectManager>();
        uiManager = FindObjectOfType<UIManager>();
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
        attackCircle = circle.gameObject;
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
        GameObject character = playerPool.Find(p => !p.activeSelf && p.GetComponent<CharacterPlayer>().GetCharacterType() == charType);
        if(character == null)
        {
            GameObject InstancingChar = Resources.Load($"Prefabs/Character/Player/{charType.ToString()}") as GameObject;
            character = Instantiate(InstancingChar, pos, Quaternion.identity);
            character.transform.SetParent(PlayerParent.transform);
            playerPool.Add(character);
        }
        
        CharacterBase characterBase = character.GetComponent<CharacterBase>();
        characterBase.SetCharacterType(charType);
        effectManager.AttachStarAura(character);
        character.transform.position = pos;
        character.SetActive(true);

        return characterBase;
    }
}
