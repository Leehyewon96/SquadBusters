using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected Animator animator = null;
    protected CharacterController characterController = null;
    protected Movement3D movement3D = null;
    protected CharacterStat characterStat = null;
    protected HpBar hpBar = null;

    protected float attackRange = 1f;
    protected float attackRadius = 1f;
    protected float attackDamage = 30f;


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
    }


    protected virtual void Attack()
    {
        animator.SetBool(AnimLocalize.contactEnemy, true);

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, attackRadius, transform.forward, out hit, attackRange))
        {
            Debug.Log($"hit object : {hit.collider.name}");
            hit.collider.gameObject.GetComponent<CharacterBase>().TakeDamage(attackDamage);
            
        }

        Debug.DrawRay(transform.position, transform.forward * attackRange, Color.red, 5f);
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
}
