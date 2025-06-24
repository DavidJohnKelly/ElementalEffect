using UnityEngine;

/// <summary>
/// Handles elemental resistance, damage buildup, and triggering of elemental effects on the enemy.
/// </summary>
public class ElementResistanceController : MonoBehaviour
{
    [Header("Resistance Settings")]
    [Tooltip("Resistance threshold for air damage effects")]
    [SerializeField] private float airResistance;
    private float airDamage;

    [Tooltip("Resistance threshold for fire damage effects")]
    [SerializeField] private float fireResistance;
    private float fireDamage;

    [Tooltip("Resistance threshold for water damage effects")]
    [SerializeField] private float waterResistance;
    private float waterDamage;

    [Tooltip("Resistance threshold for earth damage effects")]
    [SerializeField] private float earthResistance;
    private float earthDamage;

    [Tooltip("Resistance threshold for electricity damage effects")]
    [SerializeField] private float electricityResistance;
    private float electricityDamage;

    [Tooltip("Resistance threshold for ice damage effects")]
    [SerializeField] private float iceResistance;
    private float iceDamage;

    [Header("Effect Settings")]
    [Tooltip("How many times per second elemental effects can apply")]
    [SerializeField] private float elementalEffectsPerSecond = 10f;
    private float elementalTimeBuffer;

    [Tooltip("Rate at which elemental damage decays over time")]
    [SerializeField] private float elementalDecayPerSecond = 2f;

    [Tooltip("Penalty factor when multiple elements interfere")]
    [SerializeField] private float elementalInteferenceDivisoryPenalty = 2f;

    private EnemyController enemyController;
    private float secondsSinceLastElementalEffect = 0;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
        elementalTimeBuffer = 1f / elementalEffectsPerSecond;
    }

    private void Update()
    {
        DecreaseElementalDamage();

        secondsSinceLastElementalEffect += Time.deltaTime;

        if (secondsSinceLastElementalEffect < elementalTimeBuffer) return;
        else secondsSinceLastElementalEffect = 0;

        if (ShouldApplyElementalEffect(airDamage, airResistance))
        {
            HandleAirEffect();
        }

        if (ShouldApplyElementalEffect(earthDamage, earthResistance))
        {
            HandleEarthEffect();
        }

        if (ShouldApplyElementalEffect(electricityDamage, electricityResistance))
        {
            HandleElectricEffect();
        }

        if (ShouldApplyElementalEffect(fireDamage, fireResistance))
        {
            HandleFireEffect();
        }

        if (ShouldApplyElementalEffect(iceDamage, iceResistance))
        {
            HandleIceEffect();
        }

        if (ShouldApplyElementalEffect(waterDamage, waterResistance))
        {
            HandleWaterEffect();
        }
    }

    /// <summary>
    /// Gradually reduces stored elemental damage over time.
    /// </summary>
    private void DecreaseElementalDamage()
    {
        float decayAmount = elementalDecayPerSecond * Time.deltaTime;

        airDamage = Mathf.Max(0, airDamage - decayAmount);
        earthDamage = Mathf.Max(0, earthDamage - decayAmount);
        electricityDamage = Mathf.Max(0, electricityDamage - decayAmount);
        fireDamage = Mathf.Max(0, fireDamage - decayAmount);
        iceDamage = Mathf.Max(0, iceDamage - decayAmount);
        waterDamage = Mathf.Max(0, waterDamage - decayAmount);
    }

    /// <summary>
    /// Adds elemental damage of the given type to the corresponding pool.
    /// </summary>
    public void TakeElementalDamage(TowerElement element, float damage)
    {
        switch (element)
        {
            case TowerElement.Air:
                airDamage += damage;
                break;
            case TowerElement.Earth:
                earthDamage += damage;
                break;
            case TowerElement.Electric:
                electricityDamage += damage;
                break;
            case TowerElement.Fire:
                fireDamage += damage;
                break;
            case TowerElement.Ice:
                iceDamage += damage;
                break;
            case TowerElement.Water:
                waterDamage += damage;
                break;
        }
    }

    /// <summary>
    /// Determines if the stored damage exceeds resistance and effect should apply.
    /// </summary>
    private bool ShouldApplyElementalEffect(float damage, float resistance)
    {
        return damage > resistance;
    }

    private void HandleAirEffect()
    {
        enemyController.ModifySpeedForTime(-5f, 1.5f);

        if (ShouldApplyElementalEffect(fireDamage, fireResistance))
        {
            enemyController.HandlePhysicalDamage(10f);
            fireDamage = 0f;
        }

        waterDamage /= elementalInteferenceDivisoryPenalty;
        airDamage /= elementalInteferenceDivisoryPenalty;

        ParticleFactory.Instance.SpawnParticleSystem(TowerElement.Air, transform, enemyController.GetEnemyRadius() + 1);
    }

    private void HandleEarthEffect()
    {
        enemyController.HandlePhysicalDamage(15f);

        if (ShouldApplyElementalEffect(iceDamage, iceResistance))
        {
            enemyController.HandlePhysicalDamage(15f);
            iceDamage = 0;
        }
        if (ShouldApplyElementalEffect(fireDamage, fireResistance))
        {
            enemyController.HandlePhysicalDamage(15f);
            fireDamage = 0;
        }

        electricityDamage /= elementalInteferenceDivisoryPenalty;
        earthDamage /= elementalInteferenceDivisoryPenalty;

        ParticleFactory.Instance.SpawnParticleSystem(TowerElement.Earth, transform, enemyController.GetEnemyRadius() + 1);
    }

    private void HandleElectricEffect()
    {
        enemyController.HandlePhysicalDamage(10f);
        enemyController.ModifySpeedForTime(0, 1.5f);

        if (ShouldApplyElementalEffect(waterDamage, waterResistance))
        {
            enemyController.HandlePhysicalDamage(10f);
            waterDamage = 0f;
        }

        electricityDamage /= elementalInteferenceDivisoryPenalty;

        ParticleFactory.Instance.SpawnParticleSystem(TowerElement.Electric, transform, enemyController.GetEnemyRadius() + 1);
    }

    private void HandleFireEffect()
    {
        enemyController.HandlePhysicalDamage(15f);

        if (ShouldApplyElementalEffect(earthDamage, earthResistance))
        {
            enemyController.HandlePhysicalDamage(15f);
            earthDamage = 0f;
        }

        iceDamage /= elementalInteferenceDivisoryPenalty;
        waterDamage /= elementalInteferenceDivisoryPenalty;
        fireDamage /= elementalInteferenceDivisoryPenalty;

        ParticleFactory.Instance.SpawnParticleSystem(TowerElement.Fire, transform, enemyController.GetEnemyRadius() + 1);
    }

    private void HandleIceEffect()
    {
        enemyController.HandlePhysicalDamage(0.1f);
        enemyController.ModifySpeedForTime(enemyController.GetEnemySpeed() / 5f, 3f);

        float maxWater = Mathf.Max(0, waterResistance - 1);
        waterDamage = Mathf.Min(maxWater, 1.5f * waterDamage);

        iceDamage /= elementalInteferenceDivisoryPenalty;
        fireDamage /= elementalInteferenceDivisoryPenalty;
        electricityDamage /= elementalInteferenceDivisoryPenalty;

        ParticleFactory.Instance.SpawnParticleSystem(TowerElement.Ice, transform, enemyController.GetEnemyRadius() + 1);
    }

    private void HandleWaterEffect()
    {
        if (ShouldApplyElementalEffect(iceDamage, iceResistance))
        {
            enemyController.ModifySpeedForTime(0, 2.5f);
            enemyController.HandlePhysicalDamage(30);
        }

        float maxIce = Mathf.Max(0, iceResistance - 1);
        iceDamage = Mathf.Min(maxIce, 1.5f * iceDamage);

        fireDamage /= elementalInteferenceDivisoryPenalty;
        airDamage /= elementalInteferenceDivisoryPenalty;
        waterDamage /= elementalInteferenceDivisoryPenalty;

        ParticleFactory.Instance.SpawnParticleSystem(TowerElement.Water, transform, enemyController.GetEnemyRadius() + 1);
    }
}