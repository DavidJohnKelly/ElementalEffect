using System.Collections.Generic;
using UnityEngine;

public class NearbyTowerDetector : MonoBehaviour
{
    private TowerController towerController;
    private SphereCollider sphereCollider;
    private float range;
    private readonly HashSet<Collider> overlappingColliders = new();

    void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    public void SetRadius(float radius)
    {
        range = radius;

        if (sphereCollider == null) return;
        else sphereCollider.radius = range;
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;

        if (otherObject.transform.parent == null) return;

        GameObject otherTower = otherObject.transform.parent.gameObject;
        GameObject thisTower = transform.parent.gameObject;
        if (otherTower.CompareTag("Selectable"))
        {
            if (thisTower == otherTower) return;

            if (!overlappingColliders.Contains(other))
            {
                overlappingColliders.Add(other);
                towerController.HandleNearbyTowerAdded();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (overlappingColliders.Contains(other))
        {
            overlappingColliders.Remove(other);
            towerController.HandleNearbyTowerRemoved();
        }
    }
}