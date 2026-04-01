using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SquadAgent : Agent
{
    [SerializeField] private string behaviourName;
    private CharacterBase character;
    private CharacterStat characterStat;
    private TrainingManager trainingManager;

    private void Awake()
    {
        character = GetComponent<CharacterBase>();
        characterStat = GetComponent<CharacterStat>();
        trainingManager = GetComponent<TrainingManager>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(characterStat.GetCurrentHp() / characterStat.GetMaxHp());
        sensor.AddObservation(character.transform.position);

        if (GameManager.Instance.attackCircle == null) return;
        if (!GameManager.Instance.attackCircle.TryGetComponent<PlayerAttackCircle>(out PlayerAttackCircle attackCircle)) return;

        var units = attackCircle.GetOwners;
        for (int i = 0; i < GameManager.MAX_UNITS; i++)
        {
            if (i < units.Count)
            {
                var unit = units[i];
                if (units[i].gameObject == gameObject) continue;

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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Wall"))
            AddReward(RewardConstant.CrashScore);

        if (hit.gameObject.layer == LayerMask.NameToLayer("Coin"))
            AddReward(RewardConstant.GetCoinScore);

        if (hit.gameObject.layer == LayerMask.NameToLayer("Item"))
            AddReward(RewardConstant.GetItemScore);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];
        GameObject target;

        switch (action)
        {
            case 0:
                target = trainingManager.GetNearestEnemy();
                character.AIActionByJob(target);
                break;
            case 1:
                target = trainingManager.GetWeakestEnemy();
                character.AIActionByJob(target);
                break;
            case 2:
                target = trainingManager.GetNearestFriendly();
                if (target != null)
                    character.MoveToTarget(target);
                break;
            case 3:
                target = trainingManager.GetWeakestFriendly();
                if (target != null)
                    character.MoveToTarget(target);
                break;
        }
    }
}
