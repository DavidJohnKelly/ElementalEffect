using System.Collections.Generic;
using UnityEngine;

public class PlacementValidator : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    private readonly HashSet<Collider> overlappingColliders = new();

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

    private void OnTriggerExit(Collider other)
    {
        if (overlappingColliders.Contains(other))
        {
            overlappingColliders.Remove(other);
        }
    }

    public bool CheckPlacementValid()
    {
        return overlappingColliders.Count == 0;
    }

    private void OnDisable()
    {
        overlappingColliders.Clear();
    }
}