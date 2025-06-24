using UnityEngine;

public class GenericShootingController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float ShotsPerSecond { get; set; } = 2f;
    public float ProjectileSpeed { get; set; } = 200f;
    public float Damage { get; set; } = 10f;
    public bool ShootContinuously { get; set; } = false;

    private float timeBetweenShots;
    private float timeSinceLastShot = 0f;
    private TowerController towerController;

    protected virtual void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
        UpdateShotsPerSecond(ShotsPerSecond);
    }

    private void UpdateShotsPerSecond(float newShotsPerSecond)
    {
        if (newShotsPerSecond == 0)
        {
            Debug.Log("Error in GenericShootingController! newShotsPerSecond is 0!");
            return;
        }
        ShotsPerSecond = newShotsPerSecond;
        timeBetweenShots = 1f / newShotsPerSecond;
    }

    protected void Update()
    {
        if (ShootContinuously)
        {
            Shoot();
            return;
        }

        timeSinceLastShot += Time.deltaTime;

        if (timeSinceLastShot >= timeBetweenShots && towerController.CanShoot)
        {
            bool shot = Shoot();
            if (shot)
            {
                timeSinceLastShot = 0f;
            }
        }
    }
    protected virtual bool Shoot()
    {
        Debug.Log("Shooting not implemented");
        return false;
    }

    protected virtual bool FireProjectile(Vector3 aimVector)
    {
        if (!projectilePrefab)
        {
            Debug.LogError("Projectile Prefab is not assigned!");
            return false;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.LookRotation(aimVector)
        );

        // Set the projectile's direction and speed
        if (projectile.TryGetComponent<ProjectileController>(out var projectileController))
        {
            projectileController.Direction = aimVector;
            projectileController.Speed = ProjectileSpeed;
            projectileController.Distance = towerController.Radius + 2f;  // Some padding as range is spherical
            projectileController.Damage = Damage;
            return true;
        }
        return false;
    }
}