using System.Linq;
using Unity.MLAgents;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    [SerializeField] Agent agent;

    public Vector3 GetNearestEnemyPosition()
    {
        var nearestPos = Vector3.zero;
        if (GameManager.Instance.EnemiesInFullRange.Count == 0)
        {
            return gameObject.transform.position;
        }

        GameManager.Instance.EnemiesInFullRange = GameManager.Instance.EnemiesInFullRange.OrderBy(e => 
            Vector3.Distance(gameObject.transform.position, e.transform.position)).ToList();
        nearestPos.x = GameManager.Instance.EnemiesInFullRange.FirstOrDefault().transform.position.x;
        nearestPos.z = GameManager.Instance.EnemiesInFullRange.FirstOrDefault().transform.position.z;
        return nearestPos;
    }

    public GameObject GetNearestEnemy()
    {
        //return GameManager.Instance.EnemiesInFullRange.FirstOrDefault();
        if(GameManager.Instance.attackCircle.TryGetComponent<AttackCircle>(out AttackCircle attackCircle))
        {
            attackCircle.GetDetectedEnemies = attackCircle.GetDetectedEnemies.OrderBy(e =>
            Vector3.Distance(gameObject.transform.position, e.transform.position)).ToList();
            return attackCircle.GetDetectedEnemies.FirstOrDefault();
        }
        return null;
    }

    public void OnEnemyKill()
    {
        agent.AddReward(RewardConstant.KillScore);
        agent.EndEpisode();
    }

    public void OnUnitDead()
    {
        agent.AddReward(-RewardConstant.KillScore);
        agent.EndEpisode();
    }
}
