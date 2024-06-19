using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected Animator animator = null;
    protected CharacterController characterController = null;
    protected Movement3D movement3D = null;
    protected CharacterStat characterStat = null;
   
    protected HpBar hpBar = null;
    
    protected float attackRange = 2f;
    protected float attackRadius = 1f;
    protected float attackDamage = 30f;

    private List<GameObject> DetectedEnemies = new List<GameObject>();

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        characterStat = GetComponent<CharacterStat>();
        characterController = GetComponent<CharacterController>();
        hpBar = GetComponentInChildren<HpBar>();
        movement3D = GetComponent<Movement3D>();

    }

    protected virtual void Start()
    {
        hpBar.SetMaxHp(characterStat.GetMaxHp());
        hpBar.UpdateCurrentHp(characterStat.GetCurrentHp());
        
        characterStat.onCurrentHpChanged += hpBar.UpdateCurrentHp;
        characterStat.onCurrentHpZero += SetDead;
        characterStat.onCharacterBeginAttack += MoveToEnemy;
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void Move()
    {
        
    }

    

    protected virtual void Attack(GameObject target)
    {
        StopCoroutine(CoMoveToEnemy(target));

        animator.SetBool(AnimLocalize.contactEnemy, true);
        StartCoroutine(CoAttack(target));
    }

    private IEnumerator CoAttack(GameObject target)
    {
        while (true)
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, attackRadius, transform.forward, out hit, attackRange))
            {
                if (hit.collider.gameObject != this && hit.collider.gameObject.TryGetComponent<CharacterBase>(out CharacterBase hitObj))
                {
                    hitObj.TakeDamage(attackDamage);
                }
            }
            else
            {
                animator.SetBool(AnimLocalize.contactEnemy, false);
                yield break;
            }

            //if(target.TryGetComponent<CharacterBase>(out CharacterBase targetObj))
            //{
            //    targetObj.TakeDamage(attackDamage);
            //}

            yield return new WaitForSeconds(2f);
        }
    }

    protected virtual void TakeDamage(float inDamage)
    {
        characterStat.ApplyDamage(inDamage);
    }

    protected virtual void SetDead()
    {
        //죽은 오브젝트 자리에 동전 생성
        gameObject.SetActive(false);
    }

    protected virtual void UpdateEnemyList(GameObject target)
    {
        if(!DetectedEnemies.Contains(target))
        {
            DetectedEnemies.Add(target);
        }
    }

    protected virtual void MoveToEnemy()
    {

    }

    private IEnumerator CoMoveToEnemy(GameObject target)
    {
        animator.SetBool(AnimLocalize.contactEnemy, false);
        float distance = Mathf.Abs(Vector3.Distance(target.transform.position, transform.position));
        while (distance > 1.5f)
        {
            yield return new WaitForSeconds(0.1f);
            //animator.SetFloat(AnimLocalize.moveSpeed, characterController.velocity.magnitude);

            movement3D.Move(target.transform.position);
            distance = Vector3.Distance(target.transform.position, transform.position);
        }

        animator.SetFloat(AnimLocalize.moveSpeed, 0);
        Attack(target);
    }

    private void OnUnDetectEnemy(GameObject target)
    {
        //StartCoroutine(CoMoveToEnemy(target));
        if(DetectedEnemies.Contains(target))
        {
            DetectedEnemies.Remove(target);
        }
    }
}
