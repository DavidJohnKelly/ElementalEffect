using System.Collections;
using UnityEngine;

/// <summary>
/// Controls enemy behavior including movement, death handling, damage, and bonuses.
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("Type Settings")]
    [Tooltip("Type classification of this enemy")]
    public EnemyType Type;

    [Header("Destroy Settings")]
    [Tooltip("Layer representing the player for collision detection")]
    [SerializeField] private LayerMask playerLayer;

    [Tooltip("Prefab used for explosion effect when enemy is destroyed")]
    [SerializeField] private GameObject explosionPrefab;

    [Tooltip("Audio clip played when the enemy is destroyed")]
    [SerializeField] private AudioClip explosionSound;

    [Tooltip("Reward given to the player upon destroying this enemy")]
    public float DestroyReward;

    [Header("Model Settings")]
    [Tooltip("Visual model to represent this enemy")]
    [SerializeField] private GameObject objectModel;

    [Tooltip("Scale factor applied to the enemy's visual model")]
    [SerializeField] private float objectScale = 1.0f;

    [Tooltip("Speed at which the enemy model rotates to face movement direction")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Player Bonus Settings")]
    [Tooltip("Text displayed when player kills this enemy")]
    [SerializeField] private string bonusText = "BONUS!";

    [Tooltip("Font size of the bonus text")]
    [SerializeField] private float bonusFontSize = 20f;

    [Tooltip("Color of the bonus text")]
    [SerializeField] private Color bonusColour = Color.yellow;

    [Tooltip("Multiplier applied to rewards when killed by the player")]
    [SerializeField] private float bonusMultiplier = 2f;

    /// <summary>
    /// The path the enemy is currently following.
    /// </summary>
    public SplinePath Path { get => pathingController.Path; }

    /// <summary>
    /// Current pathing state of the enemy.
    /// </summary>
    public PathState PathState { get => pathingController.PathState; }

    private GameController gameController;
    private PathingController pathingController;
    private ElementResistanceController elementResistanceController;
    private EndDamageController endDamageController;
    private HealthController healthController;
    private GameObject spawnedModel;
    private BoxCollider boxCollider;
    private AudioSource audioSource;

    [Header("Teleport Settings (For Teleporting Enemy Type)")]
    [Tooltip("Minimum time between teleports (seconds). Increase for less frequent teleports.")]
    [SerializeField] private float minTeleportInterval = 5f;

    [Tooltip("Maximum time between teleports (seconds). Randomly chosen in this range for unpredictability.")]
    [SerializeField] private float maxTeleportInterval = 15f;

    [Tooltip("Maximum steps (knots) ahead to consider for teleportation. Limits distance for fairness.")]
    [SerializeField] private int maxTeleportSteps = 3;

    [Tooltip("Maximum world-space distance for teleport targets. Prevents long-range jumps.")]
    [SerializeField] private float maxTeleportDistance = 20f;

    [Tooltip("Optional prefab for visual effect on teleport (e.g., particles).")]
    [SerializeField] private GameObject teleportEffectPrefab;

    [Tooltip("Optional audio clip for teleport sound.")]
    [SerializeField] private AudioClip teleportSound;

    private bool dead = false;

    private void Awake()
    {
        gameController = GameController.Instance;
        pathingController = GetComponentInChildren<PathingController>();
        elementResistanceController = GetComponentInChildren<ElementResistanceController>();
        endDamageController = GetComponentInChildren<EndDamageController>();
        healthController = GetComponentInChildren<HealthController>();
        boxCollider = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (objectModel == null)
        {
            Debug.LogWarning("Model not assigned in inspector");
            return;
        }

        spawnedModel = Instantiate(objectModel, transform.position, Quaternion.identity);
        spawnedModel.transform.parent = transform;
        spawnedModel.transform.localScale = new Vector3(objectScale, objectScale, objectScale);
        AdjustColliderBounds();

        if (Type == EnemyType.Teleporting)
        {
            StartCoroutine(TeleportRoutine());
        }
    }

    /// <summary>
    /// Coroutine to handle random teleport timing for the teleporting enemy type.
    /// </summary>
    private IEnumerator TeleportRoutine()
    {
        while (!dead)
        {
            // Wait a random interval for unpredictability and to avoid frequent teleports
            float waitTime = Random.Range(minTeleportInterval, maxTeleportInterval);
            yield return new WaitForSeconds(waitTime);

            // Attempt to teleport if conditions allow
            Teleport();
        }
    }

    /// <summary>
    /// Handles teleportation to a nearby knot for the teleporting enemy type.
    /// Updates position and PathState instantly, then resumes normal movement.
    /// </summary>
    private void Teleport()
    {
        if (dead || pathingController == null || pathingController.Path == null) return;

        // Get nearby valid teleport targets (nearby knots within limits)
        var possibleTargets = pathingController.Path.GetNearbyKnotsForTeleport(
            pathingController.PathState,
            maxTeleportSteps,
            maxTeleportDistance
        );

        if (possibleTargets.Count == 0) return; // No valid spots, skip teleport

        // Randomly select a target for unpredictability
        var random = new System.Random(pathingController.PathState.Seed.Next());
        int index = random.Next(possibleTargets.Count);
        var target = possibleTargets[index];

        // Update PathState to the new knot (resumes normal pathing from there)
        pathingController.PathState = pathingController.Path.CreatePathState(target.Spline, target.Knot);

        // Instantly move to the new position
        transform.position = pathingController.PathState.NextWaypoint;

        // Optional: Play effects for player feedback
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        }
        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }
    }

    private void Update()
    {
        if (pathingController == null || spawnedModel == null)
        {
            return;
        }

        Vector3 movementDirection = pathingController.GetMovementDirection();

        if (movementDirection != Vector3.zero)
        {
            RotateModel(movementDirection);
        }
    }

    private void RotateModel(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void AdjustColliderBounds()
    {
        if (spawnedModel == null || boxCollider == null) return;

        Renderer[] renderers = spawnedModel.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds combinedBounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        Vector3 localCenter = transform.InverseTransformPoint(combinedBounds.center);
        Vector3 localSize = combinedBounds.size;

        localSize.x /= transform.lossyScale.x;
        localSize.y /= transform.lossyScale.y;
        localSize.z /= transform.lossyScale.z;

        boxCollider.center = localCenter;
        boxCollider.size = localSize;
    }

    /// <summary>
    /// Handles enemy death and triggers appropriate effects and rewards.
    /// </summary>
    public void HandleEnemyDied(bool isFromPlayer)
    {
        if (dead) return;

        dead = true;
        if (isFromPlayer)
        {
            TextFactory.Instance.CreateText(bonusText, bonusColour, bonusFontSize, transform.position);
        }
        gameController.HandleEnemyDeath(DestroyReward * (isFromPlayer ? bonusMultiplier : 1f));
        Explode();
    }

    private void OnDestroy()
    {
        if (spawnedModel != null)
        {
            Destroy(spawnedModel);
        }
    }

    /// <summary>
    /// Called when the enemy reaches the end of the path.
    /// </summary>
    public void HandlePathFinished()
    {
        if (dead) return;

        dead = true;
        endDamageController.TriggerEndDamage();
        Explode();
    }

    private void Explode()
    {
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1f);
        }
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    /// <summary>
    /// Applies elemental damage to the enemy.
    /// </summary>
    public void HandleElementalDamage(TowerElement element, float damage)
    {
        elementResistanceController.TakeElementalDamage(element, damage);
    }

    /// <summary>
    /// Applies physical damage to the enemy.
    /// </summary>
    public void HandlePhysicalDamage(float damage, bool isFromPlayer = false)
    {
        healthController.TakeDamage(damage, isFromPlayer);
    }

    /// <summary>
    /// Gets the enemy's vertical radius based on its collider.
    /// </summary>
    public float GetEnemyRadius()
    {
        if (boxCollider == null) return 1f;
        else return boxCollider.bounds.extents.y;
    }

    /// <summary>
    /// Gets the enemy's current speed.
    /// </summary>
    public float GetEnemySpeed()
    {
        return pathingController.Speed;
    }

    /// <summary>
    /// Temporarily modifies the enemy's speed.
    /// </summary>
    public void ModifySpeedForTime(float speed, float seconds)
    {
        if (pathingController == null) return;
        StopCoroutine(nameof(ResetSpeedAfterDelay));
        pathingController.SetSpeed(speed);
        StartCoroutine(ResetSpeedAfterDelay(seconds));
    }

    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        pathingController.ResetSpeed();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            HandlePathFinished();
        }
    }
}