using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    //[HideInInspector] public HpBarManager hpBarManager = null;
    //[HideInInspector] public AttackCircleManager attackCircleManager = null;
    [HideInInspector] public ItemManager itemManager = null;
    [HideInInspector] public EffectManager effectManager = null;
    [HideInInspector] public UIManager uiManager = null;

    public GameObject attackCircle = null;
    public bool isConnect { get; set; } = false;

    private List<SpawnPos> spawnPoses;

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
    }

    public void Start()
    {
        StartCoroutine(CoInitGame());
    }

    private IEnumerator CoInitGame()
    {
        yield return new WaitUntil(() => isConnect);
        yield return new WaitForSeconds(2f);
        Debug.Log("게임씬 초기화.");
        InitGame();
        
    }

    public void InitGame()
    {
        //hpBarManager = FindObjectOfType<HpBarManager>();
        //attackCircleManager = FindObjectOfType<AttackCircleManager>();
        itemManager = FindObjectOfType<ItemManager>();
        effectManager = FindObjectOfType<EffectManager>();
        uiManager = FindObjectOfType<UIManager>();
        spawnPoses = FindObjectsOfType<SpawnPos>().ToList();
        spawnPoses.ForEach(s => s.StartSpawn());

        SpawnCharacter(Vector3.up * 2.26f, CharacterType.ElPrimo);
    }

    //게임시작시 최초로 AttackCircle, Player 스폰시키는 함수
    public void SpawnCharacter(Vector3 pos, CharacterType chartype)
    {
        //CharacterBase newPlayer = SpawnPlayer(pos, chartype);
        string path = $"Prefabs/Character/PlayerAttackCircle";
        attackCircle = PhotonNetwork.Instantiate(path, pos, Quaternion.identity);
        Camera.main.GetComponent<CameraFollow>().SetTarget(attackCircle.gameObject);
        PlayerAttackCircle circle = attackCircle.GetComponent<PlayerAttackCircle>();
        //circle.UpdateOwners(newPlayer);
        circle.UpdateRadius(4f); // Localize 시키기
        //newPlayer.GetComponent<CharacterPlayer>().SetAttackCircle(circle);
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
        //GameObject InstancingChar = Resources.Load($"Prefabs/Character/Player/{charType.ToString()}") as GameObject;
        string path = $"Prefabs/Character/Player/{charType.ToString()}";
        GameObject character = PhotonNetwork.Instantiate(path, pos, Quaternion.identity);

        CharacterBase characterBase = character.GetComponent<CharacterBase>();
        characterBase.SetCharacterType(charType);
        effectManager.AttachStarAura(character);
        character.transform.position = pos;
        character.SetActive(true);

        return characterBase;
    }
}
