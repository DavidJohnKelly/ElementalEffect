using UnityEngine;

/// <summary>
/// Manages enemy health and handles death logic when health reaches zero.
/// </summary>
public class HealthController : MonoBehaviour
{
    [Tooltip("Initial health value of the enemy")]
    [SerializeField] private float baseHealth = 10f;

    private EnemyController enemyController;
    private bool hasDied = false;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    /// <summary>
    /// Reduces health by the given amount. Triggers death if health falls to zero.
    /// </summary>
    /// <param name="damage">Amount of damage taken</param>
    /// <param name="isFromPlayer">Whether the damage source is from the player</param>
    public void TakeDamage(float damage, bool isFromPlayer = false)
    {
        if (hasDied) return;

        baseHealth -= damage;
        if (baseHealth <= 0)
        {
            hasDied = true;
            enemyController.HandleEnemyDied(isFromPlayer);
        }
    }
}
