using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [Tooltip("The path the enemies will follow.")]
    [SerializeField] private SplinePath path;
    [Tooltip("The list of the different enemy types.")]
    [SerializeField] private List<GameObject> enemyTypes = new();

    private readonly Dictionary<EnemyType, GameObject> enemyCache = new();

    public static EnemyFactory Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of EnemyFactory detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Retrieves an enemy object based on the specified enemy type.
    /// </summary>
    /// <param name="type">The type of enemy to spawn.</param>
    /// <returns>Instantiated enemy game object or null if no valid enemy type is found.</returns>
    public GameObject GetEnemy(EnemyType type)
    {
        // Return cached enemy if available
        if (enemyCache.TryGetValue(type, out var cachedEnemy))
        {
            GameObject instantiatedEnemy = Instantiate(cachedEnemy);
            SetEnemyPath(instantiatedEnemy);
            return instantiatedEnemy;
        }

        foreach (var enemyPrefab in enemyTypes)
        {
            if (enemyPrefab.TryGetComponent<EnemyController>(out var enemyController))
            {
                if (enemyController.Type == type)
                {
                    enemyCache[type] = enemyPrefab;

                    GameObject instantiatedEnemy = Instantiate(enemyPrefab);
                    SetEnemyPath(instantiatedEnemy);
                    return instantiatedEnemy;
                }
            }
        }

        Debug.LogError("No enemy found for type: " + type);
        return null;
    }

    /// <summary>
    /// Assigns the given path to the enemy's PathingController.
    /// </summary>
    /// <param name="enemy">The enemy game object to set the path for.</param>
    private void SetEnemyPath(GameObject enemy)
    {
        PathingController pathingController = enemy.GetComponentInChildren<PathingController>();
        if (pathingController != null)
        {
            pathingController.Path = path;
        }
        else
        {
            Debug.LogWarning("Enemy does not have a PathingController.");
        }
    }
}
