using UnityEngine;

/// <summary>
/// Controls the elemental synergy effects for a tower, modifying its damage and range based on nearby towers.
/// </summary>
public class ElementalTowerController : MonoBehaviour
{
    /// <summary>
    /// Settings related to the visual display of synergy effects.
    /// </summary>
    [Header("Synergy Settings")]
    /// <summary>
    /// The string to display when a positive synergy effect is active.
    /// </summary>
    [Tooltip("The string to display when a positive synergy effect is active.")]
    [SerializeField] private string positiveSynergyString = "Boosted";
    /// <summary>
    /// The color of the text displayed for a positive synergy effect.
    /// </summary>
    [Tooltip("The color of the text displayed for a positive synergy effect.")]
    [SerializeField] private Color positiveSynergyColour = Color.green;
    /// <summary>
    /// The string to display when a negative synergy effect is active.
    /// </summary>
    [Tooltip("The string to display when a negative synergy effect is active.")]
    [SerializeField] private string negativeSynergyString = "Hindered";
    /// <summary>
    /// The color of the text displayed for a negative synergy effect.
    /// </summary>
    [Tooltip("The color of the text displayed for a negative synergy effect.")]
    [SerializeField] private Color negativeSynergyColour = Color.red;
    /// <summary>
    /// The font size of the synergy effect text.
    /// </summary>
    [Tooltip("The font size of the synergy effect text.")]
    [SerializeField] private float synergyFontSize = 24f;
    /// <summary>
    /// The vertical offset of the synergy effect text from the tower's position.
    /// </summary>
    [Tooltip("The vertical offset of the synergy effect text from the tower's position.")]
    [SerializeField] private float synergyHeightOffset = 3f;

    /// <summary>
    /// Settings related to the modifiers applied by elemental synergies.
    /// </summary>
    [Header("Modifier Settings")]
    /// <summary>
    /// The percentage amount by which the tower's radius is modified per synergy point.
    /// </summary>
    [Tooltip("The percentage amount by which the tower's radius is modified per synergy point.")]
    public float RadiusModifier = 0.08f;
    /// <summary>
    /// The percentage amount by which the tower's damage is modified per synergy point.
    /// </summary>
    [Tooltip("The percentage amount by which the tower's damage is modified per synergy point.")]
    public float DamageModifier = 0.05f;

    // Multiplier for water damage based on synergies.
    private float WaterDamageModifier = 1.0f;
    // Multiplier for earth damage based on synergies.
    private float EarthDamageModifier = 1.0f;
    // Multiplier for fire damage based on synergies.
    private float FireDamageModifier = 1.0f;
    // Multiplier for air damage based on synergies.
    private float AirDamageModifier = 1.0f;
    // Multiplier for ice damage based on synergies.
    private float IceDamageModifier = 1.0f;
    // Multiplier for electric damage based on synergies.
    private float ElectricDamageModifier = 1.0f;

    // Reference to the parent TowerController.
    private TowerController towerController;
    // The elemental type of this tower.
    private TowerElement towerElement;

    /// <summary>
    /// Retrieves the TowerController component and the tower's element.
    /// </summary>
    private void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
        towerElement = towerController.TowerElement;
    }

    /// <summary>
    /// Gets the current elemental damage modifier for this tower based on active synergies.
    /// </summary>
    /// <returns>The current elemental damage modifier.</returns>
    public float GetElementalModifier()
    {
        return towerElement switch
        {
            TowerElement.Air => AirDamageModifier,
            TowerElement.Earth => EarthDamageModifier,
            TowerElement.Electric => ElectricDamageModifier,
            TowerElement.Fire => FireDamageModifier,
            TowerElement.Ice => IceDamageModifier,
            TowerElement.Water => WaterDamageModifier,
            _ => AirDamageModifier,
        };
    }

    /// <summary>
    /// Modifies the tower's attributes (damage and/or range) based on the element of a nearby tower.
    /// </summary>
    /// <param name="modifyingElement">The element of the tower causing the modification.</param>
    /// <param name="sign">The sign of the modification (+1 for positive, -1 for negative).</param>
    public void ModifyAttributes(TowerElement modifyingElement, float sign)
    {
        sign = Mathf.Sign(sign);

        switch (modifyingElement)
        {
            case TowerElement.Air:
                HandleAir(sign);
                break;
            case TowerElement.Earth:
                HandleEarth(sign);
                break;
            case TowerElement.Electric:
                HandleElectricity(sign);
                break;
            case TowerElement.Fire:
                HandleFire(sign);
                break;
            case TowerElement.Ice:
                HandleIce(sign);
                break;
            case TowerElement.Water:
                HandleWater(sign);
                break;
        }

        BalanceDamage();
    }

    // Ensures that the damage modifiers stay within a reasonable range.
    private void BalanceDamage()
    {
        if (WaterDamageModifier < 0)
        {
            WaterDamageModifier = 0;
        }
        else if (WaterDamageModifier > 4)
        {
            WaterDamageModifier = 4;
        }
        if (EarthDamageModifier < 0)
        {
            EarthDamageModifier = 0;
        }
        else if (EarthDamageModifier > 4)
        {
            EarthDamageModifier = 4;
        }
        if (FireDamageModifier < 0)
        {
            FireDamageModifier = 0;
        }
        else if (FireDamageModifier > 4)
        {
            FireDamageModifier = 4;
        }
        if (AirDamageModifier < 0)
        {
            AirDamageModifier = 0;
        }
        else if (AirDamageModifier > 4)
        {
            AirDamageModifier = 4;
        }
        if (IceDamageModifier < 0)
        {
            IceDamageModifier = 0;
        }
        else if (IceDamageModifier > 4)
        {
            IceDamageModifier = 4;
        }
        if (ElectricDamageModifier < 0)
        {
            ElectricDamageModifier = 0;
        }
        else if (ElectricDamageModifier > 4)
        {
            ElectricDamageModifier = 4;
        }
    }

    // Adjusts the tower's range based on the provided sign and the RadiusModifier.
    private void AdjustRadius(float sign)
    {
        float modifier = sign * RadiusModifier;
        towerController.HandleRangeModification(modifier);
    }

    // Displays visual feedback (text) for synergy effects.
    private void DisplaySynergy(int synergyCount)
    {
        Vector3 heightOffset = new(0, synergyHeightOffset, 0);
        Vector3 spawnPosition = transform.position + heightOffset;
        if (synergyCount < 0)
        {
            TextFactory.Instance.CreateText(negativeSynergyString, negativeSynergyColour, synergyFontSize, spawnPosition);
        }
        else if (synergyCount > 0)
        {
            TextFactory.Instance.CreateText(positiveSynergyString, positiveSynergyColour, synergyFontSize, spawnPosition);
        }
    }

    // Handles the synergy effects when an Air tower is nearby.
    private void HandleAir(float sign)
    {
        int synergyCount = 0;

        if (towerElement == TowerElement.Water)
        {
            WaterDamageModifier -= sign * AirDamageModifier * DamageModifier;
            synergyCount -= 1;
        }
        else if (towerElement == TowerElement.Earth)
        {
            AdjustRadius(sign);
            EarthDamageModifier += sign * AirDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Fire)
        {
            AdjustRadius(sign);
            FireDamageModifier += sign * AirDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Air)
        {
            return;
        }
        else if (towerElement == TowerElement.Ice)
        {
            AdjustRadius(sign);
            IceDamageModifier += sign * AirDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Electric)
        {
            return;
        }

        DisplaySynergy(synergyCount);
    }

    // Handles the synergy effects when an Earth tower is nearby.
    private void HandleEarth(float sign)
    {
        int synergyCount = 0;

        if (towerElement == TowerElement.Water)
        {
            return;
        }
        else if (towerElement == TowerElement.Earth)
        {
            return;
        }
        else if (towerElement == TowerElement.Fire)
        {
            FireDamageModifier += sign * EarthDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Air)
        {
            return;
        }
        else if (towerElement == TowerElement.Ice)
        {
            IceDamageModifier += sign * EarthDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Electric)
        {
            return;
        }

        DisplaySynergy(synergyCount);
    }

    // Handles the synergy effects when an Electric tower is nearby.
    private void HandleElectricity(float sign)
    {
        int synergyCount = 0;

        if (towerElement == TowerElement.Water)
        {
            WaterDamageModifier += sign * ElectricDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Earth)
        {
            EarthDamageModifier -= sign * ElectricDamageModifier * DamageModifier;
            synergyCount -= 1;
        }
        else if (towerElement == TowerElement.Fire)
        {
            return;
        }
        else if (towerElement == TowerElement.Air)
        {
            return;
        }
        else if (towerElement == TowerElement.Ice)
        {
            IceDamageModifier -= sign * ElectricDamageModifier * DamageModifier;
            synergyCount -= 1;
        }
        else if (towerElement == TowerElement.Electric)
        {
            return;
        }

        DisplaySynergy(synergyCount);
    }

    // Handles the synergy effects when a Fire tower is nearby.
    private void HandleFire(float sign)
    {
        int synergyCount = 0;

        if (towerElement == TowerElement.Water)
        {
            WaterDamageModifier -= sign * FireDamageModifier * DamageModifier;
            synergyCount -= 1;
        }
        else if (towerElement == TowerElement.Earth)
        {
            WaterDamageModifier += sign * FireDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Fire)
        {
            return;
        }
        else if (towerElement == TowerElement.Air)
        {
            AirDamageModifier += sign * FireDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Ice)
        {
            IceDamageModifier -= sign * FireDamageModifier * DamageModifier;
            synergyCount -= 1;
        }
        else if (towerElement == TowerElement.Electric)
        {
            return;
        }

        DisplaySynergy(synergyCount);
    }

    // Handles the synergy effects when an Ice tower is nearby.
    private void HandleIce(float sign)
    {
        int synergyCount = 0;

        if (towerElement == TowerElement.Water)
        {
            WaterDamageModifier += sign * IceDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Earth)
        {
            EarthDamageModifier += sign * IceDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Fire)
        {
            FireDamageModifier -= sign * IceDamageModifier * DamageModifier;
            synergyCount -= 1;
        }
        else if (towerElement == TowerElement.Air)
        {
            AirDamageModifier += sign * IceDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Ice)
        {
            return;
        }
        else if (towerElement == TowerElement.Electric)
        {
            ElectricDamageModifier -= sign * IceDamageModifier * DamageModifier;
            synergyCount -= 1;
        }

        DisplaySynergy(synergyCount);
    }

    // Handles the synergy effects when a Water tower is nearby.
    private void HandleWater(float sign)
    {
        int synergyCount = 0;

        if (towerElement == TowerElement.Water)
        {
            return;
        }
        else if (towerElement == TowerElement.Earth)
        {
            return;
        }
        else if (towerElement == TowerElement.Fire)
        {
            FireDamageModifier -= sign * WaterDamageModifier * DamageModifier;
            synergyCount -= 1;
        }
        else if (towerElement == TowerElement.Air)
        {
            AirDamageModifier -= sign * WaterDamageModifier * DamageModifier;
            synergyCount -= 1;
        }
        else if (towerElement == TowerElement.Ice)
        {
            IceDamageModifier += sign * WaterDamageModifier * DamageModifier;
            synergyCount += 1;
        }
        else if (towerElement == TowerElement.Electric)
        {
            ElectricDamageModifier += sign * WaterDamageModifier * DamageModifier;
            synergyCount += 1;
        }

        DisplaySynergy(synergyCount);
    }
}