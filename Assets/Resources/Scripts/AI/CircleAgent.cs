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
    [SerializeField] private string behaviourName;
    [SerializeField] private TrainingManager trainingManager;
    private StatsRecorder statsRecorder;

    private void Awake()
    {
        statsRecorder = Academy.Instance.StatsRecorder;
    }

    public override void OnEpisodeBegin()
    {
        if (trainingManager != null)
            trainingManager.ResetEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (GameManager.Instance.attackCircle == null) return;
        if (!GameManager.Instance.attackCircle.TryGetComponent<PlayerAttackCircle>(out PlayerAttackCircle attackCircle)) return;

        var units = attackCircle.GetOwners;
        for (int i = 0; i < GameManager.MAX_UNITS; i++)
        {
            if (i < units.Count)
            {
                var unit = units[i];
                sensor.AddObservation(1f);
                sensor.AddObservation((int)unit.GetCharacterType());
                sensor.AddObservation((int)unit.GetCharacterLevel());

                if (unit.TryGetComponent<CharacterStat>(out CharacterStat cs))
                    sensor.AddObservation(cs.GetCurrentHp() / cs.GetMaxHp());
                else
                    sensor.AddObservation(0f);
            }
            else
            {
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
            }
        }

        var enemies = attackCircle.GetDetectedEnemies;
        for (int i = 0; i < GameManager.MAX_ENEMIES; i++)
        {
            if (i < enemies.Count)
            {
                var enemy = enemies[i] != null ? enemies[i].GetComponent<CharacterBase>() : null;

                if (enemy == null)
                {
                    sensor.AddObservation(0f);
                    sensor.AddObservation(Vector3.zero);
                    sensor.AddObservation(0f);
                    sensor.AddObservation(0f);
                    sensor.AddObservation(0f);
                    continue;
                }

                sensor.AddObservation(1f);
                sensor.AddObservation(enemy.transform.position - gameObject.transform.position);
                sensor.AddObservation((int)enemy.GetCharacterType());
                sensor.AddObservation((int)enemy.GetCharacterLevel());

                if (enemy.TryGetComponent<CharacterStat>(out CharacterStat cs))
                    sensor.AddObservation(cs.GetCurrentHp() / cs.GetMaxHp());
                else
                    sensor.AddObservation(0f);
            }
            else
            {
                sensor.AddObservation(0f);
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (GameManager.Instance.attackCircle == null) return;
        if (!GameManager.Instance.attackCircle.TryGetComponent<PlayerAttackCircle>(out PlayerAttackCircle attackCircle)) return;

        var units = attackCircle.GetOwners;
        int action = actions.DiscreteActions[0];

        if (trainingManager == null) return;

        statsRecorder.Add("AI_Decision/Action_Choice", action);

        GameObject target = null;
        switch (action)
        {
            case 0:
                target = trainingManager.GetNearestEnemy();
                break;
            case 1:
                target = trainingManager.GetWeakestEnemy();
                break;
        }

        if (target != null)
        {
            units.ForEach(e =>
            {
                if (e.TryGetComponent<CharacterBase>(out CharacterBase character))
                    character.AIActionByJob(target);
            });
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discrete = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.Alpha1)) discrete[0] = 0;
        if (Input.GetKey(KeyCode.Alpha2)) discrete[0] = 1;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Item"))
            AddReward(RewardConstant.GetItemScore);

        if (hit.gameObject.layer == LayerMask.NameToLayer("Coin"))
            AddReward(RewardConstant.GetCoinScore);
    }
}
