using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages game state, mode switching between FPS and Tower Defense, and core game systems.
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("Tower Defence Objects")]
    [Tooltip("Camera used during Tower Defense mode")]
    public GameObject towerDefenceCamera;
    [Tooltip("UI controller for Tower Defense mode")]
    public TowerDefenceUIController towerDefenceUI;
    [Tooltip("Background audio for Tower Defense mode")]
    public AudioClip towerDefenceAudio;

    [Header("FPS Objects")]
    [Tooltip("Camera used during FPS mode")]
    public GameObject FPSCamera;
    [Tooltip("UI text displaying player health")]
    public TMP_Text HealthText;
    [Tooltip("Background audio for FPS mode")]
    public AudioClip FPSAudio;

    [Header("Game Settings")]
    [Tooltip("Starting money amount")]
    [SerializeField] private float initialMoney;
    [Tooltip("Money multiplier applied after each round")]
    [SerializeField] private float roundMoneyMultiplier = 1.5f;
    [Tooltip("Starting health value")]
    [SerializeField] private float initialHealth;
    [Tooltip("Scene to load when winning")]
    [SerializeField] private string winSceneString;
    [Tooltip("Scene to load when losing")]
    [SerializeField] private string loseSceneString;

    public float CurrentMoney { get; private set; }
    public static GameController Instance { get; private set; }
    private bool inFPSStage = false;
    public bool InFPSStage { get => inFPSStage; }
    private bool finalRound = false;
    public bool FinalRound { get => finalRound; set => finalRound = value; }

    private RoundController roundController;
    private int enemyCount;
    private float health;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            roundController = GetComponentInChildren<RoundController>();
        }
        else
        {
            Debug.LogWarning("Multiple instances of GameController detected.");
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();

        health = initialHealth;

        SwitchToTowerDefence();
    }

    /// <summary>
    /// Switches game mode to FPS perspective.
    /// </summary>
    public void SwitchToFPS()
    {
        enemyCount = roundController.GetNextRoundEnemyCount();
        finalRound = roundController.StartRound();

        if (towerDefenceCamera != null)
        {
            towerDefenceCamera.SetActive(false);
        }
        if (FPSCamera != null)
        {
            FPSCamera.SetActive(true);
            PlayerSpawnController playerSpawnController = FindFirstObjectByType<PlayerSpawnController>(FindObjectsInactive.Include);
            playerSpawnController.gameObject.SetActive(true);
            playerSpawnController.SetSpawnLocation(FPSCamera);
            playerSpawnController.gameObject.SetActive(false);
        }

        inFPSStage = true;
        UpdateHealth(0);

        DeselectAll();

        PlayAudio(FPSAudio);
    }

    /// <summary>
    /// Switches game mode to Tower Defense perspective.
    /// </summary>
    public void SwitchToTowerDefence()
    {
        CurrentMoney += Mathf.RoundToInt(initialMoney * Mathf.Pow(roundMoneyMultiplier, roundController.RoundIndex));

        if (towerDefenceCamera != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            towerDefenceCamera.SetActive(true);
        }
        if (FPSCamera != null)
        {
            FPSCamera.SetActive(false);
        }

        PlayerSpawnController playerSpawnController = FindFirstObjectByType<PlayerSpawnController>(FindObjectsInactive.Include);
        playerSpawnController.gameObject.SetActive(true);
        inFPSStage = false;

        PlayAudio(towerDefenceAudio);
    }

    // Plays the specified audio clip as background music
    private void PlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Stop();
        audioSource.loop = true;
        audioSource.Play();
    }

    /// <summary>
    /// Applies damage when enemies reach the end point in Tower Defense mode.
    /// </summary>
    /// <param name="endDamage">Amount of damage to apply</param>
    public void HandleEndDamage(float endDamage)
    {
        UpdateHealth(endDamage);
        HandleEnemyRemoved();
    }

    // Updates health and checks for game over
    private void UpdateHealth(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            SceneManager.LoadScene(loseSceneString);
        }
        if (HealthText.isActiveAndEnabled)
        {
            HealthText.text = "Health: " + Mathf.Max(0f, Mathf.RoundToInt(health)) + "/ " + initialHealth;
        }
    }

    // Handles enemy removal and checks for round completion
    private void HandleEnemyRemoved()
    {
        enemyCount--;

        if (enemyCount == 0)
        {
            if (finalRound)
            {
                SceneManager.LoadScene(winSceneString);
                return;
            }
            else
            {
                SwitchToTowerDefence();
            }
        }
    }

    /// <summary>
    /// Handles enemy death and rewards player with money.
    /// </summary>
    /// <param name="destroyReward">Money reward for killing enemy</param>
    public void HandleEnemyDeath(float destroyReward)
    {
        CurrentMoney += destroyReward;
        HandleEnemyRemoved();
    }

    /// <summary>
    /// Increments enemy count when new enemy spawns.
    /// </summary>
    public void HandleEnemyAdded()
    {
        enemyCount++;
    }

    /// <summary>
    /// Deducts money when purchasing towers/upgrades.
    /// </summary>
    /// <param name="cost">Amount to deduct</param>
    public void ApplyCost(float cost)
    {
        CurrentMoney -= cost;
        towerDefenceUI.UpdateResourceAmount(CurrentMoney);
    }

    /// <summary>
    /// Deselects all currently selected placeable objects.
    /// </summary>
    public void DeselectAll()
    {
        PlaceableObject[] selectableObjects = FindObjectsByType<PlaceableObject>(FindObjectsSortMode.InstanceID);
        foreach (PlaceableObject obj in selectableObjects)
        {
            obj.Deselect();
        }
    }

    /// <summary>
    /// Gets current round's enemy composition.
    /// </summary>
    /// <returns>List of enemy types and counts</returns>
    public List<(EnemyType, int)> GetCurrentEnemyList()
    {
        return roundController.GetCurrentEnemyList();
    }
}