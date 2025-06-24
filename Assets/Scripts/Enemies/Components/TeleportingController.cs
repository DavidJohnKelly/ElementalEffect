using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls teleportation behavior for enemies that can randomly teleport to nearby nodes.
/// </summary>
public class TeleportingController : MonoBehaviour
{
    [Header("Teleportation Settings")]
    [Tooltip("Minimum time between teleportations")]
    [SerializeField] private float minTeleportInterval = 8f;

    [Tooltip("Maximum time between teleportations")]
    [SerializeField] private float maxTeleportInterval = 15f;

    [Tooltip("Maximum distance (in nodes) the enemy can teleport")]
    [SerializeField] private int maxTeleportDistance = 3;

    [Tooltip("Minimum distance (in nodes) the enemy must teleport")]
    [SerializeField] private int minTeleportDistance = 1;

    [Tooltip("Visual effect prefab for teleportation")]
    [SerializeField] private GameObject teleportEffectPrefab;

    [Tooltip("Audio clip for teleportation sound")]
    [SerializeField] private AudioClip teleportSound;

    private PathingController pathingController;
    private SplinePath splinePath;
    private EnemyController enemyController;
    private AudioSource audioSource;
    private Coroutine teleportCoroutine;
    private bool canTeleport = true;

    private void Awake()
    {
        pathingController = GetComponent<PathingController>();
        enemyController = GetComponentInParent<EnemyController>();
        audioSource = GetComponentInParent<AudioSource>();
    }

    private void Start()
    {
        splinePath = pathingController.Path;
        if (splinePath != null && canTeleport)
        {
            StartTeleportTimer();
        }
    }

    private void StartTeleportTimer()
    {
        if (teleportCoroutine != null)
        {
            StopCoroutine(teleportCoroutine);
        }
        teleportCoroutine = StartCoroutine(TeleportTimer());
    }

    private IEnumerator TeleportTimer()
    {
        while (canTeleport)
        {
            float waitTime = Random.Range(minTeleportInterval, maxTeleportInterval);
            yield return new WaitForSeconds(waitTime);

            if (canTeleport)
            {
                AttemptTeleport();
            }
        }
    }

    private void AttemptTeleport()
    {
        if (pathingController.PathState == null || splinePath == null)
            return;

        List<PathState> validTeleportTargets = GetValidTeleportTargets();

        if (validTeleportTargets.Count > 0)
        {
            PathState targetState = validTeleportTargets[Random.Range(0, validTeleportTargets.Count)];
            PerformTeleport(targetState);
        }
    }

    private List<PathState> GetValidTeleportTargets()
    {
        List<PathState> validTargets = new List<PathState>();
        PathState currentState = pathingController.PathState;

        // Get all possible nodes within teleport range
        HashSet<PathState> exploredStates = new HashSet<PathState>();
        Queue<(PathState state, int distance)> stateQueue = new Queue<(PathState, int)>();

        stateQueue.Enqueue((currentState, 0));
        exploredStates.Add(currentState);

        while (stateQueue.Count > 0)
        {
            var (state, distance) = stateQueue.Dequeue();

            if (distance >= minTeleportDistance && distance <= maxTeleportDistance)
            {
                validTargets.Add(state);
            }

            if (distance < maxTeleportDistance)
            {
                List<UnityEngine.Splines.SplineKnotIndex> availableRoutes = splinePath.GetAvailableRoutes(state);

                foreach (var route in availableRoutes)
                {
                    Vector3 worldPosition = splinePath.GetKnotWorldPosition(route.Spline, route.Knot);
                    PathState nextState = new PathState(worldPosition, route.Spline, route.Knot);

                    // Use a simple comparison based on spline and knot indices to avoid duplicates
                    bool alreadyExplored = false;
                    foreach (var explored in exploredStates)
                    {
                        if (explored.CurrentSplineIndex == nextState.CurrentSplineIndex &&
                            explored.CurrentKnotIndex == nextState.CurrentKnotIndex)
                        {
                            alreadyExplored = true;
                            break;
                        }
                    }

                    if (!alreadyExplored)
                    {
                        exploredStates.Add(nextState);
                        stateQueue.Enqueue((nextState, distance + 1));
                    }
                }
            }
        }

        return validTargets;
    }

    private void PerformTeleport(PathState targetState)
    {
        // Play teleport effect at current position
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.parent.position, Quaternion.identity);
        }

        // Play teleport sound
        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }

        // Update position and path state
        transform.parent.position = targetState.NextWaypoint;
        pathingController.PathState = targetState;

        // Play teleport effect at destination
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, targetState.NextWaypoint, Quaternion.identity);
        }
    }

    /// <summary>
    /// Disables teleportation ability (useful for status effects or game states)
    /// </summary>
    public void DisableTeleportation()
    {
        canTeleport = false;
        if (teleportCoroutine != null)
        {
            StopCoroutine(teleportCoroutine);
            teleportCoroutine = null;
        }
    }

    /// <summary>
    /// Re-enables teleportation ability
    /// </summary>
    public void EnableTeleportation()
    {
        canTeleport = true;
        StartTeleportTimer();
    }

    private void OnDestroy()
    {
        if (teleportCoroutine != null)
        {
            StopCoroutine(teleportCoroutine);
        }
    }
}