using UnityEngine;

/// <summary>
/// Represents an object that can be placed in the game world.
/// </summary>
public class PlaceableObject : SelectableObject
{
    /// <summary>
    /// The LayerMask defining the layers where this object can be placed.
    /// </summary>
    [Tooltip("The LayerMask defining the layers where this object can be placed.")]
    public LayerMask PlacementLayer;

    // The currently instantiated model GameObject for this placeable object.
    private GameObject modelInstance;

    // The BoxCollider used for placement validation.
    BoxCollider placementCollider;

    /// <summary>
    /// Sets the visual model for this placeable object.
    /// </summary>
    /// <param name="model">The GameObject to use as the model.</param>
    public void SetModel(GameObject model)
    {
        if (model == null) return;

        if (modelInstance != null)
        {
            Destroy(modelInstance);
        }

        modelInstance = Instantiate(model, transform);
        modelInstance.tag = "Model";
        UpdateRenderers();
        placementCollider = GetComponentInChildren<BoxCollider>();
        if (ModelUtil.GetModelBounds(modelInstance, out var bounds))
        {
            ModelUtil.FitCollider(placementCollider, bounds);
            placementCollider.center += new Vector3(0, 0.25f, 0);  // Ensure no collision with ground
        }
        modelInstance.SetActive(true);
    }

    // Destroys the instantiated model instance when this object is destroyed.
    private void OnDestroy()
    {
        Destroy(modelInstance);
        Destroy(gameObject);
    }
}