using UnityEngine;

/// <summary>
/// Handles the selection of PlaceableObjects in the game world using raycasting.
/// </summary>
public class ObjectSelector : MonoBehaviour
{
    // The currently selected PlaceableObject.
    private PlaceableObject selectedObject;
    /// <summary>
    /// Gets or sets the currently selected PlaceableObject. Setting this will deselect the previously selected object.
    /// </summary>
    public PlaceableObject SelectedObject { get => selectedObject; set => SelectObject(value); }
    // A flag indicating whether object selection is currently enabled.
    private bool canSelect = false;
    /// <summary>
    /// Gets or sets whether object selection is currently enabled.
    /// </summary>
    public bool CanSelect { get => canSelect; set => SetCanSelect(value); }

    /// <summary>
    /// Attempts to select a PlaceableObject based on a raycast.
    /// </summary>
    /// <param name="ray">The ray to cast for object selection.</param>
    /// <param name="layer">The LayerMask to filter selectable objects.</param>
    /// <returns>True if an object was successfully selected, false otherwise.</returns>
    public bool TrySelectObject(Ray ray, LayerMask layer)
    {
        bool validSelection = TryGetSelectedObject(ray, layer, out var selected);
        if (validSelection) SelectedObject = selected;
        return validSelection;
    }

    // Deselects the currently selected object, if any.
    private void DeselectCurrent()
    {
        if (selectedObject != null)
        {
            selectedObject.Deselect();
        }
        selectedObject = null;
    }

    // Enables or disables the object selection functionality.
    private void SetCanSelect(bool enabled)
    {
        gameObject.SetActive(enabled);
        canSelect = enabled;
    }

    // Attempts to get a PlaceableObject from a raycast hit.
    private bool TryGetSelectedObject(Ray ray, LayerMask mask, out PlaceableObject selected)
    {
        selected = null;
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, mask);

        foreach (RaycastHit hit in hits)
        {
            GameObject objectHit = hit.collider.gameObject;

            if (objectHit.transform.parent == null) continue;

            GameObject objectParent = objectHit.transform.parent.gameObject;

            if (objectParent.TryGetComponent(out PreviewObject _)) continue;

            if (objectParent.TryGetComponent(out PlaceableObject newSelection))
            {
                if (newSelection == selected) continue;

                selected = newSelection;
                return true;
            }
        }

        return false;
    }

    // Selects a new PlaceableObject, deselecting the previously selected one.
    private void SelectObject(PlaceableObject newSelection)
    {
        DeselectCurrent();
        selectedObject = newSelection;

        if (newSelection != null)
        {
            selectedObject.Select();
        }
    }
}