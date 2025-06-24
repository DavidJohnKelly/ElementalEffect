using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses trigger colliders to determine if an object's placement is valid by checking for overlaps with specific layers.
/// </summary>
public class PlacementValidator : MonoBehaviour
{
    /// <summary>
    /// The LayerMask defining the layers that will cause placement to be invalid.
    /// </summary>
    [Tooltip("The LayerMask defining the layers that will cause placement to be invalid.")]
    [SerializeField] private LayerMask targetLayer;
    // Stores the colliders that are currently overlapping with this object's trigger.
    private readonly HashSet<Collider> overlappingColliders = new();
    // A temporary list used to store colliders that need to be removed from the overlappingColliders.
    private readonly List<Collider> toRemove = new();

    /// <summary>
    /// Checks for and removes any invalid or inactive colliders from the overlapping colliders list.
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

    // Forces a trigger exit event for the given collider.
    private void ForceTriggerExit(Collider other)
    {
        if (overlappingColliders.Contains(other))
        {
            overlappingColliders.Remove(other);
        }
    }

    /// <summary>
    /// Called when another collider enters this object's trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;

        if (gameObject == otherObject) return;

        if ((targetLayer & (1 << otherObject.layer)) != 0)
        {
            if (!overlappingColliders.Contains(other))
            {
                overlappingColliders.Add(other);
            }
        }
    }

    /// <summary>
    /// Called when another collider exits this object's trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerExit(Collider other)
    {
        ForceTriggerExit(other);
    }

    /// <summary>
    /// Checks if the current placement is valid (no overlaps with target layers).
    /// </summary>
    /// <returns>True if there are no overlapping colliders on the target layer, false otherwise.</returns>
    public bool CheckPlacementValid()
    {
        return overlappingColliders.Count == 0;
    }

    /// <summary>
    /// Clears the list of overlapping colliders when this object is disabled.
    /// </summary>
    private void OnDisable()
    {
        overlappingColliders.Clear();
    }
}