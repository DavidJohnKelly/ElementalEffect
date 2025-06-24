using UnityEngine;

public class ElementalTowerController : MonoBehaviour
{
    public enum Element { Water, Earth, Fire, Air, Ice, Electricity }

    [SerializeField] private Element element;
    [SerializeField] private float BaseDamage = 10f;
    [SerializeField] private float ElementalDamageModifier = 0.5f;

    public float WaterDamage { get; private set; }
    public float EarthDamage { get; private set; }
    public float FireDamage { get; private set; }
    public float AirDamage { get; private set; }
    public float IceDamage { get; private set; }
    public float ElectricityDamage { get; private set; }

    private TowerController towerController;

    void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
    }

    void Start()
    {
        if (element == Element.Water)
        {
            WaterDamage = BaseDamage * ElementalDamageModifier;
        }
        else if (element == Element.Earth)
        {
            EarthDamage = BaseDamage * ElementalDamageModifier;
        }
        else if (element == Element.Fire)
        {
            FireDamage = BaseDamage * ElementalDamageModifier;
        }
        else if (element == Element.Air)
        {
            AirDamage = BaseDamage * ElementalDamageModifier;
        }
        else if (element == Element.Ice)
        {
            IceDamage = BaseDamage * ElementalDamageModifier;
        }
        else if (element == Element.Electricity)
        {
            ElectricityDamage = BaseDamage * ElementalDamageModifier;
        }
    }


    public void IncreaseAttributes()
    {
        ModifyAttributes(1);
    }

    public void DecreaseAttributes()
    {
        ModifyAttributes(-1);
    }

    private void ModifyAttributes(float sign)
    {
        sign = Mathf.Sign(sign);

        if (element == Element.Water)
        {
            HandleWater(sign);
        }
        else if (element == Element.Earth)
        {
            HandleEarth(sign);
        }
        else if (element == Element.Fire)
        {
            HandleFire(sign);
        }
        else if (element == Element.Air)
        {
            HandleAir(sign);
        }
        else if (element == Element.Ice)
        {
            HandleIce(sign);
        }
        else if (element == Element.Electricity)
        {
            HandleElectricity(sign);
        }
        else
        {
            Debug.LogError("Element not found: " + element);
        }

        BalanceDamage();
    }

    private void BalanceDamage()
    {
        if (WaterDamage < 0)
        {
            WaterDamage = 0;
        }
        else if (WaterDamage > 100)
        {
            WaterDamage = 100;
        }
        if (EarthDamage < 0)
        {
            EarthDamage = 0;
        }
        else if (EarthDamage > 100)
        {
            EarthDamage = 100;
        }
        if (FireDamage < 0)
        {
            FireDamage = 0;
        }
        else if (FireDamage > 100)
        {
            FireDamage = 100;
        }
        if (AirDamage < 0)
        {
            AirDamage = 0;
        }
        else if (AirDamage > 100)
        {
            AirDamage = 100;
        }
        if (IceDamage < 0)
        {
            IceDamage = 0;
        }
        else if (IceDamage > 100)
        {
            IceDamage = 100;
        }
        if (ElectricityDamage < 0)
        {
            ElectricityDamage = 0;
        }
        else if (ElectricityDamage > 100)
        {
            ElectricityDamage = 100;
        }
    }

    private readonly float RADIUS_MODIFIER = 0.04f;
    private void AdjustRadius(float damage, float sign)
    {
        float radius_increase = sign * damage * RADIUS_MODIFIER;
        float radius = towerController.Radius;
        towerController.HandleRadiusUpdate(radius + radius_increase);
    }

    private readonly float DAMAGE_MODIFIER = 0.01f;
    private void HandleWater(float sign)
    {
        if (element == Element.Water)
        {
            return;
        }
        else if (element == Element.Earth)
        {
            return;
        }
        else if (element == Element.Fire)
        {
            FireDamage -= sign * WaterDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Air)
        {
            AirDamage -= sign * WaterDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Ice)
        {
            IceDamage += sign * WaterDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Electricity)
        {
            ElectricityDamage += sign * WaterDamage * DAMAGE_MODIFIER;
        }
    }

    private void HandleEarth(float sign)
    {
        if (element == Element.Water)
        {
            return;
        }
        else if (element == Element.Earth)
        {
            return;
        }
        else if (element == Element.Fire)
        {
            FireDamage += sign * EarthDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Air)
        {
            return;
        }
        else if (element == Element.Ice)
        {
            IceDamage += sign * EarthDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Electricity)
        {
            return;
        }
    }

    private void HandleFire(float sign)
    {
        if (element == Element.Water)
        {
            WaterDamage -= sign * FireDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Earth)
        {
            WaterDamage += sign * FireDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Fire)
        {
            return;
        }
        else if (element == Element.Air)
        {
            AirDamage += sign * FireDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Ice)
        {
            IceDamage -= sign * FireDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Electricity)
        {
            return;
        }
    }

    private void HandleAir(float sign)
    {
        if (element == Element.Water)
        {
            WaterDamage -= sign * AirDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Earth)
        {
            AdjustRadius(sign, AirDamage);
            EarthDamage += sign * AirDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Fire)
        {
            FireDamage += sign * AirDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Air)
        {
            return;
        }
        else if (element == Element.Ice)
        {
            AdjustRadius(sign, AirDamage);
            IceDamage += sign * AirDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Electricity)
        {
            return;
        }
    }

    private void HandleIce(float sign)
    {
        if (element == Element.Water)
        {
            WaterDamage += sign * IceDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Earth)
        {
            EarthDamage += sign * IceDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Fire)
        {
            FireDamage -= sign * IceDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Air)
        {
            AirDamage += sign * IceDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Ice)
        {
            return;
        }
        else if (element == Element.Electricity)
        {
            ElectricityDamage -= sign * IceDamage * DAMAGE_MODIFIER;
        }
    }

    private void HandleElectricity(float sign)
    {
        if (element == Element.Water)
        {
            WaterDamage += sign * ElectricityDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Earth)
        {
            EarthDamage -= sign * ElectricityDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Fire)
        {
            return;
        }
        else if (element == Element.Air)
        {
            return;
        }
        else if (element == Element.Ice)
        {
            IceDamage -= sign * ElectricityDamage * DAMAGE_MODIFIER;
        }
        else if (element == Element.Electricity)
        {
            return;
        }
    }
}