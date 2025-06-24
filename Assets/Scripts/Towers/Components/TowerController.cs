using UnityEngine;

/// <summary>
/// Acts as the central controller for a tower, managing its core properties and coordinating interactions between its components.
/// This script serves as a communication hub, preventing direct dependencies between sub-components.
/// </summary>
public class TowerController : MonoBehaviour
{
    /// <summary>
    /// The type of the tower (e.g., Aiming, AreaOfAffect, Spray).
    /// </summary>
    [Tooltip("The type of the tower.")]
    public TowerType TowerType;
    /// <summary>
    /// The elemental type of the tower (e.g., Water, Fire, Air).
    /// </summary>
    [Tooltip("The elemental type of the tower.")]
    public TowerElement TowerElement;
    /// <summary>
    /// The current level of the tower.
    /// </summary>
    [Tooltip("The current level of the tower.")]
    public TowerLevel TowerLevel;
    /// <summary>
    /// The cost to initially build this tower.
    /// </summary>
    [Tooltip("The cost to initially build this tower.")]
    public float TowerCost;
    /// <summary>
    /// The cost to upgrade this tower to the next level.
    /// </summary>
    [Tooltip("The cost to upgrade this tower to the next level.")]
    public float NextTowerCost;

    /// <summary>
    /// Gets whether the tower can currently shoot based on the game state.
    /// </summary>
    public bool CanShoot { get => gameController.InFPSStage; }
    /// <summary>
    /// Gets the current range of the tower.
    /// </summary>
    public float Range { get => rangeController.GetRangeRadius(); }
    /// <summary>
    /// Gets the PlaceableObject component attached to this tower's base.
    /// </summary>
    public PlaceableObject TowerBase { get => placeableObject; }

    // Reference to the GameController singleton.
    private GameController gameController;
    // Reference to the PlaceableObject component for handling selection and placement.
    private PlaceableObject placeableObject;
    // Reference to the UpgradeManager component for handling tower upgrades.
    private UpgradeManager upgradeManager;
    // Reference to the RangeController component for managing the tower's range.
    private RangeController rangeController;
    // Reference to the ElementalTowerController component for managing elemental synergies.
    private ElementalTowerController elementalTowerController;
    // Reference to the NearbyTowerDetector component for detecting nearby towers.
    private NearbyTowerDetector nearbyTowerDetector;

    /// <summary>
    /// Retrieves references to all necessary sub-components.
    /// </summary>
    private void Awake()
    {
        gameController = GameController.Instance;
        placeableObject = GetComponent<PlaceableObject>();
        upgradeManager = GetComponentInChildren<UpgradeManager>();
        rangeController = GetComponentInChildren<RangeController>();
        elementalTowerController = GetComponentInChildren<ElementalTowerController>();
        nearbyTowerDetector = GetComponentInChildren<NearbyTowerDetector>();
    }

    /// <summary>
    /// Subscribes to the OnSelected and OnDeselected events of the PlaceableObject component.
    /// </summary>
    private void OnEnable()
    {
        if (placeableObject != null)
        {
            placeableObject.OnSelected += HandleSelected;
            placeableObject.OnDeselected += HandleDeselected;
        }
    }

    /// <summary>
    /// Unsubscribes from the OnSelected and OnDeselected events of the PlaceableObject component.
    /// </summary>
    private void OnDisable()
    {
        if (placeableObject != null)
        {
            placeableObject.OnSelected -= HandleSelected;
            placeableObject.OnDeselected -= HandleDeselected;
        }
    }

    /// <summary>
    /// Handles the selection of the tower, showing the range indicator and upgrade menu.
    /// </summary>
    /// <param name="selected">The SelectableObject that was selected.</param>
    public void HandleSelected(SelectableObject selected)
    {
        if (rangeController != null)
        {
            rangeController.ShowRangeRadius();
        }
        if (upgradeManager != null)
        {
            upgradeManager.ShowUpgradeMenu();
        }
    }

    /// <summary>
    /// Handles the deselection of the tower, hiding the range indicator and upgrade menu.
    /// </summary>
    /// <param name="deselected">The SelectableObject that was deselected.</param>
    public void HandleDeselected(SelectableObject deselected)
    {
        if (rangeController != null)
        {
            rangeController.HideRangeRadius();
        }
        if (upgradeManager != null)
        {
            upgradeManager.HideUpgradeMenu();
        }
    }

    /// <summary>
    /// Notifies the RangeController to apply a modifier to the tower's range.
    /// </summary>
    /// <param name="modifier">The modifier to apply to the range.</param>
    public void HandleRangeModification(float modifier)
    {
        if (rangeController != null)
        {
            rangeController.ApplyRadiusModifier(modifier);
        }
    }

    /// <summary>
    /// Notifies the NearbyTowerDetector to update its detection radius based on the tower's current range.
    /// </summary>
    public void UpdateRangeDependencies()
    {
        if (nearbyTowerDetector != null)
        {
            nearbyTowerDetector.SetRadius(Range);
        }
    }

    /// <summary>
    /// Notifies the ElementalTowerController that a nearby tower with the given element has been added.
    /// </summary>
    /// <param name="element">The element of the nearby tower that was added.</param>
    public void HandleNearbyTowerAdded(TowerElement element)
    {
        if (elementalTowerController != null)
        {
            elementalTowerController.ModifyAttributes(element, 1);
        }
    }

    /// <summary>
    /// Notifies the ElementalTowerController that a nearby tower with the given element has been removed.
    /// </summary>
    /// <param name="element">The element of the nearby tower that was removed.</param>
    public void HandleNearbyTowerRemoved(TowerElement element)
    {
        if (elementalTowerController != null)
        {
            elementalTowerController.ModifyAttributes(element, -1);
        }
    }

    /// <summary>
    /// Gets the current elemental damage modifier from the ElementalTowerController.
    /// </summary>
    /// <returns>The current elemental damage modifier.</returns>
    public float GetElementalDamageModifier()
    {
        return elementalTowerController.GetElementalModifier();
    }
}