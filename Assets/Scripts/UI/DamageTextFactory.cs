using UnityEngine;

/// <summary>
/// A factory class responsible for creating and displaying damage text popups for entities.
/// This class follows the Singleton pattern for easy access.
/// </summary>
public class DamageTextFactory : MonoBehaviour
{
    /// <summary>
    /// The font size for elemental damage text.
    /// </summary>
    [Header("Damage Info Settings")]
    [SerializeField] private float elementalDamageFontSize = 18f;
    /// <summary>
    /// The font size for physical damage text.
    /// </summary>
    [SerializeField] private float physicalDamageFontSize = 12f;
    /// <summary>
    /// The color of the physical damage text.
    /// </summary>
    [SerializeField] private Color physicalDamageTextColour = Color.black;
    /// <summary>
    /// The threshold above which damage values should be rounded to the nearest whole number.
    /// </summary>
    [SerializeField] private float damageRoundingThreshold = 4f;

    /// <summary>
    /// The range of random offset applied to the damage text position.
    /// </summary>
    [Header("Damage Info Offsets")]
    [SerializeField] private float textOffsetRange = 0.5f;
    /// <summary>
    /// The fixed offset applied to the physical damage text position relative to the elemental damage text.
    /// </summary>
    [SerializeField] private Vector3 physicalDamageTextOffset = new(0.35f, 0.35f, -0.5f);

    /// <summary>
    /// The singleton instance of the DamageTextFactory.
    /// </summary>
    public static DamageTextFactory Instance;

    /// <summary>
    /// Sets up the Singleton instance of the DamageTextFactory.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of DamageTextFactory detected.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Creates and displays damage text at the specified position, differentiating between elemental and physical damage.
    /// </summary>
    /// <param name="position">The world position where the damage text should appear.</param>
    /// <param name="element">The elemental type of the damage.</param>
    /// <param name="elementalDamage">The amount of elemental damage dealt.</param>
    /// <param name="physicalDamagePercent">The percentage of total damage that is physical damage (0 to 1).</param>
    public void CreateDamageText(Vector3 position, TowerElement element, float elementalDamage, float physicalDamagePercent)
    {
        float offsetX = Random.Range(-textOffsetRange, textOffsetRange);
        float offsetY = Random.Range(-textOffsetRange, textOffsetRange);

        //Vector3 spawnPosition = position + new Vector3(offsetX, offsetY, 0f);
        //if (spawnPosition.y < 0.1f)
        //{
        //    spawnPosition.y = 0.1f;
        //}

        Color elementalTextColour = ColourFactory.Instance.GetColour(element);
        TextFactory.Instance.CreateText(elementalDamage, elementalTextColour, elementalDamageFontSize, position, elementalDamage > damageRoundingThreshold);
        if (physicalDamagePercent > 0)
        {
            float physicalDamage = elementalDamage * physicalDamagePercent;
            TextFactory.Instance.CreateText(physicalDamage, physicalDamageTextColour, physicalDamageFontSize, position + physicalDamageTextOffset, physicalDamage > damageRoundingThreshold);
        }
    }
}