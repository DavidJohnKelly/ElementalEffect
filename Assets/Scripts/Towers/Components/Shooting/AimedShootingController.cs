using UnityEngine;

public class AimedShootingController : GenericShootingController
{
    private AimSystem aimSystem;

    protected override void Awake()
    {
        base.Awake();
        aimSystem = GetComponent<AimSystem>();
    }

    protected override bool Shoot()
    {
        if (!aimSystem.GetAimVector(ProjectileSpeed, out Vector3 aimVector))
        {
            return false;
        }

        return FireProjectile(aimVector);
    }
}