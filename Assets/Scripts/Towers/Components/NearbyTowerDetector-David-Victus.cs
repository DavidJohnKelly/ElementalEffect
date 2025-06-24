using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects nearby towers and notifies the parent TowerController about their presence and element.
/// </summary>
public class NearbyTowerDetector : MonoBehaviour
{
    // Reference to the parent TowerController.
    private TowerController towerController;
    // The SphereCollider used to detect nearby towers.
    private SphereCollider sphereCollider;
    // The detection range for nearby towers.
    private float range;
    // A set to store colliders of overlapping towers to avoid duplicates.
    private readonly HashSet<Collider> overlappingColliders = new();
    // A list to store colliders that need to be removed from the overlappingColliders set.
    private readonly List<Collider> toRemove = new();

    /// <summary>
    /// Retrieves the TowerController and SphereCollider components.
    /// </summary>
    private void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    /// <summary>
    /// Checks for and removes any invalid or inactive tower colliders from the tracked list.
    /// </summary>
    private void Update()
    {
        if (overlappingColliders.Count == 0) return;

        toRemove.Clear();
        foreach (var collider in overlappingColliders)
        {
            if (collider == null || !collider.gameObject.activeInHierarchy)
            {
                toRemove.Add(collider);
            }
        }

        foreach (var collider in toRemove)
        {
            ForceTriggerExit(collider);
        }
    }

    // Forces a trigger exit event for a given collider.
    private void ForceTriggerExit(Collider other)
    {
        if (overlappingColliders.Contains(other))
        {
            ProcessTowerExit(other);
        }
    }

    // Checks if the colliding object is a valid nearby tower.
    private bool IsValidTowerCollision(Collider other, out GameObject otherTower)
    {
        otherTower = null;

        if (other.transform.parent == null) return false;
        otherTower = other.transform.parent.gameObject;

        if (!otherTower.CompareTag("Selectable")) return false;
        if (transform.parent.gameObject == otherTower) return false;

        if (!otherTower.TryGetComponent<TowerController>(out var _)) return false;

        return true;
    }

    /// <summary>
    /// Sets the detection radius for nearby towers and updates the SphereCollider.
    /// </summary>
    /// <param name="radius">The new detection radius.</param>
    public void SetRadius(float radius)
    {
        range = radius;

        if (sphereCollider == null) return;
        else sphereCollider.radius = range;
    }

    /// <summary>
    /// Called when another collider enters this trigger collider.
    /// If the other collider belongs to a valid tower, it is added to the tracked list,
    /// and the parent TowerController is notified.
    /// </summary>
    /// <param name="other">The other Collider involved in the collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidTowerCollision(other, out var otherTower)) return;

        if (!overlappingColliders.Contains(other))
        {
            TowerElement otherElement = otherTower.GetComponent<TowerController>().TowerElement;
            overlappingColliders.Add(other);
            towerController.HandleNearbyTowerAdded(otherElement);
        }
    }

    /// <summary>
    /// Called when another collider exits this trigger collider.
    /// If the exiting collider was a tracked tower, it is removed from the list,
    /// and the parent TowerController is notified.
    /// </summary>
    /// <param name="other">The other Collider involved in the collision.</param>
    private void OnTriggerExit(Collider other)
    {
        if (overlappingColliders.Contains(other))
        {
            ProcessTowerExit(other);
        }
    }

    // Processes the exit of a tower from the detection range.
    private void ProcessTowerExit(Collider other)
    {
        if (other == null || other.transform == null) return;

        GameObject otherTower = other.transform.parent.gameObject;
        if (otherTower == null) return;

        TowerElement element = otherTower.GetComponent<TowerController>().TowerElement;
        overlappingColliders.Remove(other);
        towerController.HandleNearbyTowerRemoved(element);
    }

    /// <summary>
    /// Clears the list of overlapping colliders when the component is disabled.
    /// </summary>
    private void OnDisable()
    {
        overlappingColliders.Clear();
    }
}