using UnityEngine;

// Pass all communications through the Tower Controller.
// Avoids spaghetti code and makes it easier to manage the game.
public class TowerController : MonoBehaviour
{
    public TowerType towerType;
    public TowerElement towerElement;
    public TowerLevel towerLevel;
    public bool CanShoot { get => gameController.InFPSStage; }
    public float Radius { get => rangeController.GetRangeRadius(); }
    public PlaceableObject TowerBase { get => placeableObject; }

    private GameController gameController;
    private PlaceableObject placeableObject;
    private UpgradeManager upgradeManager;
    private RangeController rangeController;
    private ElementalTowerController elementalTowerController;
    private NearbyTowerDetector nearbyTowerDetector;

    private void Awake()
    {
        gameController = GameController.Instance;
        placeableObject = GetComponent<PlaceableObject>();
        upgradeManager = GetComponentInChildren<UpgradeManager>();
        rangeController = GetComponentInChildren<RangeController>();
        elementalTowerController = GetComponentInChildren<ElementalTowerController>();
        nearbyTowerDetector = GetComponentInChildren<NearbyTowerDetector>();
    }

    private void OnEnable()
    {
        if (placeableObject != null)
        {
            placeableObject.OnSelected += HandleSelected;
            placeableObject.OnDeselected += HandleDeselected;
        }
    }

    private void OnDisable()
    {
        if (placeableObject != null)
        {
            placeableObject.OnSelected -= HandleSelected;
            placeableObject.OnDeselected -= HandleDeselected;
        }
    }

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

    public void HandleRadiusUpdate(float radius)
    {
        if (nearbyTowerDetector != null)
        {
            nearbyTowerDetector.SetRadius(radius);
        }
    }

    public void HandleNearbyTowerAdded()
    {
        if (elementalTowerController != null)
        {
            elementalTowerController.IncreaseAttributes();
        }
    }

    public void HandleNearbyTowerRemoved()
    {
        if (elementalTowerController != null)
        {
            elementalTowerController.DecreaseAttributes();
        }
    }

}
