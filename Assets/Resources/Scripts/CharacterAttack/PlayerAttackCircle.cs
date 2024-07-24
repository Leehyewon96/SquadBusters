using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static CharacterPlayer;

public class PlayerAttackCircle : AttackCircle, IAttackCircleUIInterface, IAttackCircleItemInterface
{
    protected GameObject moveObj = null;
    protected Movement3D movement3D = null;
    protected CharacterController characterController = null;

    [SerializeField] protected ParticleSystem circleEffect = null;
    protected ParticleSystem.MainModule mainCircleEffect;

    [SerializeField] protected TextMeshProUGUI userName = null;
    [SerializeField] protected TextMeshProUGUI gemCnt = null;

    protected override void Awake()
    {
        base.Awake();
        moveObj = new GameObject($"Move{gameObject.name}");
        movement3D = moveObj.AddComponent<Movement3D>();
        characterController = moveObj.AddComponent<CharacterController>();
        type = circleType.Player;
        mainCircleEffect = circleEffect.main;
        attackCircleStat.SetCoin(0);
        attackCircleStat.SetGem(0);

        circleEffect.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        moveObj.transform.position = transform.position;

        if(photonView.IsMine)
        {
            GameManager.Instance.uiManager.fastMoveUI.onMoveFast -= () => movement3D.UpdateMoveSpeed(15f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveFast += () => movement3D.UpdateMoveSpeed(15f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon -= () => movement3D.UpdateMoveSpeed(7.5f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon += () => movement3D.UpdateMoveSpeed(7.5f);
            GameManager.Instance.uiManager.skillUI.doSkill += DoItemSkill;

            photonView.RPC("SetUserName", RpcTarget.AllBuffered, GameManager.Instance.userName);
            photonView.RPC("UpdateGemCnt", RpcTarget.AllBuffered, attackCircleStat.GetGem());
        }
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.endGame)
        {
            return;
        }

        if (!photonView.IsMine)
        {
            return;
        }

        SetCircleColor(CheckInput());

        if (CheckInput())
        {
            Move();
            transform.position = moveObj.transform.position;
        }
        foreach (var owner in owners)
        {
            if(owner.gameObject.activeSelf)
            {
                //owner.SetDestination(moveObj.transform.position);
                owner.SetDestinationPos(moveObj.transform.position);
            }            
        }
    }

    [PunRPC]
    public virtual void SetUserName(string username)
    {
        userName.SetText(username);
    }

    [PunRPC]
    public virtual void UpdateGemCnt(int cnt)
    {
        gemCnt.SetText(cnt.ToString());
    }

    public override void UpdateOwners(CharacterBase newOwner, bool isMerged)
    {
        base.UpdateOwners(newOwner, isMerged);

        if (owners.LastOrDefault() == newOwner)
        {
            if (newOwner.gameObject.TryGetComponent<CharacterPlayer>(out CharacterPlayer player))
            {
                OnTakeItem takeCoin = GainCoin;
                player.AddTakeItemActions(takeCoin);
                OnTakeItem takeGem = GainGem;
                player.AddTakeItemActions(takeGem);
                //OnTakeItem takeTreasureBox = GainTreasureBox;
                //player.AddTakeItemActions(takeTreasureBox);
                OnTakeItem takeBomb = GainBomb;
                player.AddTakeItemActions(takeBomb);

                player.updateCoin = SetCoin;
                player.totalCoin = GetCoin;
                player.onStun = Stun;
            }

            if(!isMerged)
            {
                return;
            }

            //머지할 수 있는지 검사
            if(newOwner.GetCharacterLevel() == CharacterLevel.End - 1)
            {
                return;
            }
            List<CharacterBase> chars = owners.FindAll(o => o.gameObject.activeSelf &&
            o.GetCharacterType() == newOwner.GetCharacterType() &&
            o.GetCharacterLevel() == newOwner.GetCharacterLevel()).ToList();
            if (chars.Count < 3)
            {
                return;
            }

            StartCoroutine(CoMergeCharacter(chars, newOwner.transform.position));
        }
    }

    private IEnumerator CoMergeCharacter(List<CharacterBase> chars, Vector3 pos)
    {
        yield return new WaitForSeconds(0.3f);
        CharacterType type = chars.FirstOrDefault().GetCharacterType();
        CharacterLevel nextLevel = chars.FirstOrDefault().GetCharacterLevel() + 1;
        foreach (var ch in chars)
        {
            //ch.Merged();
            ch.SetDead();
        }
        SpawnCharacter(transform.position, type, nextLevel, false);
        

        owners.LastOrDefault().transform.position = pos;
    }

    protected virtual void Move()
    {
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        movement3D.Move(x, z);
    }

    protected virtual bool CheckInput()
    {
        //모바일에서 터치로 변경
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            characterController.enabled = true;
            return true;
        }

        characterController.enabled = false;
        return false;
    }

    protected virtual void SetCircleColor(bool isMoving)
    {
        if (isMoving)
        {
            mainCircleEffect.startColor = Color.blue;
        }
        else
        {
            mainCircleEffect.startColor = Color.red;
        }
    }

    public void GainCoin()
    {
        if(photonView.IsMine)
        {
            attackCircleStat.SetCoin(attackCircleStat.GetCoin() + 1);
            GameManager.Instance.uiManager.coinUI.SetCoin(attackCircleStat.GetCoin());
        }
    }

    public void GainGem()
    {
        if (photonView.IsMine)
        {
            attackCircleStat.SetGem(attackCircleStat.GetGem() + 1);
            GameManager.Instance.UpdateRank(GameManager.Instance.userName, attackCircleStat.GetGem());
            photonView.RPC("UpdateGemCnt", RpcTarget.AllBuffered, attackCircleStat.GetGem());
        }
    }

    public void GainBomb()
    {
        GameManager.Instance.uiManager.skillUI.UpdateSkillType(ItemType.Bomb);
        GameManager.Instance.uiManager.skillUI.SetInteractable(true);
    }

    public int GetCoin()
    {
        return attackCircleStat.GetCoin();
    }

    public void SetCoin(int newCoin)
    {
        attackCircleStat.SetCoin(newCoin);
        GameManager.Instance.uiManager.coinUI.SetCoin(newCoin);
    }

    public virtual void GainTreasureBox()
    {
        GameManager.Instance.uiManager.ShowUI(UIType.SelectCharacter, true);
    }

    public void OnDetectedItem(NoticeType type, Item tree)
    {
        var gregs = owners.FindAll(o => o.GetCharacterType().Equals(CharacterType.Greg));
        if(gregs.Count == 0)
        {
            if(photonView.IsMine)
            {
                ShowNotice(type, tree);
            }
            
            return;
        }

        gregs.ForEach(g => g.GetComponent<Greg>().OnDetectedMoneyTree(tree));
    }

    public void OnUnDetectedItem(Item tree)
    {
        var gregs = owners.FindAll(o => o.GetCharacterType().Equals(CharacterType.Greg));
        if (gregs.Count == 0)
        {
            return;
        }

        gregs.ForEach(g => g.GetComponent<Greg>().OnUnDetectedMoneyTree(tree));

    }

    protected virtual void Stun(float stunTime)
    {
        StartCoroutine(CoStun(stunTime));
    }

    protected IEnumerator CoStun(float stunTime)
    {
        movement3D.UpdateMoveSpeed(0.5f);
        yield return new WaitForSeconds(stunTime);
        movement3D.UpdateMoveSpeed(15f);
    }

    private void ShowNotice(NoticeType type, Item item)
    {
        NoticeElem noticeElem = GameManager.Instance.uiManager.noticeUI.ShowAcitveNotice(type, true, item.gameObject);
        item.onUndetectedPlayerAttack -= delegate { noticeElem.SetActive(false); };
        item.onUndetectedPlayerAttack += delegate { noticeElem.SetActive(false); };
    }

    #region IAttackCircleUIInterface
    public void SelectCharacter(CharacterType newType, CharacterLevel newLevel)
    {
        Vector3 pos = Vector3.zero;
        float x = Random.Range(-attackCircleStat.attackRadius + 2, attackCircleStat.attackRadius - 2);
        float z = Random.Range(0, Mathf.Pow(attackCircleStat.attackRadius, 2) - Mathf.Pow(x, 2));
        pos.x = x + transform.position.x;
        pos.z = Random.Range(-Mathf.Sqrt(z) + 2, Mathf.Sqrt(z) - 2) + transform.position.z;
        CharacterBase player = SpawnCharacter(pos, newType, newLevel, true);

        if (photonView.IsMine)
        {
            circleEffect.gameObject.SetActive(true);
        }
    }

    public void DoItemSkill(ItemType itemType)
    {
        Projectile projectile = GameManager.Instance.projectileManager.GetProjectile(transform.position, ProjectileType.Bomb);
        Bomb bomb = projectile.gameObject.GetComponent<Bomb>();
        bomb.Explode(2f);
    }
    #endregion

    
}
