using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class responsible for creating and configuring tower instances based on their type, element, and level.
/// This class follows the Singleton pattern for easy access.
/// </summary>
public class TowerFactory : MonoBehaviour
{
    /// <summary>
    /// Configuration data for all available towers.
    /// </summary>
    [Header("Tower Parameter Config")]
    [SerializeField] private TowerConfigCollection towerConfigs;

    /// <summary>
    /// The base prefab for all towers, which will have the core components.
    /// </summary>
    [Header("Fabrication Config")]
    [SerializeField] private PlaceableObject basePrefab;
    /// <summary>
    /// A collection of tower models, used to visually represent different tower types, elements, and levels.
    /// </summary>
    [SerializeField] private TowerModelCollection towerModelCollection;

    /// <summary>
    /// The singleton instance of the TowerFactory.
    /// </summary>
    public static TowerFactory Instance { get; private set; }
    // A cache to store created tower prefabs for faster instantiation.
    private static readonly Dictionary<(TowerType, TowerElement, TowerLevel), PlaceableObject> towerCache = new();

    // A simple inner class to hold references to all the components attached to a tower.
    private class TowerComponentCollection
    {
        public PlaceableObject TowerBase;
        public TowerController TowerController;
        public UpgradeManager UpgradeManager;
        public RangeController RangeController;
        public ElementalTowerController ElementalTowerController;
        public NearbyTowerDetector NearbyTowerDetector;
        public GenericShootingController ShootingController;
    }

    /// <summary>
    /// Sets up the Singleton instance of the TowerFactory and performs initial checks.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of TowerFactory detected.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (basePrefab == null)
        {
            Debug.LogError("Error in TowerFactory! BasePrefab is null!");
            return;
        }

        if (towerModelCollection == null)
        {
            Debug.LogError("Error in TowerFactory! TowerModelCollection is null!");
            return;
        }

        basePrefab.SetActive(false);
    }

    /// <summary>
    /// Gets a tower instance of the specified type, element, and level.
    /// Utilizes a cache to reuse previously created tower prefabs.
    /// </summary>
    /// <param name="type">The type of the tower to create.</param>
    /// <param name="element">The element of the tower to create.</param>
    /// <param name="level">The level of the tower to create.</param>
    /// <returns>An instantiated PlaceableObject representing the configured tower.</returns>
    public PlaceableObject GetTower(TowerType type, TowerElement element, TowerLevel level)
    {
        if (towerCache.TryGetValue((type, element, level), out var tower))
        {
            return Instantiate(tower);
        }

        PlaceableObject newTower = Instantiate(basePrefab);

        TowerComponentCollection componentCollection = new()
        {
            TowerBase = newTower,
            TowerController = newTower.GetComponentInChildren<TowerController>(),
            UpgradeManager = newTower.GetComponentInChildren<UpgradeManager>(),
            RangeController = newTower.GetComponentInChildren<RangeController>(),
            ElementalTowerController = newTower.GetComponentInChildren<ElementalTowerController>(),
            NearbyTowerDetector = newTower.GetComponentInChildren<NearbyTowerDetector>(),
            ShootingController = newTower.GetComponentInChildren<GenericShootingController>(),
        };

        TowerConfig config = towerConfigs.GetConfig(type, element, level);

        HandleType(componentCollection, config);
        HandleConfig(componentCollection, config);
        HandleModel(newTower, config);

        newTower.PlacementLayer = config.PlacementLayer;

        towerCache.Add((type, element, level), newTower);

        return Instantiate(newTower);
    }

    /// <summary>
    /// Gets the configuration data for a specific tower type, element, and level.
    /// </summary>
    /// <param name="type">The type of the tower.</param>
    /// <param name="element">The element of the tower.</param>
    /// <param name="level">The level of the tower.</param>
    /// <returns>The TowerConfig object containing the configuration data.</returns>
    public TowerConfig GetTowerConfig(TowerType type, TowerElement element, TowerLevel level)
    {
        return towerConfigs.GetConfig(type, element, level);
    }

    // Handles the creation and configuration of the shooting controller based on the tower type.
    private void HandleType(TowerComponentCollection componentCollection, TowerConfig config)
    {
        GameObject shootingParent = new("Shooting Controller");
        shootingParent.transform.parent = componentCollection.TowerBase.transform;

        switch (config.Type)
        {
            case TowerType.AreaOfAffect:
                EntityDetectionSystem AOEDetectionSystem = shootingParent.AddComponent<EntityDetectionSystem>();
                AOEDetectionSystem.targetLayer = config.TargetLayer;
                AOEDetectionSystem.fieldOfViewAngle = config.FieldOfViewAngle;
                componentCollection.ShootingController = shootingParent.AddComponent<AOEShootingController>();
                break;
            case TowerType.Aiming:
                shootingParent.AddComponent<AimSystem>();
                EntityDetectionSystem AimingDetectionSystem = shootingParent.AddComponent<EntityDetectionSystem>();
                AimingDetectionSystem.targetLayer = config.TargetLayer;
                AimingDetectionSystem.fieldOfViewAngle = config.FieldOfViewAngle;
                componentCollection.ShootingController = shootingParent.AddComponent<AimedShootingController>();
                break;
            case TowerType.Spray:
                componentCollection.ShootingController = shootingParent.AddComponent<SprayShootingController>();
                break;
        }

        componentCollection.ShootingController.transform.position += new Vector3(0, 3f, 0);
        componentCollection.ShootingController.projectilePrefab = ProjectileFactory.Instance.GetProjectile(config.Type, config.Element, config.Level);
    }

    // Configures the core tower components based on the provided TowerConfig.
    private void HandleConfig(TowerComponentCollection componentCollection, TowerConfig config)
    {
        componentCollection.TowerController.TowerType = config.Type;
        componentCollection.TowerController.TowerElement = config.Element;
        componentCollection.TowerController.TowerLevel = config.Level;
        componentCollection.RangeController.BaseRange = config.Range;
        componentCollection.ShootingController.ProjectileSpeed = config.ProjectileSpeed;
        componentCollection.ShootingController.ShotsPerSecond = config.ShotsPerSecond;
        componentCollection.ShootingController.NumProjectiles = config.NumProjectiles;
        componentCollection.TowerController.TowerCost = config.Cost;

        TowerLevel nextLevel = TowerData.GetNextLevel(config.Level);
        if (nextLevel != config.Level)
        {
            componentCollection.TowerController.NextTowerCost = towerConfigs.GetConfig(config.Type, config.Element, nextLevel).Cost;
        }
        else
        {
            componentCollection.TowerController.NextTowerCost = -1f;
        }
    }

    // Sets the visual model for the tower based on the configuration.
    private void HandleModel(PlaceableObject tower, TowerConfig config)
    {
        GameObject newModel = towerModelCollection.GetTowerModel(config.Type, config.Element, config.Level);
        if (newModel == null)
        {
            Debug.LogError("Error in TowerFactory! New Tower Model is null!");
            return;
        }
        newModel.tag = "Model";
        tower.SetModel(newModel);
    }
}