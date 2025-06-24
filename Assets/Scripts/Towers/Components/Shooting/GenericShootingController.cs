using UnityEngine;

/// <summary>
/// A generic base class for controlling the shooting behavior of towers.
/// Handles basic shooting rate and projectile instantiation.
/// </summary>
public class GenericShootingController : MonoBehaviour
{
    /// <summary>
    /// The prefab of the projectile to be fired.
    /// </summary>
    [Tooltip("The prefab of the projectile to be fired.")]
    public GameObject projectilePrefab;
    /// <summary>
    /// The number of projectiles fired per shot (implementation may vary in derived classes).
    /// </summary>
    [Tooltip("The number of projectiles fired per shot.")]
    public float NumProjectiles = 1;
    /// <summary>
    /// The number of shots fired per second.
    /// </summary>
    [Tooltip("The number of shots fired per second.")]
    public float ShotsPerSecond;
    /// <summary>
    /// The speed at which the projectiles travel.
    /// </summary>
    [Tooltip("The speed at which the projectiles travel.")]
    public float ProjectileSpeed;

    // Reference to the parent TowerController for accessing tower properties.
    protected TowerController towerController;

    // Keeps track of the time elapsed since the last shot was fired.
    private float secondsSinceLastShot = 0f;

    /// <summary>
    /// Retrieves the TowerController component from the parent.
    /// </summary>
    protected virtual void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
    }

    /// <summary>
    /// Updates the shooting timer and attempts to shoot if the fire rate allows and the tower can shoot.
    /// </summary>
    protected void Update()
    {
        secondsSinceLastShot += Time.deltaTime;

        if (ShotsPerSecond == 0 || !towerController.CanShoot) return;


        if (secondsSinceLastShot >= 1f / ShotsPerSecond)
        {
            bool shot = Shoot();
            if (shot)
            {
                secondsSinceLastShot = 0f;
            }
        }
    }

    /// <summary>
    /// Virtual method to be overridden by derived classes to implement specific shooting logic.
    /// </summary>
    /// <returns>True if a shot was attempted, false otherwise.</returns>
    protected virtual bool Shoot()
    {
        Debug.Log("Shooting not implemented");
        return false;
    }

    /// <summary>
    /// Instantiates a projectile prefab and sets its initial properties based on the provided aim vector.
    /// </summary>
    /// <param name="aimVector">The direction in which the projectile should travel.</param>
    /// <returns>True if the projectile was successfully fired, false otherwise.</returns>
    protected virtual bool FireProjectile(Vector3 aimVector)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab is not assigned!");
            return false;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.LookRotation(aimVector)
        );

        // Set the projectile's direction and speed
        if (projectile.TryGetComponent<ProjectileController>(out var projectileController))
        {
            projectileController.Direction = aimVector;
            projectileController.Speed = ProjectileSpeed;
            projectileController.MaxDistance = towerController.Range;
            projectileController.ElementalDamageModfier = towerController.GetElementalDamageModifier();
            projectile.SetActive(true);

            return true;
        }
        return false;
    }
}