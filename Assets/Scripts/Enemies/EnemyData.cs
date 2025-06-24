/// <summary>
/// Defines the different types of enemies in the game.
/// </summary>
public enum EnemyType
{
    /// <summary> Slow and weak enemy type. </summary>
    SlowWeak,

    /// <summary> Fast and weak enemy type. </summary>
    FastWeak,

    /// <summary> Fast and strong enemy type. </summary>
    FastStrong,

    /// <summary> A spawning enemy that may spawn other enemies. </summary>
    Spawning,

    Teleporer,

    /// <summary> A powerful boss enemy. </summary>
    Boss
}

/// <summary>
/// Provides helper methods for enemy types.
/// </summary>
public static class EnemyData
{
    /// <summary>
    /// Gets the string representation of the given <see cref="EnemyType"/>.
    /// </summary>
    /// <param name="type">The enemy type.</param>
    /// <returns>A string representing the enemy type.</returns>
    public static string GetTypeString(EnemyType type)
    {
        return type switch
        {
            EnemyType.SlowWeak => "Basic",
            EnemyType.FastWeak => "Fast",
            EnemyType.FastStrong => "Tank",
            EnemyType.Spawning => "Mothership",
            EnemyType.Boss => "BOSS!",
            _ => "NULL",
        };
    }
}
