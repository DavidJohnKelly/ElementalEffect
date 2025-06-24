using UnityEngine;

/// <summary>
/// A shooting controller that fires multiple projectiles in a spray pattern around the tower.
/// </summary>
public class SprayShootingController : GenericShootingController
{
    /// <summary>
    /// Shoots multiple projectiles in a circular spray pattern around the tower.
    /// The number of projectiles is determined by the NumProjectiles property.
    /// </summary>
    /// <returns>True if projectiles were fired, false otherwise (e.g., if projectile prefab is not assigned).</returns>
    protected override bool Shoot()
    {
        if (!projectilePrefab)
        {
            Debug.LogError("Projectile Prefab is not assigned!");
            return false;
        }

        for (int j = 0; j < NumProjectiles; j++)
        {
            float angle = (360f / NumProjectiles) * j;
            Vector3 aimVector = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            FireProjectile(aimVector);
        }

        return true;
    }
}