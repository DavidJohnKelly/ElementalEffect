using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the flow of rounds in the game, including starting rounds and managing enemy spawning.
/// </summary>
public class RoundController : MonoBehaviour
{
    /// <summary>
    /// A list of RoundData objects defining the parameters for each round.
    /// </summary>
    [Tooltip("A list of RoundData objects defining the parameters for each round.")]
    [SerializeField] private List<RoundData> rounds = new();

    /// <summary>
    /// The current round index.
    /// </summary>
    public int RoundIndex { get; private set; } = 0;

    // Updates the names of the rounds in the editor.
    private void OnValidate()
    {
        for (int i = 0; i < rounds.Count; i++)
        {
            rounds[i].UpdateNames(i);
        }
    }

    /// <summary>
    /// Starts the next round if available.
    /// </summary>
    /// <returns>True if this was the last round, false otherwise.</returns>
    public bool StartRound()
    {
        if (rounds.Count == 0 || RoundIndex >= rounds.Count)
        {
            Debug.LogError("Error in RoundController! No rounds configured!");
            return true;
        }

        RoundData currentRound = rounds[RoundIndex];
        StartCoroutine(ExecuteRound(currentRound));
        return RoundIndex == rounds.Count - 1;
    }

    // Executes the logic for a single round.
    private IEnumerator ExecuteRound(RoundData round)
    {
        foreach (WaveData wave in round.waves)
        {
            yield return SpawnWave(wave);
            yield return new WaitForSeconds(wave.waveInterval);
        }

        RoundIndex++;
    }

    // Spawns all waves within a round.
    private IEnumerator SpawnWave(WaveData wave)
    {
        foreach (EnemyCollectionData enemyCollection in wave.enemySpawnData)
        {
            for (int i = 0; i < enemyCollection.enemyCount; i++)
            {
                EnemyFactory.Instance.GetEnemy(enemyCollection.enemyType);
                yield return new WaitForSeconds(enemyCollection.spawnInterval);
            }
        }
    }

    /// <summary>
    /// Gets the total number of enemies in the next round.
    /// </summary>
    /// <returns>The total number of enemies in the next round, or 0 if there are no more rounds.</returns>
    public int GetNextRoundEnemyCount()
    {
        if (RoundIndex < 0 || RoundIndex >= rounds.Count)
        {
            Debug.LogError("Invalid round index");
            return 0;
        }

        int totalEnemies = 0;
        RoundData round = rounds[RoundIndex];

        foreach (WaveData wave in round.waves)
        {
            foreach (EnemyCollectionData enemyCollectionData in wave.enemySpawnData)
            {
                totalEnemies += enemyCollectionData.enemyCount;
            }
        }

        return totalEnemies;
    }

    /// <summary>
    /// Gets a list of enemy types and their counts for a specific round.
    /// </summary>
    /// <param name="roundNum">The index of the round to retrieve enemy data for.</param>
    /// <returns>A list of tuples containing the EnemyType and the count for the specified round, or null if the round index is invalid.</returns>
    public List<(EnemyType, int)> GetEnemyList(int roundNum)
    {
        if (roundNum < 0 || roundNum >= rounds.Count)
        {
            Debug.LogError("Error in RoundController! Round: " + roundNum + " is out of the index of total rounds: " + rounds.Count);
            return null;
        }

        RoundData roundData = rounds[roundNum];

        List<(EnemyType, int)> enemyData = new();

        foreach (WaveData wave in roundData.waves)
        {
            foreach (EnemyCollectionData enemyCollectionData in wave.enemySpawnData)
            {
                enemyData.Add((enemyCollectionData.enemyType, enemyCollectionData.enemyCount));
            }
        }

        return enemyData;
    }

    /// <summary>
    /// Gets a list of enemy types and their counts for the current round.
    /// </summary>
    /// <returns>A list of tuples containing the EnemyType and the count for the current round.</returns>
    public List<(EnemyType, int)> GetCurrentEnemyList()
    {
        return GetEnemyList(RoundIndex);
    }
}