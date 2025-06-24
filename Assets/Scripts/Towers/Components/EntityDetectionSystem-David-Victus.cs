using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A system that detects entities within a specified range and field of view.
/// </summary>
public class EntityDetectionSystem : MonoBehaviour
{
    /// <summary>
    /// The angle of the field of view in degrees.
    /// </summary>
    [Tooltip("The angle of the field of view in degrees.")]
    public float fieldOfViewAngle = 360f;
    /// <summary>
    /// The LayerMask to filter for target objects.
    /// </summary>
    [Tooltip("The LayerMask to filter for target objects.")]
    public LayerMask targetLayer;
    /// <summary>
    /// Whether to draw debug gizmos in the editor.
    /// </summary>
    [Tooltip("Whether to draw debug gizmos in the editor.")]
    [SerializeField] private bool debugGizmos = true;

    // Reference to the parent TowerController.
    private TowerController towerController;
    // An array to store colliders found within the detection radius (used for non-alloc OverlapSphere).
    private readonly Collider[] colliders = new Collider[25];
    // The current detection radius, typically obtained from the TowerController.
    private float detectionRadius;

    /// <summary>
    /// Retrieves the TowerController component from the parent.
    /// </summary>
    private void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
    }

    // Detects targets within the detection radius and optionally requires line of sight.
    private List<Transform> DetectTargets(bool requireLineOfSight = true)
    {
        detectionRadius = towerController.Range;
        List<Transform> detectedTargets = new();
        int hits = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, colliders, targetLayer);

        for (int i = 0; i < hits; i++)
        {
            Collider collider = colliders[i];
            Transform targetTransform = collider.transform;
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;

            if (IsInFieldOfView(directionToTarget) && (!requireLineOfSight || HasLineOfSight(targetTransform)))
            {
                detectedTargets.Add(targetTransform);
            }
        }
        return detectedTargets;
    }

    // Checks if a given direction is within the field of view.
    private bool IsInFieldOfView(Vector3 direction)
    {
        if (fieldOfViewAngle == 360) return true;

        float angle = Vector3.Angle(transform.forward, direction);
        return angle <= fieldOfViewAngle * 0.5f;
    }

    // Checks if there is a clear line of sight to the target (currently always returns true).
    private bool HasLineOfSight(Transform target)
    {
        //if (Physics.Linecast(transform.position, target.position, out RaycastHit hit))
        //    return hit.collider.transform == target;
        //return false;

        return true; // Don't worry about it
    }

    /// <summary>
    /// Attempts to get the nearest target's position and velocity within the detection range and field of view.
    /// </summary>
    /// <param name="position">Out parameter for the nearest target's position.</param>
    /// <param name="velocity">Out parameter for the nearest target's velocity.</param>
    /// <returns>True if a target was found, false otherwise.</returns>
    public bool TryGetNearestTarget(out Vector3 position, out Vector3 velocity)
    {
        position = Vector3.zero;
        velocity = Vector3.zero;

        List<Transform> detectedTargets = DetectTargets();

        if (detectedTargets.Count == 0) return false;

        float nearestDistance = float.MaxValue;
        Transform nearest = null;

        foreach (Transform targetTransform in detectedTargets)
        {
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            if (distance >= nearestDistance) continue;

            nearestDistance = distance;
            nearest = targetTransform;
        }

        if (nearest == null) return false;

        position = nearest.transform.position;
        velocity = GetTargetVelocity(nearest.gameObject);

        return velocity.magnitude != 0;
    }

    /// <summary>
    /// Gets a list of all detected target GameObjects within the detection range and field of view.
    /// Line of sight is not required for this method.
    /// </summary>
    /// <returns>A list of detected target GameObjects.</returns>
    public List<GameObject> GetAllDetectedTargets()
    {
        List<Transform> detectedTargets = DetectTargets(false);
        List<GameObject> detectedObjects = new();

        foreach (Transform target in detectedTargets)
        {
            detectedObjects.Add(target.gameObject);
        }

        return detectedObjects;
    }

    // Gets the velocity of a target GameObject by checking for a PathingController component.
    private Vector3 GetTargetVelocity(GameObject col)
    {
        var pathing = col.GetComponentInChildren<PathingController>();
        if (pathing == null) return Vector3.zero;
        else return pathing.Velocity;
    }

    /// <summary>
    /// Draws debug gizmos in the editor to visualize the detection range and field of view.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!debugGizmos) return;
        // Avoid communication for debug draws
        float detectionRadius = transform.parent.GetComponentInChildren<RangeController>().GetRangeRadius();

        Vector3 leftLimit = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * transform.forward * detectionRadius;
        Vector3 rightLimit = Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * transform.forward * detectionRadius;

        if (fieldOfViewAngle != 360)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + leftLimit);
            Gizmos.DrawLine(transform.position, transform.position + rightLimit);
        }
    }
}