using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public Vector3 Position { get => previewObject.transform.position; }
    public GameObject PreviewGameObject { get => GetPreview(); }

    private PlaceableObject previewObject;
    private PlacementValidator placementValidator;

    private GameObject GetPreview()
    {
        if (previewObject != null)
        {
            return previewObject.gameObject;
        }
        return null;
    }

    public void ShowPreview(bool show)
    {
        if (previewObject != null)
        {
            previewObject.SetActive(show);
        }
    }

    public void ShowPreviewFeatures(bool show)
    {
        UpdatePreviewColour(show);
        ShowPreviewRange(show);
    }

    public void CleanupPreview()
    {
        if (previewObject != null)
        {
            previewObject.SetActive(false);
            Destroy(previewObject);
            previewObject = null;
        }
    }

    public void CreatePreviewObject(PlaceableObject obj)
    {
        if (obj == null) return;

        CleanupPreview();
        previewObject = Instantiate(obj);
        placementValidator = previewObject.GetComponentInChildren<PlacementValidator>();
        ShowPreview(false);
    }

    public void UpdatePreviewColour(bool isPlacementValid)
    {
        if (previewObject == null) return;

        previewObject.SetColour(isPlacementValid ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f)); // Green if valid, Red if invalid
    }

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

    public void Align(RaycastHit hitPoint)
    {
        if (previewObject == null) return;

        //float bottomOffset = ModelUtil.GetObjectHalfHeight(previewObject);
        previewObject.transform.position = hitPoint.point;// + new Vector3(0, bottomOffset, 0);
    }

    public bool CheckPlacementValid()
    {
        return placementValidator.CheckPlacementValid();
    }
}