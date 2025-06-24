using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns enemies at random intervals from a predefined list, inheriting path settings from a parent enemy.
/// </summary>
public class SpawningController : MonoBehaviour
{
    [Tooltip("List of enemy types that can be spawned.")]
    [SerializeField] private List<EnemyType> enemySpawnList = new();

    [Tooltip("Minimum time between spawns.")]
    [SerializeField] private float minSpawnInterval = 0.5f;

    [Tooltip("Maximum time between spawns.")]
    [SerializeField] private float maxSpawnInterval = 2.5f;

    private EnemyController enemyController;
    private float timeUntilNextSpawn;
    private float timeElapsed;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void Start()
    {
        if (enemySpawnList == null || enemySpawnList.Count == 0)
        {
            Debug.LogError("Error in SpawningController! No enemies are provided!");
            return;
        }

        timeUntilNextSpawn = GetRandomSpawnInterval();
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= timeUntilNextSpawn)
        {
            SpawnEnemy();
            timeElapsed = 0f;
            timeUntilNextSpawn = GetRandomSpawnInterval();
        }
    }

    /// <summary>
    /// Spawns a random enemy from the spawn list and assigns its pathing.
    /// </summary>
    private void SpawnEnemy()
    {
        if (enemySpawnList.Count == 0)
        {
            Debug.LogError("Error in SpawningController! Enemy spawn list is empty!");
            return;
        }

        int randomIndex = GetRandomIndex();
        GameObject enemyToSpawn = EnemyFactory.Instance.GetEnemy(enemySpawnList[randomIndex]);

        if (enemyToSpawn != null)
        {
            enemyToSpawn.transform.SetPositionAndRotation(transform.position, transform.rotation);
            PathingController spawnedPathingController = enemyToSpawn.GetComponentInChildren<PathingController>();
            spawnedPathingController.Path = enemyController.Path;
            spawnedPathingController.PathState = enemyController.PathState;
        }

        GameController.Instance.HandleEnemyAdded();
    }

    /// <summary>
    /// Returns a random float between min and max spawn interval.
    /// </summary>
    private float GetRandomSpawnInterval()
    {
        System.Random random = new();
        return (float)(random.NextDouble() * (maxSpawnInterval - minSpawnInterval) + minSpawnInterval);
    }

    /// <summary>
    /// Returns a random index from the spawn list.
    /// </summary>
    private int GetRandomIndex()
    {
        System.Random random = new();
        return random.Next(enemySpawnList.Count);
    }
}
