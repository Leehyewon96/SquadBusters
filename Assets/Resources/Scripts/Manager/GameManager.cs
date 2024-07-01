using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    [HideInInspector] public HpBarManager hpBarManager = null;
    [HideInInspector] public AttackCircleManager attackCircleManager = null;
    [HideInInspector] public ItemManager itemManager = null;
    [HideInInspector] public EffectManager effectManager = null;
    [HideInInspector] public UIManager uiManager = null;

    public GameObject attackCircle { get; private set; } = null;

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
        SpawnCharacter(Vector3.up * 2.26f, CharacterType.ElPrimo);
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

    }

    //게임시작시 최초로 AttackCircle, Player 스폰시키는 함수
    public void SpawnCharacter(Vector3 pos, CharacterType chartype)
    {
        CharacterBase newPlayer = SpawnPlayer(pos, chartype);
        AttackCircle circle = attackCircleManager.GetAttackCircle(AttackCircle.circleType.Player);
        attackCircle = circle.gameObject;
        Camera.main.GetComponent<CameraFollow>().SetTarget(circle.gameObject);
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
        GameObject InstancingChar = Resources.Load($"Prefabs/Character/Player/{charType.ToString()}") as GameObject;
        GameObject character = Instantiate(InstancingChar, pos, Quaternion.identity);

        CharacterBase characterBase = character.GetComponent<CharacterBase>();
        characterBase.SetCharacterType(charType);
        effectManager.AttachStarAura(character);
        character.transform.position = pos;
        character.SetActive(true);

        return characterBase;
    }
}
