using UnityEngine;

/// <summary>
/// Manages the display and behavior of a preview object before placement.
/// </summary>
public class PreviewObject : MonoBehaviour
{
    /// <summary>
    /// Gets the current position of the preview object.
    /// </summary>
    public Vector3 Position { get => previewObject.transform.position; }
    /// <summary>
    /// Gets the GameObject of the current preview object.
    /// </summary>
    public GameObject PreviewGameObject { get => GetPreview(); }

    // The currently instantiated preview PlaceableObject.
    private PlaceableObject previewObject;
    // The PlacementValidator component attached to the preview object.
    private PlacementValidator placementValidator;

    // Returns the GameObject of the preview object if it exists.
    private GameObject GetPreview()
    {
        if (previewObject != null)
        {
            return previewObject.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Shows or hides the preview object.
    /// </summary>
    /// <param name="show">True to show the preview, false to hide it.</param>
    public void ShowPreview(bool show)
    {
        if (previewObject != null)
        {
            previewObject.SetActive(show);
        }
    }

    /// <summary>
    /// Shows or hides visual features of the preview, like color and range indicators.
    /// </summary>
    /// <param name="show">True to show the features, false to hide them.</param>
    public void ShowPreviewFeatures(bool show)
    {
        UpdatePreviewColour(show);
        ShowPreviewRange(show);
        EnableNearbyTowerSynergy(show);
    }

    /// <summary>
    /// Destroys the current preview object and resets the internal reference.
    /// </summary>
    public void CleanupPreview()
    {
        if (previewObject != null)
        {
            previewObject.SetActive(false);
            Destroy(previewObject);
        }
        previewObject = null;
    }

    /// <summary>
    /// Creates a preview object based on the provided PlaceableObject.
    /// </summary>
    /// <param name="obj">The PlaceableObject to create a preview of.</param>
    public void CreatePreviewObject(PlaceableObject obj)
    {
        if (obj == null) return;

        CleanupPreview();
        previewObject = Instantiate(obj);
        placementValidator = previewObject.GetComponentInChildren<PlacementValidator>();
        ShowPreviewFeatures(false);
        ShowPreview(false);
    }

    /// <summary>
    /// Updates the color of the preview object based on placement validity.
    /// </summary>
    /// <param name="isPlacementValid">True if the placement is valid, false otherwise.</param>
    public void UpdatePreviewColour(bool isPlacementValid)
    {
        if (previewObject == null) return;

        previewObject.SetColour(isPlacementValid ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f)); // Green if valid, Red if invalid
    }

    /// <summary>
    /// Shows or hides the range indicator of the preview object.
    /// </summary>
    /// <param name="isPlacementValid">True to show the range if placement is valid, false otherwise.</param>
    public void ShowPreviewRange(bool isPlacementValid)
    {
        if (previewObject == null) return;

        RangeController rangeController = previewObject.GetComponentInChildren<RangeController>();

        if (rangeController == null) return;

        if (isPlacementValid)
        {
            rangeController.ShowRangeRadius();
        }
        else
        {
            rangeController.HideRangeRadius();
        }
    }

    /// <summary>
    /// Enables or disables the nearby tower synergy detection on the preview object.
    /// </summary>
    /// <param name="isPlacementValid">True to enable synergy detection if placement is valid, false otherwise.</param>
    public void EnableNearbyTowerSynergy(bool isPlacementValid)
    {
        if (previewObject == null) return;

        NearbyTowerDetector nearbyTowerDetector = previewObject.GetComponentInChildren<NearbyTowerDetector>();

        if (nearbyTowerDetector == null) return;

        nearbyTowerDetector.enabled = isPlacementValid;
    }

    /// <summary>
    /// Aligns the preview object to the position of the provided RaycastHit.
    /// </summary>
    /// <param name="hitPoint">The RaycastHit containing the position to align to.</param>
    public void Align(RaycastHit hitPoint)
    {
        if (previewObject == null) return;

        //float bottomOffset = ModelUtil.GetObjectHalfHeight(previewObject);
        previewObject.transform.position = hitPoint.point;// + new Vector3(0, bottomOffset, 0);
    }

    /// <summary>
    /// Checks if the current preview object's placement is valid.
    /// </summary>
    /// <returns>True if the placement is valid, false otherwise.</returns>
    public bool CheckPlacementValid()
    {
        if (placementValidator == null)
        {
            Debug.Log("ERROR! PlacementValidator is null!");
            return false;
        }
        return placementValidator.CheckPlacementValid();
    }
}