using UnityEngine;

/// <summary>
/// A shooting controller that aims at targets using an AimSystem component.
/// </summary>
public class AimedShootingController : GenericShootingController
{
    // The AimSystem component responsible for calculating the aiming vector.
    private AimSystem aimSystem;

    /// <summary>
    /// Initializes the AimSystem component.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        aimSystem = GetComponent<AimSystem>();
    }

    /// <summary>
    /// Attempts to shoot a projectile by getting the aim vector from the AimSystem and firing.
    /// </summary>
    /// <returns>True if a projectile was successfully fired, false otherwise.</returns>
    protected override bool Shoot()
    {
        if (!aimSystem.GetAimVector(ProjectileSpeed, out Vector3 aimVector))
        {
            return false;
        }

        return FireProjectile(aimVector);
    }
}