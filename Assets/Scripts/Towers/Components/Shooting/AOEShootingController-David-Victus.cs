using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A shooting controller that applies area-of-effect damage to all detected enemies within range.
/// </summary>
public class AOEShootingController : GenericShootingController
{
    // The EntityDetectionSystem component used to find targets within range.
    private EntityDetectionSystem entityDetectionSystem;
    // A reference to the ProjectileController component of the projectile prefab for damage information.
    private ProjectileController projectileController;

    /// <summary>
    /// Initializes the EntityDetectionSystem component.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        entityDetectionSystem = GetComponent<EntityDetectionSystem>();
    }

    /// <summary>
    /// Retrieves the ProjectileController from the projectile prefab.
    /// </summary>
    private void Start()
    {
        projectileController = projectilePrefab.GetComponent<ProjectileController>();
    }

    /// <summary>
    /// Applies area-of-effect damage to all detected enemies within range and spawns a particle effect.
    /// </summary>
    /// <returns>True if damage was applied to at least one enemy, false otherwise.</returns>
    protected override bool Shoot()
    {
        List<GameObject> detectedObjects = entityDetectionSystem.GetAllDetectedTargets();

        if (detectedObjects.Count == 0) return false;

        foreach (GameObject detectedObject in detectedObjects)
        {
            if (detectedObject.TryGetComponent<EnemyController>(out var enemyController))
            {
                enemyController.HandleElementalDamage(projectileController.Element, projectileController.ElementalDamage * projectileController.ElementalDamageModfier);
                DamageTextFactory.Instance.CreateDamageText(
                    detectedObject.transform.position,
                    projectileController.Element,
                    projectileController.ElementalDamage * projectileController.ElementalDamageModfier,
                    projectileController.PhysicalDamagePercent
                );
            }
        }

        ParticleFactory.Instance.SpawnParticleSystem(projectileController.Element, transform, towerController.Range);
        return true;
    }
}