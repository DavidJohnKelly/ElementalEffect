using System.Collections.Generic;
using UnityEngine;

public class EntityDetectionSystem : MonoBehaviour
{
    public float fieldOfViewAngle = 360f;
    public LayerMask targetLayer;
    [SerializeField] private bool debugGizmos = true;

    private TowerController towerController;
    private readonly Collider[] colliders = new Collider[3];
    private float detectionRadius;

    private void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
    }

    private List<Transform> DetectTargets()
    {
        detectionRadius = towerController.Radius;
        List<Transform> detectedTargets = new();
        int hits = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, colliders, targetLayer);

        for (int i = 0; i < hits; i++)
        {
            Collider collider = colliders[i];
            Transform targetTransform = collider.transform;
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;

            if (IsInFieldOfView(directionToTarget) && HasLineOfSight(targetTransform))
            {
                detectedTargets.Add(targetTransform);
            }
        }
        return detectedTargets;
    }

    private bool IsInFieldOfView(Vector3 direction)
    {
        if (fieldOfViewAngle == 360) return true;

        float angle = Vector3.Angle(transform.forward, direction);
        return angle <= fieldOfViewAngle * 0.5f;
    }

    private bool HasLineOfSight(Transform target)
    {
        if (Physics.Linecast(transform.position, target.position, out RaycastHit hit))
            return hit.collider.transform == target;
        return false;
    }

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

    private Vector3 GetTargetVelocity(GameObject col)
    {
        var pathing = col.GetComponentInChildren<PathingController>();
        if (pathing == null) return Vector3.zero;
        else return pathing.Velocity;
    }

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