using UnityEngine;

/// <summary>
/// Contains the different model variations for a tower based on its level.
/// </summary>
[System.Serializable]
public class TowerLevelCollection
{
    /// <summary>
    /// The base model for the first level of the tower.
    /// </summary>
    public GameObject baseModel;
    /// <summary>
    /// The model for the second level (first upgrade) of the tower.
    /// </summary>
    public GameObject FirstUpgradeModel;
    /// <summary>
    /// The model for the third level (second upgrade) of the tower.
    /// </summary>
    public GameObject SecondUpgradeModel;

    /// <summary>
    /// Gets the appropriate tower model based on the specified level.
    /// </summary>
    /// <param name="level">The level of the tower.</param>
    /// <returns>The GameObject representing the tower model for the given level.</returns>
    public GameObject GetTowerModel(TowerLevel level)
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
/// Contains the different level collections for a specific tower element.
/// </summary>
[System.Serializable]
public class TowerTypeCollection
{
    /// <summary>
    /// Model variations for Water element towers.
    /// </summary>
    public TowerLevelCollection WaterTowerModels = new();
    /// <summary>
    /// Model variations for Fire element towers.
    /// </summary>
    public TowerLevelCollection FireTowerModels = new();
    /// <summary>
    /// Model variations for Air element towers.
    /// </summary>
    public TowerLevelCollection AirTowerModels = new();
    /// <summary>
    /// Model variations for Earth element towers.
    /// </summary>
    public TowerLevelCollection EarthTowerModels = new();
    /// <summary>
    /// Model variations for Electric element towers.
    /// </summary>
    public TowerLevelCollection LightningTowerModels = new();
    /// <summary>
    /// Model variations for Ice element towers.
    /// </summary>
    public TowerLevelCollection IceTowerModels = new();

    /// <summary>
    /// Gets the appropriate tower model based on the specified element and level.
    /// </summary>
    /// <param name="element">The element of the tower.</param>
    /// <param name="level">The level of the tower.</param>
    /// <returns>The GameObject representing the tower model for the given element and level.</returns>
    public GameObject GetTowerModel(TowerElement element, TowerLevel level)
    {
        return element switch
        {
            TowerElement.Water => WaterTowerModels.GetTowerModel(level),
            TowerElement.Fire => FireTowerModels.GetTowerModel(level),
            TowerElement.Air => AirTowerModels.GetTowerModel(level),
            TowerElement.Earth => EarthTowerModels.GetTowerModel(level),
            TowerElement.Electric => LightningTowerModels.GetTowerModel(level),
            TowerElement.Ice => IceTowerModels.GetTowerModel(level),
            _ => null,
        };
    }

}

/// <summary>
/// Contains the different type collections for organizing tower models.
/// </summary>
[System.Serializable]
public class TowerModelCollection
{
    /// <summary>
    /// Model variations for Aiming type towers.
    /// </summary>
    public TowerTypeCollection AimingTowers = new();
    /// <summary>
    /// Model variations for Spray type towers.
    /// </summary>
    public TowerTypeCollection SprayTowers = new();
    /// <summary>
    /// Model variations for Area of Effect type towers.
    /// </summary>
    public TowerTypeCollection AOETowers = new();

    /// <summary>
    /// Gets the appropriate tower model based on the specified type, element, and level.
    /// </summary>
    /// <param name="type">The type of the tower.</param>
    /// <param name="element">The element of the tower.</param>
    /// <param name="level">The level of the tower.</param>
    /// <returns>The GameObject representing the tower model for the given type, element, and level.</returns>
    public GameObject GetTowerModel(TowerType type, TowerElement element, TowerLevel level)
    {
        return type switch
        {
            TowerType.Aiming => AimingTowers.GetTowerModel(element, level),
            TowerType.AreaOfAffect => AOETowers.GetTowerModel(element, level),
            TowerType.Spray => SprayTowers.GetTowerModel(element, level),
            _ => null,
        };
    }
}