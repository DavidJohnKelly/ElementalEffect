using UnityEngine;

/// <summary>
/// Handles the placement of objects in the game world, including previewing and validating placement.
/// </summary>
public class ObjectPlacer : MonoBehaviour
{
    // The currently selected PlaceableObject to place.
    private PlaceableObject objectInstance;
    /// <summary>
    /// Gets or sets the PlaceableObject that will be placed. Setting this will update the preview.
    /// </summary>
    public PlaceableObject ObjectToPlace { get => objectInstance; set => SetObjectToPlace(value); }
    // The PreviewObject component responsible for displaying the placement preview.
    private PreviewObject previewObject;
    /// <summary>
    /// Gets the GameObject of the preview object.
    /// </summary>
    public GameObject PreviewObject { get => previewObject.PreviewGameObject; }
    // A flag indicating whether the object placement functionality is currently enabled.
    private bool canPlace = false;
    /// <summary>
    /// Gets or sets whether object placement is currently enabled.
    /// </summary>
    public bool CanPlace { get => canPlace; set => SetCanPlace(value); }

    // A flag indicating if the player is currently moving an existing object.
    private bool isMovingObject;
    // A flag indicating if the current preview placement is valid.
    private bool isPlacementValid;

    /// <summary>
    /// Initializes the PreviewObject component.
    /// </summary>
    private void Awake()
    {
        previewObject = gameObject.AddComponent<PreviewObject>();
    }

    // Updates the position of the preview object based on the raycast hit point.
    private void UpdatePreviewPosition(RaycastHit hitPoint)
    {
        previewObject.Align(hitPoint);
        previewObject.ShowPreview(true);
    }

    // Enables or disables the object placement functionality and the preview object.
    private void SetCanPlace(bool enabled)
    {
        if (PreviewObject)
        {
            PreviewObject.SetActive(enabled);
        }
        gameObject.SetActive(enabled);
        canPlace = enabled;
    }

    /// <summary>
    /// Updates the preview object's position, visibility, and features based on the raycast hit point.
    /// </summary>
    /// <param name="hitPoint">The RaycastHit containing the potential placement position.</param>
    public void UpdatePreview(RaycastHit hitPoint)
    {
        if (previewObject == null) return;

        if (ObjectToPlace == null)
        {
            previewObject.CleanupPreview();
            return;
        }

        isPlacementValid = previewObject.CheckPlacementValid();

        if (!isMovingObject)
        {
            isPlacementValid = isPlacementValid && IsWithinCost();
        }

        previewObject.ShowPreviewFeatures(isPlacementValid);
        UpdatePreviewPosition(hitPoint);
    }

    /// <summary>
    /// Checks if the cost of the object to place is within the player's current money.
    /// </summary>
    /// <returns>True if the cost is within the budget, false otherwise.</returns>
    public bool IsWithinCost()
    {
        if (ObjectToPlace.TryGetComponent<TowerController>(out var towerController))
        {
            return towerController.TowerCost <= GameController.Instance.CurrentMoney;
        }

        return true;
    }

    /// <summary>
    /// Handles the placement of the currently previewed object if the placement is valid.
    /// </summary>
    public void HandlePlacement()
    {
        if (isPlacementValid)
        {
            PlaceObject();
        }
    }

    /// <summary>
    /// Cancels the movement of a currently moving object, returning it to its original position.
    /// </summary>
    public void CancelMovement()
    {
        if (isMovingObject)
        {
            ObjectToPlace.SetActive(true);
            isMovingObject = false;
            ObjectToPlace = null;
        }
    }

    /// <summary>
    /// Starts the process of moving an existing PlaceableObject.
    /// </summary>
    /// <param name="obj">The PlaceableObject to start moving.</param>
    public void StartMovingObject(PlaceableObject obj)
    {
        if (obj == null) return;

        if (isMovingObject)
        {
            CancelMovement();
        }

        isMovingObject = true;
        obj.SetActive(false);
        ObjectToPlace = obj;
    }

    // Instantiates the object to place at the preview location.
    private void PlaceObject()
    {
        if (isMovingObject)
        {
            InstantiateObjectToPlace();
            Destroy(ObjectToPlace);
            ObjectToPlace = null;
            isMovingObject = false;
            return;
        }

        if (ObjectToPlace.TryGetComponent<TowerController>(out var towerController))
        {
            InstantiateObjectToPlace();
            GameController.Instance.ApplyCost(towerController.TowerCost);
            return;
        }
    }

    // Instantiates the currently selected object at the preview position.
    private void InstantiateObjectToPlace()
    {
        ObjectToPlace.SetActive(false);
        PlaceableObject placedObject = Instantiate(ObjectToPlace, previewObject.Position, Quaternion.identity);
        placedObject.SetActive(true);
        placedObject.Deselect();
    }

    /// <summary>
    /// Sets the object to be placed and updates the preview accordingly.
    /// </summary>
    /// <param name="newObject">The PlaceableObject to set for placement.</param>
    private void SetObjectToPlace(PlaceableObject newObject)
    {
        previewObject.CleanupPreview();
        objectInstance = newObject;
        if (objectInstance != null)
        {
            objectInstance.SetActive(false);
            previewObject.CreatePreviewObject(newObject);
        }
    }
}