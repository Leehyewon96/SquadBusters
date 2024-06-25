using UnityEngine.AI;

public class CharacterUnit : CharacterBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        hpBar = GameManager.Instance.hpBarManager.GetHpBar(HpBar.barType.Player); //GetComponentInChildren<HpBar>();
        attackCircle = GameManager.Instance.attackCircleManager.GetAttackCircle(AttackCircle.circleType.Player);
        characterController.enabled = false;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        animator.SetFloat(AnimLocalize.moveSpeed, navMeshAgent.velocity.magnitude);
        navMeshAgent.SetDestination(attackCircle.transform.position);
    }
}
