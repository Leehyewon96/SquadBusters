using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CharacterPlayer;

public class PlayerAttackCircle : AttackCircle, IAttackCircleUIInterface
{
    protected GameObject moveObj = null;
    protected Movement3D movement3D = null;
    protected CharacterController characterController = null;

    [SerializeField] protected ParticleSystem circleEffect = null;
    protected ParticleSystem.MainModule mainCircleEffect;

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

        if (photonView.IsMine)
        {
            CharacterBase character = SpawnPlayer(transform.position, CharacterType.ElPrimo, false);
        }
    }

    protected override void Start()
    {
        moveObj.transform.position = transform.position;

        if(photonView.IsMine)
        {
            GameManager.Instance.uiManager.fastMoveUI.onMoveFast -= () => movement3D.UpdateMoveSpeed(30f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveFast += () => movement3D.UpdateMoveSpeed(30f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon -= () => movement3D.UpdateMoveSpeed(15f);
            GameManager.Instance.uiManager.fastMoveUI.onMoveCommon += () => movement3D.UpdateMoveSpeed(15f);
        }
        
        if (!photonView.IsMine)
        {
            circleEffect.gameObject.SetActive(false);
        }
    }

    protected virtual void Update()
    {
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
                owner.SetDestination(moveObj.transform.position);
            }            
        }
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
                OnTakeItem takeTreasureBox = GainTreasureBox;
                player.AddTakeItemActions(takeTreasureBox);
                player.updateCoin = SetCoin;
                player.totalCoin = GetCoin;
            }

            if(!isMerged)
            {
                return;
            }

            //머지할 수 있는지 검사
            List<CharacterBase> chars = owners.FindAll(o => o.gameObject.activeSelf && o.GetCharacterType() == newOwner.GetCharacterType()).ToList();
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
        foreach (var ch in chars)
        {
            //ch.Merged();
            ch.SetDead();
        }
        SpawnPlayer(transform.position, CharacterType.ElPrimo2, false);
        

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
        }
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
        GameManager.Instance.uiManager.ShowUI(UIType.SelectCharacter);
    }

    #region IAttackCircleUIInterface
    public void SelectCharacter(CharacterType newType)
    {
        Vector3 pos = Vector3.zero;
        float x = Random.Range(-attackCircleStat.attackRadius + 2, attackCircleStat.attackRadius - 2);
        float z = Random.Range(0, Mathf.Pow(attackCircleStat.attackRadius, 2) - Mathf.Pow(x, 2));
        pos.x = x + transform.position.x;
        pos.z = Random.Range(-Mathf.Sqrt(z) + 2, Mathf.Sqrt(z) - 2) + transform.position.z;
        CharacterBase player = SpawnPlayer(pos, newType, true);
        //UpdateOwners(player, false);
    }
    #endregion
}
