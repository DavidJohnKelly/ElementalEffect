using UnityEngine;

/// <summary>
/// Stores the different projectile models for each tower level.
/// </summary>
[System.Serializable]
public class ProjectileLevelCollection
{
    /// <summary>
    /// The base projectile model for level one.
    /// </summary>
    [Tooltip("The base projectile model for level one.")]
    public GameObject baseModel;
    /// <summary>
    /// The upgraded projectile model for level two.
    /// </summary>
    [Tooltip("The upgraded projectile model for level two.")]
    public GameObject FirstUpgradeModel;
    /// <summary>
    /// The fully upgraded projectile model for level three.
    /// </summary>
    [Tooltip("The fully upgraded projectile model for level three.")]
    public GameObject SecondUpgradeModel;

    /// <summary>
    /// Gets the appropriate projectile model based on the provided tower level.
    /// </summary>
    /// <param name="level">The current level of the tower.</param>
    /// <returns>The GameObject representing the projectile model for the given level, or null if no model is found.</returns>
    public GameObject GetProjectileModel(TowerLevel level)
    {
        return level switch
        {
            TowerLevel.One => baseModel,
            TowerLevel.Two => FirstUpgradeModel,
            TowerLevel.Three => SecondUpgradeModel,
            _ => null,
        };
    }
}

/// <summary>
/// Stores a collection of projectile models, organized by tower element and level.
/// </summary>
[System.Serializable]
public class ProjectileModelCollection
{
    /// <summary>
    /// Projectile models for the Water element.
    /// </summary>
    [Tooltip("Projectile models for the Water element.")]
    public ProjectileLevelCollection WaterProjectileModels = new();
    /// <summary>
    /// Projectile models for the Fire element.
    /// </summary>
    [Tooltip("Projectile models for the Fire element.")]
    public ProjectileLevelCollection FireProjectileModels = new();
    /// <summary>
    /// Projectile models for the Air element.
    /// </summary>
    [Tooltip("Projectile models for the Air element.")]
    public ProjectileLevelCollection AirProjectileModels = new();
    /// <summary>
    /// Projectile models for the Earth element.
    /// </summary>
    [Tooltip("Projectile models for the Earth element.")]
    public ProjectileLevelCollection EarthProjectileModels = new();
    /// <summary>
    /// Projectile models for the Lightning element.
    /// </summary>
    [Tooltip("Projectile models for the Lightning element.")]
    public ProjectileLevelCollection LightningProjectileModels = new();
    /// <summary>
    /// Projectile models for the Ice element.
    /// </summary>
    [Tooltip("Projectile models for the Ice element.")]
    public ProjectileLevelCollection IceProjectileModels = new();

    /// <summary>
    /// Gets the appropriate projectile model based on the provided tower element and level.
    /// </summary>
    /// <param name="element">The elemental type of the projectile.</param>
    /// <param name="level">The current level of the tower.</param>
    /// <returns>The GameObject representing the projectile model for the given element and level, or null if no model is found.</returns>
    public GameObject GetProjectileModel(TowerElement element, TowerLevel level)
    {
        return element switch
        {
            TowerElement.Water => WaterProjectileModels.GetProjectileModel(level),
            TowerElement.Fire => FireProjectileModels.GetProjectileModel(level),
            TowerElement.Air => AirProjectileModels.GetProjectileModel(level),
            TowerElement.Earth => EarthProjectileModels.GetProjectileModel(level),
            TowerElement.Electric => LightningProjectileModels.GetProjectileModel(level),
            TowerElement.Ice => IceProjectileModels.GetProjectileModel(level),
            _ => null,
        };
    }
}