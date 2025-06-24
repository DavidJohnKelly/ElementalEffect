using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class responsible for creating and managing projectile instances.
/// </summary>
public class ProjectileFactory : MonoBehaviour
{
    /// <summary>
    /// The base prefab to instantiate for new projectiles.
    /// </summary>
    [Tooltip("The base prefab to instantiate for new projectiles.")]
    [SerializeField] private GameObject projectilePrefab;
    /// <summary>
    /// A collection containing different projectile models based on element and level.
    /// </summary>
    [Tooltip("A collection containing different projectile models based on element and level.")]
    [SerializeField] private ProjectileModelCollection projectileModelCollection;

    /// <summary>
    /// Singleton instance of the ProjectileFactory.
    /// </summary>
    public static ProjectileFactory Instance { get; private set; }
    // A cache to store instantiated projectile prefabs for different tower configurations.
    private static readonly Dictionary<(TowerType, TowerElement, TowerLevel), GameObject> projectileCache = new();

    /// <summary>
    /// Initializes the singleton instance and performs setup for the factory.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of ProjectileFactory detected.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (projectilePrefab == null)
        {
            Debug.LogError("Error in TowerFactory! BasePrefab is null!");
            return;
        }

        if (projectileModelCollection == null)
        {
            Debug.LogError("Error in TowerFactory! TowerModelCollection is null!");
            return;
        }

        projectilePrefab.SetActive(false);
    }

    /// <summary>
    /// Retrieves a projectile GameObject based on the specified tower type, element, and level.
    /// Utilizes a cache for performance.
    /// </summary>
    /// <param name="type">The type of the tower firing the projectile.</param>
    /// <param name="element">The elemental type of the projectile.</param>
    /// <param name="level">The level of the tower firing the projectile.</param>
    /// <returns>An instantiated GameObject representing the projectile.</returns>
    public GameObject GetProjectile(TowerType type, TowerElement element, TowerLevel level)
    {
        if (projectileCache.TryGetValue((type, element, level), out var projectile))
        {
            return Instantiate(projectile);
        }

        GameObject newProjectile = Instantiate(projectilePrefab);
        ProjectileController projectileController = newProjectile.GetComponent<ProjectileController>();
        projectileController.Element = element;

        HandleType(projectileController, type);
        HandleLevel(projectileController, level);
        HandleModel(newProjectile, element, level);

        projectileCache.Add((type, element, level), newProjectile);

        return Instantiate(newProjectile);
    }

    // Sets the physical and elemental damage of the projectile based on the tower type.
    private void HandleType(ProjectileController projectile, TowerType type)
    {
        switch (type)
        {
            case TowerType.Aiming:
                projectile.PhysicalDamagePercent = 0.8f;
                projectile.ElementalDamage = 6f;
                return;
            case TowerType.AreaOfAffect:
                projectile.PhysicalDamagePercent = 0f;
                projectile.ElementalDamage = 0.5f;
                return;
            case TowerType.Spray:
                projectile.PhysicalDamagePercent = 0.5f;
                projectile.ElementalDamage = 3f;
                return;
        }
    }

    // Modifies the elemental damage of the projectile based on the tower level.
    private void HandleLevel(ProjectileController projectile, TowerLevel level)
    {
        switch (level)
        {
            case TowerLevel.One:
                projectile.ElementalDamage *= 1f;
                return;
            case TowerLevel.Two:
                projectile.ElementalDamage *= 1.2f;
                return;
            case TowerLevel.Three:
                projectile.ElementalDamage *= 1.5f;
                return;
        }
    }

    // Updates the visual model of the projectile based on its element and the tower level.
    private void HandleModel(GameObject projectile, TowerElement element, TowerLevel level)
    {
        GameObject newModel = projectileModelCollection.GetProjectileModel(element, level);
        if (newModel == null)
        {
            Debug.LogError("Error in ProjectileFactory! New Projectile Model is null!");
            return;
        }
        UpdateModel(projectile, newModel);
    }

    // Instantiates and sets up the visual model for the projectile.
    private void UpdateModel(GameObject projectile, GameObject model)
    {
        if (model == null) return;

        GameObject newModel = Instantiate(model, projectile.transform);
        newModel.tag = "Model";

        CapsuleCollider projectileCollider = projectile.GetComponentInChildren<CapsuleCollider>();
        if (ModelUtil.GetModelBounds(newModel, out var bounds))
        {
            ModelUtil.FitCollider(projectileCollider, bounds);
        }
        newModel.SetActive(true);
    }
}