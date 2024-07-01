using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttackCircle : AttackCircle, IAttackCircleUIInterface
{
    protected GameObject moveObj = null;
    protected Movement3D movement3D = null;
    protected CharacterController characterController = null;

    protected override void Awake()
    {
        base.Awake();
        moveObj = new GameObject($"Move{gameObject.name}");
        movement3D = moveObj.AddComponent<Movement3D>();
        characterController = moveObj.AddComponent<CharacterController>();

        type = circleType.Player;
    }

    protected override void Start()
    {
        base.Start();
        moveObj.transform.position = transform.position;
    }

    protected virtual void Update()
    {
        if((CheckInput()))
        {
            Move();
            transform.position = moveObj.transform.position;
        }
    }

    public override void UpdateOwners(CharacterBase newOwner)
    {
        base.UpdateOwners(newOwner);
        if (owners.LastOrDefault() == newOwner)
        {
            //머지할 수 있는지 검사
            List<CharacterBase> chars = owners.FindAll(o => o.GetCharacterType() == newOwner.GetCharacterType()).ToList();
            if (chars.Count < 3)
            {
                return;
            }

            StartCoroutine(CoMergeCharacter(chars, newOwner.transform.position));
        }
    }

    private IEnumerator CoMergeCharacter(List<CharacterBase> chars, Vector3 pos)
    {
        yield return new WaitForSeconds(0.7f);
        SpawnPlayer(CharacterType.ElPrimo2);
        foreach (var ch in chars)
        {
            ch.SetDead();
        }

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

    public void SelectCharacter(CharacterType newType)
    {
        SpawnPlayer(newType);
    }

    private void SpawnPlayer(CharacterType newType)
    {
        Vector3 pos = Vector3.zero;
        float x = Random.Range(-attackCircleStat.attackRadius + 2, attackCircleStat.attackRadius - 2);
        float z = Random.Range(0, Mathf.Pow(attackCircleStat.attackRadius, 2) - Mathf.Pow(x, 2));
        pos.x = x + transform.position.x;
        pos.z = Random.Range(-Mathf.Sqrt(z) + 2, Mathf.Sqrt(z) - 2) + transform.position.z;
        CharacterBase player = GameManager.Instance.SpawnPlayer(pos, newType);
        player.GetComponent<CharacterPlayer>().SetAttackCircle(this);
        UpdateOwners(player);
    }
}
