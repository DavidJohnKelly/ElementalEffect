using UnityEngine;

/// <summary>
/// Provides utility functions for working with object models and their bounds.
/// </summary>
public static class ModelUtil
{
    /// <summary>
    /// Gets the GameObject representing the model of a PlaceableObject.
    /// </summary>
    /// <param name="obj">The PlaceableObject to get the model from.</param>
    /// <returns>The GameObject with the "Model" tag that is a child of the PlaceableObject, or null if not found.</returns>
    public static GameObject GetObjectModel(PlaceableObject obj)
    {
        foreach (Transform transform in obj.transform)
        {
            if (transform.CompareTag("Model"))
            {
                return transform.gameObject;
            }
        }

        Debug.LogError("Error in ModelUtil! Model not found!");
        return null;
    }

    /// <summary>
    /// Gets the combined bounds of all renderers within a GameObject and its children.
    /// </summary>
    /// <param name="model">The GameObject containing the model.</param>
    /// <param name="bounds">When this method returns, contains the combined bounds of the model, or an empty Bounds if no renderers are found.</param>
    /// <returns>True if renderers were found and bounds were calculated, false otherwise.</returns>
    public static bool GetModelBounds(GameObject model, out Bounds bounds)
    {
        bounds = new Bounds();
        if (model == null) return false;

        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("Error in ModelUtil! No renderers found!");
            return false;
        }

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return true;
    }

    /// <summary>
    /// Transforms world space bounds to the local space of a target transform.
    /// </summary>
    /// <param name="worldBounds">The bounds in world space.</param>
    /// <param name="targetTransform">The transform to convert the bounds to local space of.</param>
    /// <returns>The bounds transformed to the local space of the target transform.</returns>
    public static Bounds TransformBoundsToLocal(Bounds worldBounds, Transform targetTransform)
    {
        Vector3 localCenter = targetTransform.InverseTransformPoint(worldBounds.center);
        Vector3 localSize = targetTransform.InverseTransformVector(worldBounds.size);
        return new Bounds(localCenter, localSize);
    }

    /// <summary>
    /// Gets the combined bounds of the model associated with a PlaceableObject.
    /// </summary>
    /// <param name="obj">The PlaceableObject to get the model bounds from.</param>
    /// <param name="bounds">When this method returns, contains the combined bounds of the model, or an empty Bounds if the model or its renderers are not found.</param>
    /// <returns>True if the model and its renderers were found and bounds were calculated, false otherwise.</returns>
    public static bool GetObjectBounds(PlaceableObject obj, out Bounds bounds)
    {
        GameObject model = GetObjectModel(obj);
        return GetModelBounds(model, out bounds);
    }

    /// <summary>
    /// Gets half the height of the model.
    /// </summary>
    /// <param name="model">The GameObject containing the model.</param>
    /// <returns>Half the height of the model's bounds, or 0 if bounds cannot be determined.</returns>
    public static float GetModelHalfHeight(GameObject model)
    {
        bool valid_bounds = GetModelBounds(model, out var bounds);
        if (!valid_bounds)
        {
            Debug.LogError("Error in ModelUtil! Model bounds not found!");
            return 0;
        }
        else return bounds.extents.y;
    }

    /// <summary>
    /// Gets half the height of the model associated with a PlaceableObject.
    /// </summary>
    /// <param name="obj">The PlaceableObject to get the model height from.</param>
    /// <returns>Half the height of the PlaceableObject's model bounds, or 0 if the model or its bounds cannot be determined.</returns>
    public static float GetObjectHalfHeight(PlaceableObject obj)
    {
        GameObject model = GetObjectModel(obj);
        if (model == null)
        {
            Debug.LogError("Error in ModelUtil! Model not found!");
            return 0;
        }
        else return GetModelHalfHeight(model);
    }

    /// <summary>
    /// Gets the point on the bounds that is furthest in the direction of movement.
    /// </summary>
    /// <param name="bounds">The bounds to check.</param>
    /// <param name="movementDirection">The direction of movement.</param>
    /// <returns>The frontmost point on the bounds in the given direction.</returns>
    public static Vector3 GetFrontmostPoint(Bounds bounds, Vector3 movementDirection)
    {
        Vector3 frontmostPoint = bounds.center;
        frontmostPoint.x += bounds.extents.x * Mathf.Sign(movementDirection.x);
        frontmostPoint.y += bounds.extents.y * Mathf.Sign(movementDirection.y);
        frontmostPoint.z += bounds.extents.z * Mathf.Sign(movementDirection.z);
        return frontmostPoint;
    }

    /// <summary>
    /// Fits a collider to the provided bounds. Supports BoxCollider and CapsuleCollider.
    /// </summary>
    /// <param name="collider">The collider to fit.</param>
    /// <param name="bounds">The bounds to fit the collider to.</param>
    public static void FitCollider(Collider collider, Bounds bounds)
    {
        if (collider == null)
        {
            Debug.LogError("Error in ModelUtil! collider is null!");
        }

        if (collider is BoxCollider boxCollider)
        {
            boxCollider.center = bounds.center;
            boxCollider.size = bounds.size;
        }
        else if (collider is CapsuleCollider capsuleCollider)
        {
            Vector3 size = bounds.size;
            capsuleCollider.center = bounds.center;
            capsuleCollider.radius = Mathf.Max(size.x, size.z) * 0.5f;
            capsuleCollider.height = size.y;
            capsuleCollider.direction = 1;
        }
    }
}