using UnityEngine;

/// <summary>
/// Represents the different types of towers available in the game.
/// </summary>
public enum TowerType
{
    /// <summary>
    /// Towers that target and shoot at individual enemies.
    /// </summary>
    Aiming,
    /// <summary>
    /// Towers that deal damage to enemies within a certain area.
    /// </summary>
    AreaOfAffect,
    /// <summary>
    /// Towers that fire multiple projectiles in a spread pattern.
    /// </summary>
    Spray
}

/// <summary>
/// Represents the different elemental types of towers and their projectiles.
/// </summary>
public enum TowerElement
{
    /// <summary>
    /// Represents the Air element.
    /// </summary>
    Air,
    /// <summary>
    /// Represents the Earth element.
    /// </summary>
    Earth,
    /// <summary>
    /// Represents the Electric element.
    /// </summary>
    Electric,
    /// <summary>
    /// Represents the Fire element.
    /// </summary>
    Fire,
    /// <summary>
    /// Represents the Ice element.
    /// </summary>
    Ice,
    /// <summary>
    /// Represents the Water element.
    /// </summary>
    Water
}

/// <summary>
/// Represents the different upgrade levels of a tower.
/// </summary>
public enum TowerLevel
{
    /// <summary>
    /// The first level of the tower.
    /// </summary>
    One,
    /// <summary>
    /// The second level of the tower.
    /// </summary>
    Two,
    /// <summary>
    /// The third level of the tower.
    /// </summary>
    Three
}

/// <summary>
/// Provides static utility methods for working with tower data and enums.
/// </summary>
public static class TowerData
{
    /// <summary>
    /// Gets the next upgrade level for a given tower level.
    /// </summary>
    /// <param name="currentLevel">The current level of the tower.</param>
    /// <returns>The next tower level, or the current level if it's already the highest.</returns>
    public static TowerLevel GetNextLevel(TowerLevel currentLevel)
    {
        return currentLevel switch
        {
            TowerLevel.One => TowerLevel.Two,
            TowerLevel.Two => TowerLevel.Three,
            _ => currentLevel,
        };
    }

    /// <summary>
    /// Gets the TowerType enum value based on an integer representation.
    /// </summary>
    /// <param name="type">The integer representation of the tower type (1: Aiming, 2: AreaOfAffect, 3: Spray).</param>
    /// <returns>The corresponding TowerType enum value.</returns>
    public static TowerType GetTowerType(int type)
    {
        switch (type)
        {
            case 1:
                return TowerType.Aiming;
            case 2:
                return TowerType.AreaOfAffect;
            case 3:
                return TowerType.Spray;
        }
        Debug.LogError("Error in TowerData! Tower Type: " + type + " invalid!");
        return TowerType.Aiming;
    }

    /// <summary>
    /// Gets the string representation of a TowerType enum value.
    /// </summary>
    /// <param name="type">The TowerType enum value.</param>
    /// <returns>The string representation of the tower type.</returns>
    public static string GetTypeString(TowerType type)
    {
        return type switch
        {
            TowerType.Aiming => "Aiming",
            TowerType.AreaOfAffect => "Area Of Affect",
            TowerType.Spray => "Spray",
            _ => "NULL",
        };
    }

    /// <summary>
    /// Gets the TowerElement enum value based on a string representation.
    /// </summary>
    /// <param name="str">The string representation of the tower element (e.g., "Air", "Water").</param>
    /// <returns>The corresponding TowerElement enum value.</returns>
    public static TowerElement GetTowerElement(string str)
    {
        if (str.ToLower().Contains("air")) return TowerElement.Air;
        if (str.ToLower().Contains("earth")) return TowerElement.Earth;
        if (str.ToLower().Contains("electric")) return TowerElement.Electric;
        if (str.ToLower().Contains("fire")) return TowerElement.Fire;
        if (str.ToLower().Contains("ice")) return TowerElement.Ice;
        if (str.ToLower().Contains("water")) return TowerElement.Water;

        Debug.LogError("Error in TowerData! Tower element: " + str + " invalid!");
        return TowerElement.Air;
    }
}

/// <summary>
/// Configuration data for a specific tower.
/// </summary>
[System.Serializable]
public class TowerConfig
{
    /// <summary>
    /// The type of the tower.
    /// </summary>
    public TowerType Type { get; set; }
    /// <summary>
    /// The elemental type of the tower.
    /// </summary>
    public TowerElement Element { get; set; }
    /// <summary>
    /// The current level of the tower.
    /// </summary>
    public TowerLevel Level { get; set; }
    /// <summary>
    /// The layer mask for valid placement locations for this tower.
    /// </summary>
    public LayerMask PlacementLayer { get; set; }
    /// <summary>
    /// The layer mask for targets that this tower can attack.
    /// </summary>
    public LayerMask TargetLayer { get; set; }
    /// <summary>
    /// The field of view angle for the tower's targeting system.
    /// </summary>
    public float FieldOfViewAngle { get; set; }

    /// <summary>
    /// The attack range of the tower.
    /// </summary>
    public float Range;
    /// <summary>
    /// The percentage of damage dealt as physical damage.
    /// </summary>
    public float PhysicalDamagePercent;
    /// <summary>
    /// The number of shots fired per second by the tower.
    /// </summary>
    public float ShotsPerSecond;
    /// <summary>
    /// The speed at which the tower's projectiles travel.
    /// </summary>
    public float ProjectileSpeed;
    /// <summary>
    /// The number of projectiles fired per shot (for spray-type towers).
    /// </summary>
    public float NumProjectiles;
    /// <summary>
    /// The cost to build this specific tower configuration.
    /// </summary>
    public float Cost;
}

/// <summary>
/// Configuration data for all levels of a specific tower element.
/// </summary>
[System.Serializable]
public class TowerLevelConfig
{
    /// <summary>
    /// Configuration for level one of the tower.
    /// </summary>
    public TowerConfig LevelOneConfig = new();
    /// <summary>
    /// Configuration for level two of the tower.
    /// </summary>
    public TowerConfig LevelTwoConfig = new();
    /// <summary>
    /// Configuration for level three of the tower.
    /// </summary>
    public TowerConfig LevelThreeConfig = new();

    /// <summary>
    /// Gets the TowerConfig for a specific tower level.
    /// </summary>
    /// <param name="level">The desired tower level.</param>
    /// <returns>The TowerConfig for the specified level, or null if the level is invalid.</returns>
    public TowerConfig GetConfig(TowerLevel level)
    {
        return level switch
        {
            TowerLevel.One => LevelOneConfig,
            TowerLevel.Two => LevelTwoConfig,
            TowerLevel.Three => LevelThreeConfig,
            _ => null,
        };
    }
}

/// <summary>
/// Configuration data for all levels of a specific tower element.
/// </summary>
[System.Serializable]
public class TowerElementConfig
{
    /// <summary>
    /// Level configurations for Water element towers.
    /// </summary>
    public TowerLevelConfig Water = new();
    /// <summary>
    /// Level configurations for Fire element towers.
    /// </summary>
    public TowerLevelConfig Fire = new();
    /// <summary>
    /// Level configurations for Air element towers.
    /// </summary>
    public TowerLevelConfig Air = new();
    /// <summary>
    /// Level configurations for Earth element towers.
    /// </summary>
    public TowerLevelConfig Earth = new();
    /// <summary>
    /// Level configurations for Electric element towers.
    /// </summary>
    public TowerLevelConfig Electric = new();
    /// <summary>
    /// Level configurations for Ice element towers.
    /// </summary>
    public TowerLevelConfig Ice = new();

    /// <summary>
    /// Gets the TowerLevelConfig for a specific tower element.
    /// </summary>
    /// <param name="element">The desired tower element.</param>
    /// <returns>The TowerLevelConfig for the specified element, or null if the element is invalid.</returns>
    public TowerLevelConfig GetLevelConfig(TowerElement element)
    {
        return element switch
        {
            TowerElement.Water => Water,
            TowerElement.Fire => Fire,
            TowerElement.Air => Air,
            TowerElement.Earth => Earth,
            TowerElement.Electric => Electric,
            TowerElement.Ice => Ice,
            _ => null,
        };
    }

    /// <summary>
    /// Gets the TowerConfig for a specific tower element and level.
    /// </summary>
    /// <param name="element">The desired tower element.</param>
    /// <param name="level">The desired tower level.</param>
    /// <returns>The TowerConfig for the specified element and level, or null if either is invalid.</returns>
    public TowerConfig GetConfig(TowerElement element, TowerLevel level)
    {
        return GetLevelConfig(element)?.GetConfig(level);
    }
}

/// <summary>
/// A collection of tower configurations, organized by tower type and element.
/// </summary>
[System.Serializable]
public class TowerConfigCollection
{
    /// <summary>
    /// General configuration settings for all towers.
    /// </summary>
    [Header("General Config")]
    /// <summary>
    /// The layer mask for targets that all towers can potentially target.
    /// </summary>
    public LayerMask TargetingLayer;
    /// <summary>
    /// The layer mask for valid placement locations for all towers.
    /// </summary>
    public LayerMask PlacementLayer;
    /// <summary>
    /// The default field of view angle for all towers.
    /// </summary>
    public float FieldOfViewAngle = 360;

    /// <summary>
    /// Specific configurations for Aiming type towers.
    /// </summary>
    [Header("Specific Config")]
    public TowerElementConfig Aiming = new();
    /// <summary>
    /// Specific configurations for Spray type towers.
    /// </summary>
    public TowerElementConfig Spray = new();
    /// <summary>
    /// Specific configurations for Area of Effect type towers.
    /// </summary>
    public TowerElementConfig AreaOfEffect = new();

    /// <summary>
    /// Gets the TowerConfig for a specific tower type, element, and level.
    /// </summary>
    /// <param name="type">The desired tower type.</param>
    /// <param name="element">The desired tower element.</param>
    /// <param name="level">The desired tower level.</param>
    /// <returns>The TowerConfig for the specified type, element, and level, or null if any is invalid.</returns>
    public TowerConfig GetConfig(TowerType type, TowerElement element, TowerLevel level)
    {
        TowerConfig config = type switch
        {
            TowerType.Aiming => Aiming.GetConfig(element, level),
            TowerType.Spray => Spray.GetConfig(element, level),
            TowerType.AreaOfAffect => AreaOfEffect.GetConfig(element, level),
            _ => null,
        };

        config.Type = type;
        config.Element = element;
        config.Level = level;
        config.TargetLayer = TargetingLayer;
        config.PlacementLayer = PlacementLayer;
        config.FieldOfViewAngle = FieldOfViewAngle;

        return config;
    }
}