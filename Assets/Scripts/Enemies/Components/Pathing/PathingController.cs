using UnityEngine;

/// <summary>
/// Controls enemy movement along a spline path.
/// </summary>
public class PathingController : MonoBehaviour
{
    [Tooltip("Default base speed of the enemy")]
    [SerializeField] private float defaultSpeed = 10.0f;
    private float currentSpeed;

    /// <summary>
    /// The spline path the enemy follows.
    /// </summary>
    [Tooltip("The spline path the enemy follows")]
    public SplinePath Path { get; set; }

    /// <summary>
    /// Current path state including the next waypoint.
    /// </summary>
    [Tooltip("Current path state including the next waypoint")]
    public PathState PathState { get; set; }

    /// <summary>
    /// Current movement speed of the enemy.
    /// </summary>
    [Tooltip("Current movement speed of the enemy")]
    public float Speed { get => currentSpeed; }

    /// <summary>
    /// Current velocity vector towards the next waypoint.
    /// </summary>
    [Tooltip("Current velocity vector towards the next waypoint")]
    public Vector3 Velocity { get => (PathState.NextWaypoint - transform.parent.position).normalized * currentSpeed; }

    private readonly float distance_threshold = 0.1f * 0.1f; // Squared to avoid performance impact of Vector3.Distance for all enemies
    private EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
        currentSpeed = defaultSpeed;
    }

    /// <summary>
    /// Initializes the path state at the beginning of the path.
    /// </summary>
    public void InitialisePathState()
    {
        PathState = new PathState
        {
            NextWaypoint = Path.GetFirstPosition()
        };
    }

    private void Update()
    {
        transform.parent.position = Vector3.MoveTowards(
            transform.parent.position,
            PathState.NextWaypoint,
            currentSpeed * Time.deltaTime
        );

        if ((transform.parent.position - PathState.NextWaypoint).sqrMagnitude < distance_threshold)
        {
            PathState nextState = Path.GetNextPathState(PathState);
            if (nextState == null)
            {
                enemyController.HandlePathFinished();
            }
            else
            {
                PathState = Path.GetNextPathState(PathState);
            }
        }
    }

    /// <summary>
    /// Gets the normalized direction vector towards the next waypoint.
    /// </summary>
    public Vector3 GetMovementDirection()
    {
        if (PathState == null)
        {
            InitialisePathState();
        }
        return (PathState.NextWaypoint - transform.position).normalized;
    }

    /// <summary>
    /// Sets the enemy's movement speed.
    /// </summary>
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }

    /// <summary>
    /// Resets the enemy's speed to the default value.
    /// </summary>
    public void ResetSpeed()
    {
        currentSpeed = defaultSpeed;
    }
}
