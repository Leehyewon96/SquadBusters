using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public static class RewardConstant
{
    public const float CrashScore = -0.01f;
    public const float KillEnemyScore = 0.1f;
    public const float DeadUnit = -0.1f;
    public const float GetCoinScore = 0.01f;
    public const float GetItemScore = 0.01f;
}

public class CircleAgent : Agent
{
    [SerializeField] string behaviourName;
    [SerializeField] TrainingManager trainingManager;

    private float maxDistance = 10f;

    public override void CollectObservations(VectorSensor sensor)
    {
        if (GameManager.Instance.attackCircle == null) return;
        if (!GameManager.Instance.attackCircle.TryGetComponent<PlayerAttackCircle>(out PlayerAttackCircle attackCircle)) return;

        // 유닛 정보
        for (int i = 0; i < GameManager.MAX_UNITS; i++)
        {
            var units = attackCircle.GetOwners;

            if (i < units.Count)
            {
                var unit = units[i];
                sensor.AddObservation(1f); // 슬롯 활성화 여부
                sensor.AddObservation((int)unit.GetCharacterType()); // 유닛 종류
                sensor.AddObservation((int)unit.GetCharacterLevel()); // 유닛 레벨

                if (unit.TryGetComponent<CharacterStat>(out CharacterStat characterStat))
                {
                    sensor.AddObservation(characterStat.GetCurrentHp() / characterStat.GetMaxHp()); //현재 체력 (정규화)
                }
            }
            else
            {
                sensor.AddObservation(0f); // 슬롯 활성화 여부
                sensor.AddObservation(0f); // 유닛 종류
                sensor.AddObservation(0f); // 유닛 레벨
                sensor.AddObservation(0f); //현재 체력 (정규화)
            }
        }

        //적 정보
        for(int i = 0; i < GameManager.MAX_ENEMIES; i++)
        {
            var enemies = attackCircle.GetDetectedEnemies;

            if (i < enemies.Count)
            {
                var enemy = enemies[i] != null ? enemies[i].GetComponent<CharacterBase>() : null;
                if (enemy == null) return;

                sensor.AddObservation(1f); // 슬롯 활성화 여부
                sensor.AddObservation(enemy.transform.position - gameObject.transform.position); // 적 캐릭터 상대위치
                sensor.AddObservation((int)enemy.GetCharacterType()); // 적 캐릭터 종류
                sensor.AddObservation((int)enemy.GetCharacterLevel()); // 적 캐릭터 레벨

                if (enemy.TryGetComponent<CharacterStat>(out CharacterStat characterStat))
                {
                    sensor.AddObservation(characterStat.GetCurrentHp() / characterStat.GetMaxHp()); //현재 체력 (정규화)
                }
            }
            else
            {
                sensor.AddObservation(0f); // 슬롯 활성화 여부
                sensor.AddObservation(0f); // 적 캐릭터 종류
                sensor.AddObservation(0f); // 적 캐릭터 레벨
                sensor.AddObservation(0f); //현재 체력 (정규화)
            }
        }
    }

    #region OnControllerColliderHit
    /*private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.01f);
        }

        if (hit.gameObject.CompareTag("Coin"))
        {
            AddReward(0.01f);
        }
    }*/
    #endregion

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (GameManager.Instance.attackCircle == null) return;
        if (!GameManager.Instance.attackCircle.TryGetComponent<PlayerAttackCircle>(out PlayerAttackCircle attackCircle)) return;
        var units = attackCircle.GetOwners;

        int action = actions.DiscreteActions[0];
        if (trainingManager == null) return;
        GameObject target = null;
        switch(action)
        {
            case 0:
                target = trainingManager.GetNearestEnemy();
                break;
            case 1:
                target = trainingManager.GetWeakestEnemy();
                break;
            default:
                break;
        }

        units.ForEach(e =>
        {
            if (e.TryGetComponent<CharacterBase>(out CharacterBase character))
                character.AIActionByJob(target);
        });
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            AddReward(RewardConstant.GetItemScore);
        }

        if(hit.gameObject.layer == LayerMask.NameToLayer("Coin"))
        {
            AddReward(RewardConstant.GetCoinScore);
        }
    }
}
