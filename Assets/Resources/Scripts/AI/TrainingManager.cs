using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    [SerializeField] Agent agent;

    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private string[] enemyPrefabPaths;

    public Vector3 GetNearestEnemyPosition()
    {
        if (GameManager.Instance.EnemiesInFullRange.Count == 0)
            return gameObject.transform.position;

        GameManager.Instance.EnemiesInFullRange = GameManager.Instance.EnemiesInFullRange
            .OrderBy(e => Vector3.Distance(gameObject.transform.position, e.transform.position))
            .ToList();

        var nearest = GameManager.Instance.EnemiesInFullRange.FirstOrDefault();
        return new Vector3(nearest.transform.position.x, 0f, nearest.transform.position.z);
    }

    public GameObject GetNearestEnemy()
    {
        if (!GameManager.Instance.attackCircle.TryGetComponent<AttackCircle>(out AttackCircle attackCircle))
            return null;

        var enemies = attackCircle.GetDetectedEnemies;
        if (enemies.Count == 0) return null;

        return enemies
            .OrderBy(e => Vector3.Distance(gameObject.transform.position, e.transform.position))
            .FirstOrDefault();
    }

    public GameObject GetWeakestEnemy()
    {
        if (!GameManager.Instance.attackCircle.TryGetComponent<AttackCircle>(out AttackCircle attackCircle))
            return null;

        return attackCircle.GetDetectedEnemies
            .Where(e => e != null && e.TryGetComponent<CharacterStat>(out _))
            .Select(e => e.GetComponent<CharacterStat>())
            .OrderBy(e => e.GetCurrentHp())
            .FirstOrDefault()?.gameObject;
    }

    public GameObject GetNearestFriendly()
    {
        if (!GameManager.Instance.attackCircle.TryGetComponent<AttackCircle>(out AttackCircle attackCircle))
            return null;

        return attackCircle.GetOwners
            .OrderBy(e => Vector3.Distance(gameObject.transform.position, e.gameObject.transform.position))
            .FirstOrDefault()?.gameObject;
    }

    public GameObject GetWeakestFriendly()
    {
        if (!GameManager.Instance.attackCircle.TryGetComponent<AttackCircle>(out AttackCircle attackCircle))
            return null;

        return attackCircle.GetOwners
            .Where(e => e.TryGetComponent<CharacterStat>(out _))
            .Select(e => e.GetComponent<CharacterStat>())
            .OrderBy(e => e.GetCurrentHp())
            .FirstOrDefault()?.gameObject;
    }

    public Item GetNearestItem(bool coin)
    {
        if (!GameManager.Instance.attackCircle.TryGetComponent<AttackCircle>(out AttackCircle attackCircle))
            return null;

        IEnumerable<Item> query;
        if (coin)
        {
            query = attackCircle.GetDetectedItems
                .Where(e => e.GetItemType() == ItemType.Coin);
        }
        else
        {
            query = attackCircle.GetDetectedItems
                .Where(e => e.GetItemType() == ItemType.Gem ||
                            e.GetItemType() == ItemType.Bomb ||
                            e.GetItemType() == ItemType.Cannon);
        }

        return query
            .OrderBy(e => Vector3.Distance(gameObject.transform.position, e.gameObject.transform.position))
            .FirstOrDefault();
    }

    public void OnEnemyKill()
    {
        agent.AddReward(RewardConstant.KillEnemyScore);

        var attackCircle = GameManager.Instance.attackCircle.GetComponent<AttackCircle>();
        bool allEnemiesDead = attackCircle.GetDetectedEnemies.Count == 0
                           && GameManager.Instance.EnemiesInFullRange.Count == 0;

        if (allEnemiesDead)
            agent.EndEpisode();
    }

    public void OnUnitDead()
    {
        agent.AddReward(RewardConstant.DeadUnit);

        var attackCircle = GameManager.Instance.attackCircle.GetComponent<AttackCircle>();
        if (attackCircle.GetOwners.Count == 0)
            agent.EndEpisode();
    }

    public void ResetEpisode()
    {
        var attackCircle = GameManager.Instance.attackCircle.GetComponent<AttackCircle>();

        foreach (var enemy in GameManager.Instance.EnemiesInFullRange.ToList())
        {
            if (enemy != null)
                enemy.SetActive(false);
        }
        GameManager.Instance.EnemiesInFullRange.Clear();
        attackCircle.GetDetectedEnemies.Clear();

        foreach (var owner in attackCircle.GetOwners.ToList())
        {
            if (owner != null)
                owner.Init();
        }

        if (enemySpawnPoints == null || enemyPrefabPaths == null) return;

        for (int i = 0; i < enemySpawnPoints.Length && i < enemyPrefabPaths.Length; i++)
        {
            if (string.IsNullOrEmpty(enemyPrefabPaths[i])) continue;

            GameObject prefab = Resources.Load<GameObject>(enemyPrefabPaths[i]);
            if (prefab == null)
            {
                Debug.LogWarning($"[Training] 적 프리팹 없음: {enemyPrefabPaths[i]}");
                continue;
            }

            GameObject newEnemy = Instantiate(prefab, enemySpawnPoints[i].position, Quaternion.identity);
            GameManager.Instance.EnemiesInFullRange.Add(newEnemy);
        }
    }
}
