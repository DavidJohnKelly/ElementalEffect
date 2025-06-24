using UnityEngine;

/// <summary>
/// A factory class responsible for providing access to different colors based on an index or a TowerElement.
/// This class follows the Singleton pattern for easy access.
/// </summary>
public class ColourFactory : MonoBehaviour
{
    /// <summary>
    /// The color associated with the Air element.
    /// </summary>
    [SerializeField] private Color AirColour;
    /// <summary>
    /// The color associated with the Earth element.
    /// </summary>
    [SerializeField] private Color EarthColour;
    /// <summary>
    /// The color associated with the Electric element.
    /// </summary>
    [SerializeField] private Color ElectricColour;
    /// <summary>
    /// The color associated with the Fire element.
    /// </summary>
    [SerializeField] private Color FireColour;
    /// <summary>
    /// The color associated with the Ice element.
    /// </summary>
    [SerializeField] private Color IceColour;
    /// <summary>
    /// The color associated with the Water element.
    /// </summary>
    [SerializeField] private Color WaterColour;

    /// <summary>
    /// The singleton instance of the ColourFactory.
    /// </summary>
    public static ColourFactory Instance;

    /// <summary>
    /// Sets up the Singleton instance of the ColourFactory.
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of ColourFactory detected.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Gets a color based on the provided index.
    /// </summary>
    /// <param name="index">The index of the color to retrieve (0: Air, 1: Earth, 2: Electric, 3: Fire, 4: Ice, 5: Water).</param>
    /// <returns>The Color associated with the given index.</returns>
    public Color GetColour(int index)
    {
        if (index == 0) return AirColour;
        else if (index == 1) return EarthColour;
        else if (index == 2) return ElectricColour;
        else if (index == 3) return FireColour;
        else if (index == 4) return IceColour;
        else if (index == 5) return WaterColour;

        Debug.LogError("Error in ColourFactory! Index " + index + " is invalid!");
        return AirColour;
    }

    /// <summary>
    /// Gets a color based on the provided TowerElement.
    /// </summary>
    /// <param name="element">The TowerElement to retrieve the associated color for.</param>
    /// <returns>The Color associated with the given TowerElement.</returns>
    public Color GetColour(TowerElement element)
    {
        switch (element)
        {
            case TowerElement.Air:
                return AirColour;
            case TowerElement.Earth:
                return EarthColour;
            case TowerElement.Electric:
                return ElectricColour;
            case TowerElement.Fire:
                return FireColour;
            case TowerElement.Ice:
                return IceColour;
            case TowerElement.Water:
                return WaterColour;
        }

        Debug.LogError("Error in ColourFactory! Element " + element + " is invalid!");
        return AirColour;
    }
}