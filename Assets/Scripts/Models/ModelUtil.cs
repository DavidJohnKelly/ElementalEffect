using UnityEngine;

public static class ModelUtil
{
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

    public static Bounds TransformBoundsToLocal(Bounds worldBounds, Transform targetTransform)
    {
        Vector3 localCenter = targetTransform.InverseTransformPoint(worldBounds.center);
        Vector3 localSize = targetTransform.InverseTransformVector(worldBounds.size);
        return new Bounds(localCenter, localSize);
    }

    public static bool GetObjectBounds(PlaceableObject obj, out Bounds bounds)
    {
        GameObject model = GetObjectModel(obj);
        return GetModelBounds(model, out bounds);
    }

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

    public static Vector3 GetFrontmostPoint(Bounds bounds, Vector3 movementDirection)
    {
        Vector3 frontmostPoint = bounds.center;
        frontmostPoint.x += bounds.extents.x * Mathf.Sign(movementDirection.x);
        frontmostPoint.y += bounds.extents.y * Mathf.Sign(movementDirection.y);
        frontmostPoint.z += bounds.extents.z * Mathf.Sign(movementDirection.z);
        return frontmostPoint;
    }
}