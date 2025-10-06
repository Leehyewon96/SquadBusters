using System.Linq;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    [SerializeField] SquadAgent squadAgent;
    [SerializeField] float reward = 0.1f;

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
        return GameManager.Instance.EnemiesInFullRange.FirstOrDefault();
    }

    public void OnEnemyKill()
    {
        squadAgent.AddReward(reward);
    }

    public void OnSquadWipedOut()
    {
        squadAgent.AddReward(-reward);
        squadAgent.EndEpisode();
    }

    public void OnKillMonster()
    {
        squadAgent.AddReward(reward);
        squadAgent.EndEpisode();
    }
}
