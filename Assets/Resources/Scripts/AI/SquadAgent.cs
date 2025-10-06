using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Search;
using UnityEngine;

public class SquadAgent : Agent
{
    [SerializeField] string behaviourName;
    [SerializeField] CharacterBase character;
    [SerializeField] CharacterStat characterStat;
    [SerializeField] TrainingManager trainingManager;

    private float maxDistance = 10f;

    public override void CollectObservations(VectorSensor sensor)
    {
        float[] rayDistances = new float[3];
        Transform agentTransform = this.transform;

        // 정면
        if (Physics.Raycast(agentTransform.position, agentTransform.forward, out RaycastHit hit, maxDistance))
        {
            rayDistances[0] = hit.distance / maxDistance; // 0~1 사이 값으로 정규화
        }
        else
        {
            rayDistances[0] = 1.0f; // 아무것도 안 맞았으면 최대 거리
        }
        // 오른쪽
        if (Physics.Raycast(agentTransform.position, agentTransform.right, out RaycastHit hitR, maxDistance))
        {
            rayDistances[1] = hitR.distance / maxDistance; // 0~1 사이 값으로 정규화
        }
        else
        {
            rayDistances[1] = 1.0f; // 아무것도 안 맞았으면 최대 거리
        }
        // 왼쪽
        if (Physics.Raycast(agentTransform.position, -agentTransform.right, out RaycastHit hitL, maxDistance))
        {
            rayDistances[1] = hitL.distance / maxDistance; // 0~1 사이 값으로 정규화
        }
        else
        {
            rayDistances[1] = 1.0f; // 아무것도 안 맞았으면 최대 거리
        }

        // 측정된 거리 값들을 관측 정보에 추가
        sensor.AddObservation(rayDistances[0]); // 정면
        sensor.AddObservation(rayDistances[1]); // 오른쪽
        sensor.AddObservation(rayDistances[2]); // 왼쪽

        sensor.AddObservation(character.transform.position); // 캐릭터 위치
        sensor.AddObservation(characterStat.GetCoin()); // 캐릭터가 보유한 코인
        sensor.AddObservation(trainingManager.GetNearestEnemyPosition()); // 가장 가까운 적 위치
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 부딪힌 오브젝트의 태그가 "Wall"이라면 (벽의 태그를 Wall로 설정해야 함)
        if (hit.gameObject.CompareTag("Wall"))
        {
            // 작은 음수 보상을 주어 벌점을 부여합니다.
            AddReward(-0.01f);
        }

        if (hit.gameObject.CompareTag("Coin"))
        {
            // 작은 음수 보상을 주어 벌점을 부여합니다.
            AddReward(0.01f);
        }
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveAction = actions.DiscreteActions[0];
        character.AIAction(moveAction);
    }

    public void SetNearestTarget()
    {
        character.SetDestinationPos(trainingManager.GetNearestEnemyPosition());
    }
}
