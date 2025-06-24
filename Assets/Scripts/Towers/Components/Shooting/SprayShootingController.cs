using UnityEngine;

public class SprayShootingController : GenericShootingController
{
    [SerializeField] private int projectilesPerLayer = 8;
    [SerializeField] private int numberOfLayers = 3;

    protected override bool Shoot()
    {
        if (!projectilePrefab)
        {
            Debug.LogError("Projectile Prefab is not assigned!");
            return false;
        }

        for (int i = 0; i < numberOfLayers; i++)
        {
            for (int j = 0; j < projectilesPerLayer; j++)
            {
                float angle = (360f / projectilesPerLayer) * j;
                float layerAngle = (360f / numberOfLayers) * i;
                Vector3 aimVector = Quaternion.Euler(0, layerAngle, 0) * Quaternion.Euler(0, angle, 0) * Vector3.forward;

                FireProjectile(aimVector);
            }
        }

        return true;
    }
}