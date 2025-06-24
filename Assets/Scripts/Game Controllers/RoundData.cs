using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Represents a collection of enemies to be spawned with a specific type and count.
/// </summary>
[System.Serializable]
public class EnemyCollectionData
{
    /// <summary>
    /// The name of this enemy collection (for editor display).
    /// </summary>
    public string enemyName;
    /// <summary>
    /// The type of enemy to spawn.
    /// </summary>
    [Tooltip("Type of enemy to spawn")] public EnemyType enemyType;
    /// <summary>
    /// The total number of enemies of this type to spawn.
    /// </summary>
    [Tooltip("Total enemies to spawn")] public int enemyCount;
    /// <summary>
    /// The delay in seconds between each individual enemy spawn in this collection.
    /// </summary>
    [Tooltip("Delay between each spawn in seconds")] public float spawnInterval;

    /// <summary>
    /// Updates the name of this enemy collection based on its index.
    /// </summary>
    /// <param name="index">The index of this enemy collection in a list.</param>
    public void UpdateName(int index)
    {
        enemyName = $"Enemy Collection {index + 1}";
    }
}

/// <summary>
/// Represents a wave of enemies to be spawned, containing multiple enemy collections.
/// </summary>
[System.Serializable]
public class WaveData
{
    /// <summary>
    /// The name of this wave (for editor display).
    /// </summary>
    public string waveName;
    /// <summary>
    /// The delay in seconds before the next wave starts after this one finishes.
    /// </summary>
    [Tooltip("Delay between each wave in seconds")] public float waveInterval;
    /// <summary>
    /// A list of EnemyCollectionData objects defining the enemies to spawn in this wave.
    /// </summary>
    public List<EnemyCollectionData> enemySpawnData;

    /// <summary>
    /// Updates the name of this wave and the names of its enemy collections based on their indices.
    /// </summary>
    /// <param name="index">The index of this wave in a list.</param>
    public void UpdateNames(int index)
    {
        waveName = $"Wave {index + 1}";
        for (int i = 0; i < enemySpawnData.Count; i++)
        {
            enemySpawnData[i].UpdateName(i);
        }
    }
}

/// <summary>
/// Represents a round in the game, containing multiple waves of enemies.
/// </summary>
[System.Serializable]
public class RoundData
{
    /// <summary>
    /// The name of this round (for editor display).
    /// </summary>
    public string roundName;
    /// <summary>
    /// A list of WaveData objects defining the waves in this round.
    /// </summary>
    public List<WaveData> waves;

    /// <summary>
    /// Updates the name of this round and the names of its waves based on their indices.
    /// </summary>
    /// <param name="index">The index of this round in a list.</param>
    public void UpdateNames(int index)
    {
        roundName = $"Round {index + 1}";
        for (int i = 0; i < waves.Count; i++)
        {
            waves[i].UpdateNames(i);
        }
    }
}