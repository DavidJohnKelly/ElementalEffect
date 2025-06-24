using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public GameObject UpgradeUIPrefab;
    public Vector2 UIOffset = new(0, 10);

    private TowerController towerController;
    private Camera selectionCamera;
    private GameObject canvasObject;
    private Canvas upgradeUICanvas;

    private void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
    }

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
    }

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

    public void HideUpgradeMenu()
    {
        if (canvasObject != null)
        {
            canvasObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    private void CreateCanvasAndUI()
    {
        canvasObject = new GameObject("UpgradeUICanvas");
        canvasObject.transform.parent = transform;

        upgradeUICanvas = canvasObject.AddComponent<Canvas>();
        upgradeUICanvas.AddComponent<GraphicRaycaster>();
        upgradeUICanvas.AddComponent<Billboard>();
        upgradeUICanvas.renderMode = RenderMode.WorldSpace;
        upgradeUICanvas.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        RectTransform canvasRect = upgradeUICanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(200, 100);
        Vector3 initialOffset = selectionCamera.transform.TransformDirection(new Vector3(UIOffset.x, UIOffset.y, 0));
        canvasRect.position = transform.position + initialOffset;

        GameObject upgradeUIInstance = Instantiate(UpgradeUIPrefab, canvasRect);
        upgradeUIInstance.transform.SetParent(canvasObject.transform);

        canvasObject.SetActive(false);
        AttachButtonListener();
    }

    public void Upgrade()
    {
        if (towerController == null) return;
        TowerLevel nextLevel = TowerData.GetNextLevel(towerController.towerLevel);
        if (towerController.towerLevel == nextLevel) return;
        PlaceableObject newTower = TowerFactory.Instance.CreateTower(towerController.towerType, towerController.towerElement, nextLevel);

        //float originalHeight = ModelUtil.GetObjectHalfHeight(towerController.TowerBase);
        //float newHeight = ModelUtil.GetObjectHalfHeight(newTower);

        newTower.transform.position = transform.position;// + new Vector3(0, newHeight - originalHeight, 0);
        newTower.SetActive(true);

        Destroy(towerController.TowerBase);
    }

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