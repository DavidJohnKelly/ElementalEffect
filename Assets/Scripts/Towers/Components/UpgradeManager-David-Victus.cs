using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the upgrade functionality for a tower, including displaying the upgrade UI and handling the upgrade process.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    /// <summary>
    /// The prefab for the upgrade UI that will be displayed above the tower.
    /// </summary>
    [Tooltip("The prefab for the upgrade UI that will be displayed above the tower.")]
    public GameObject UpgradeUIPrefab;
    /// <summary>
    /// The offset in world space to position the upgrade UI relative to the tower.
    /// </summary>
    [Tooltip("The offset in world space to position the upgrade UI relative to the tower.")]
    public Vector2 UIOffset = new(0, 10);

    // Reference to the parent TowerController.
    private TowerController towerController;
    // The main camera used for world-to-screen space conversions.
    private Camera selectionCamera;
    // The GameObject holding the upgrade UI canvas.
    private GameObject canvasObject;
    // The Canvas component of the upgrade UI.
    private Canvas upgradeUICanvas;
    // The cost of the next upgrade level.
    private float nextUpgradeCost;
    // Flag indicating if the tower is at its maximum upgrade level.
    private bool onFinalUpgrade = false;

    /// <summary>
    /// Retrieves the TowerController component from the parent.
    /// </summary>
    private void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
    }

    /// <summary>
    /// Finds the main camera, checks for null references, and determines the next upgrade cost and final upgrade status.
    /// </summary>
    private void Start()
    {
        selectionCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if (selectionCamera == null)
        {
            Debug.LogError("Selection camera is null!");
        }
        if (UpgradeUIPrefab == null)
        {
            Debug.LogError("Upgrade UI is null!");
        }

        if (towerController.NextTowerCost == -1f)
        {
            onFinalUpgrade = true;
        }
        else
        {
            nextUpgradeCost = towerController.NextTowerCost;
        }
    }

    /// <summary>
    /// Shows the upgrade menu UI above the tower.
    /// </summary>
    public void ShowUpgradeMenu()
    {
        if (selectionCamera == null) return;

        if (canvasObject == null)
        {
            CreateCanvasAndUI();
        }

        Vector3 worldOffset = selectionCamera.transform.TransformDirection(new Vector3(UIOffset.x, UIOffset.y, 0));
        canvasObject.transform.position = transform.position + worldOffset;
        canvasObject.SetActive(true);
    }

    /// <summary>
    /// Hides the upgrade menu UI.
    /// </summary>
    public void HideUpgradeMenu()
    {
        if (canvasObject != null)
        {
            canvasObject.SetActive(false);
        }
    }

    /// <summary>
    /// Destroys the GameObject this script is attached to when it is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    // Creates the canvas and instantiates the upgrade UI prefab.
    private void CreateCanvasAndUI()
    {
        canvasObject = new GameObject("UpgradeUICanvas");
        canvasObject.transform.parent = transform;

        upgradeUICanvas = canvasObject.AddComponent<Canvas>();
        canvasObject.AddComponent<GraphicRaycaster>();
        canvasObject.AddComponent<Billboard>();
        upgradeUICanvas.renderMode = RenderMode.WorldSpace;
        upgradeUICanvas.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        RectTransform canvasRect = upgradeUICanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(200, 100);
        Vector3 initialOffset = selectionCamera.transform.TransformDirection(new Vector3(UIOffset.x, UIOffset.y, 0));
        canvasRect.position = transform.position + initialOffset;

        GameObject upgradeUIInstance = Instantiate(UpgradeUIPrefab, canvasRect);
        upgradeUIInstance.transform.SetParent(canvasObject.transform);
        if (onFinalUpgrade)
        {
            upgradeUIInstance.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = "Finished";
            upgradeUIInstance.GetComponentInChildren<TMP_Text>().text = "";
        }
        else
        {
            upgradeUIInstance.GetComponentInChildren<TMP_Text>().text = "Cost: " + nextUpgradeCost;
        }

        canvasObject.SetActive(false);
        AttachButtonListener();
    }

    /// <summary>
    /// Initiates the tower upgrade process if the player has enough money and the tower is not at its final level.
    /// </summary>
    public void Upgrade()
    {
        if (towerController == null || onFinalUpgrade) return;

        if (GameController.Instance.CurrentMoney < nextUpgradeCost)
        {
            Debug.Log("Not enough money!");
            return;
        }

        GameController.Instance.ApplyCost(nextUpgradeCost);

        TowerLevel nextLevel = TowerData.GetNextLevel(towerController.TowerLevel);
        if (towerController.TowerLevel == nextLevel) return;
        PlaceableObject newTower = TowerFactory.Instance.GetTower(towerController.TowerType, towerController.TowerElement, nextLevel);

        newTower.transform.position = transform.position;
        newTower.SetActive(true);

        Destroy(towerController.TowerBase);
    }

    // Attaches a listener to the button in the upgrade UI to call the Upgrade method.
    private void AttachButtonListener()
    {
        Button button = canvasObject.GetComponentInChildren<Button>();
        if (button == null)
        {
            Debug.LogError("Button not found in the prefab!");
            return;
        }
        button.onClick.AddListener(Upgrade);
    }
}